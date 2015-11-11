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
module Configuration

open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open CommonTypes
open Routing


type T = { Source: DirPath; Routes: Route seq; IgnoreList: string seq }
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