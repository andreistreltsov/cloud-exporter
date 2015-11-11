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
module Main 

open System
open System.Security.Permissions
open System.IO
open CommonTypes


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
