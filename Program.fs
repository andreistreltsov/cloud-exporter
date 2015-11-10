// CloudExporter is a tool that simplifies saving files from the cloud locally.
//
// Copyright (C) 2015 Andrei Streltsov <andrei@astreltsov.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>. 

open System
open System.Security.Permissions
open System.IO
open System.Text.RegularExpressions
open Newtonsoft.Json
open Newtonsoft.Json.Linq

type DirPath = DirPath of string

module IncomingFile =
    type T = IncomingFile of string
    let create filePath = IncomingFile(filePath)
    let name (IncomingFile filePath) = Path.GetFileName(filePath)
    let copyTo (IncomingFile filePath) (DirPath directory) = 
        File.Copy(filePath, Path.Combine(directory, Path.GetFileName(filePath)), true)
    let remove (IncomingFile filePath) = File.Delete(filePath)
    let matchesPattern file pattern = Regex.IsMatch(name file, pattern)
    let matchesAnyPattern file patterns = patterns |> Seq.where (fun p -> matchesPattern file p) |> Seq.length > 0

module Route = 
    type T = { Pattern: string; Destination: DirPath }
    let matchesFile route file = IncomingFile.matchesPattern file route.Pattern

type RoutedFile = { File:IncomingFile.T; Destinations:DirPath seq }

module Watcher = 
    let create (DirPath watchDir) handler = 
        let fsWatcher = new FileSystemWatcher(watchDir)
        fsWatcher.Created.Add(fun(x:FileSystemEventArgs) -> handler <| IncomingFile.create(x.FullPath))
        fsWatcher.EnableRaisingEvents <- true 
        fsWatcher

module Router = 
    let routeFile routes file = 
        let matchingDestinations = 
            routes 
            |> Seq.where (fun r -> Route.matchesFile r file) 
            |> Seq.map (fun r -> r.Destination)
            |> Seq.distinct
        if Seq.length matchingDestinations > 0 
            then Some({ File=file; Destinations=matchingDestinations })
            else None

let distribute routedFile = 
    let copyFileTo = IncomingFile.copyTo routedFile.File
    routedFile.Destinations |> Seq.iter (fun dest -> copyFileTo dest)

module IncomingFileHandler =
    let log file =  
        file.Destinations 
            |> Seq.map (fun (DirPath d) -> d) 
            |> Seq.iter (fun d -> Console.WriteLine(String.Format("[{0}] -> [{1}]", IncomingFile.name file.File, d)))
    let ignore file = Console.WriteLine(String.Format("[{0}] -> ignoring", IncomingFile.name file))
    let remove file = IncomingFile.remove file.File
    let create routes ignorePatterns = fun(file) -> 
        if IncomingFile.matchesAnyPattern file ignorePatterns
            then ignore file
            else
                let route = Router.routeFile routes
                match(route file) with
                | Some file -> 
                    log file
                    distribute file
                    remove file 
                | None -> 
                    ignore file 

module Configuration =
    type T = { Source: DirPath; Routes: Route.T seq; IgnoreList: string seq }
    let read configFilePath = 
        let jsonConfig = JObject.Parse(File.ReadAllText(configFilePath))
        { 
            Source = DirPath((string)jsonConfig.["source"]); 
            IgnoreList = jsonConfig.["ignore"] 
                |> Seq.filter (fun t -> t.Type = JTokenType.String ) 
                |> Seq.map (fun t -> (string)t);
            Routes = jsonConfig.["routes"] 
                |> Seq.filter (fun t -> t.Type = JTokenType.Object) 
                |> Seq.map (fun t -> { Pattern=(string)t.["pattern"]; Destination=DirPath((string)t.["destination"])})  
        }


[<EntryPoint>]
[<PermissionSet(SecurityAction.Demand, Name="FullTrust")>]
let main argv = 
    let configFilePath argv = if Array.length(argv) = 1 then Some(Path.GetFullPath(argv.[0])) else None
    match(configFilePath argv) with
        | Some configFile ->
            let config = Configuration.read configFile
            let (DirPath watchPath) = config.Source
            Console.WriteLine(String.Format("watching [{0}] ...", watchPath))
            let watcher = Watcher.create config.Source <| IncomingFileHandler.create config.Routes config.IgnoreList
            Console.ReadLine() |> ignore
        | None -> Console.WriteLine("Invalid usage. Pass the configuration file path as the first argument.")
    0


