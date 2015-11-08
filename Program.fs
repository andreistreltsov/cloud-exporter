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

type DirPath = DirPath of string

module Configuration = 
    type T = { Source:DirPath; Destination:DirPath }

    let create argv = 
        if Array.length(argv) >= 2
        then Some {Source = DirPath(argv.[0]); Destination = DirPath(argv.[1])}
        else None

module IncomingFile =
    type T = IncomingFile of string

    let create filePath = IncomingFile(filePath)

    let name (IncomingFile filePath) = 
        Path.GetFileName(filePath)

    let path (IncomingFile filePath) = filePath

    let copyTo (file:T) (DirPath directory) = File.Copy(path file, Path.Combine(directory, name file), true)

    let isExportFile (file:T) = 
        let fileName = name file
        (fileName.Contains(".x.") || fileName.EndsWith(".x")) && (not(fileName.EndsWith(".crdownload")))&& (not(fileName.StartsWith(".")))

    let printToConsole (file:T) = printfn "%s" (name file)

    let remove (IncomingFile filePath) = 
        File.Delete(filePath)


let createFileHandler destination =
    fun (file:IncomingFile.T) ->
        if IncomingFile.isExportFile file
        then
            IncomingFile.printToConsole file
            IncomingFile.copyTo file destination
            IncomingFile.remove file

let initializeWathcher (DirPath watchPath) fileHandler = 
    let fsWatcher = new FileSystemWatcher(watchPath)
    fsWatcher.Created.Add(fun(x:FileSystemEventArgs) -> fileHandler <| IncomingFile.create(x.FullPath))
    fsWatcher.EnableRaisingEvents <- true 
    fsWatcher


type Route = { Pattern: string; Destination: DirPath }

module Conf =
    type T = { Source: DirPath; Routes: Route list }

    let read configFilePath = { Source=DirPath("/home/andrei/Downloads"); Routes=[
                                                                    {Pattern="\.r\."; Destination=DirPath("/home/andrei/temp")};
                                                                    {Pattern="\.r$"; Destination=DirPath("/home/andrei/temp")}
                                                                  ]}
module Watcher = 
    let create handler = 0


module IncomingFileHander =
    let create routes = 0


[<EntryPoint>]
[<PermissionSet(SecurityAction.Demand, Name="FullTrust")>]
let main argv = 
    let configFilePath argv = if Array.length(argv) = 1 then Some(Path.GetFullPath(argv.[0])) else None

    match(configFilePath argv) with
        | Some configFile ->
            let config = Conf.read configFile

            //let watcher = Watcher.create config.Source <| IncomingFileHanler.create config.Routes
            Console.ReadLine() |> ignore

        | None -> Console.WriteLine("Invalid usage. Pass the configuration file path as the first argument.")
    0


