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
module IncomingFileHandler

open System
open CommonTypes
open Routing
open IncomingFile


let create routes ignorePatterns = fun(file) -> 

    let remove file = IncomingFile.remove file.File

    let ignore file = Console.WriteLine(String.Format("[{0}] -> ignoring", IncomingFile.name file))

    let distribute file = 
        file.Destinations |> Seq.iter (fun dest -> IncomingFile.copyTo file.File dest)
        file

    let log file =  
        file.Destinations 
            |> Seq.map (fun (DirPath d) -> d) 
            |> Seq.iter (fun d -> Console.WriteLine(String.Format("[{0}] -> [{1}]", IncomingFile.name file.File, d)))
        file

    if IncomingFile.matchesAnyPattern file ignorePatterns
        then ignore file
    else 
        match(Routing.routeFile routes file) with
        | Some file -> file |> log |> distribute |> remove
        | None -> ignore file 