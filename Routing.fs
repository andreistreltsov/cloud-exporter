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
module Routing

open CommonTypes


type Route = { Pattern: string; Destination: DirPath }

type RoutedFile = { File:IncomingFile.T; Destinations:DirPath seq }

let matchesFile route file = IncomingFile.matchesPattern file route.Pattern

let routeFile routes file = 
    let matchingDestinations = 
        routes 
        |> Seq.where (fun r -> matchesFile r file) 
        |> Seq.map (fun r -> r.Destination)
        |> Seq.distinct
    if Seq.length matchingDestinations > 0 
        then Some({ File=file; Destinations=matchingDestinations })
        else None


