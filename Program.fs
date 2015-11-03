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

[<EntryPoint>]
[<PermissionSet(SecurityAction.Demand, Name="FullTrust")>]
let main argv = 
    match (Configuration.create argv) with 
        | Some c -> 
            let watcher = createFileHandler c.Destination |> initializeWathcher c.Source
            Console.ReadLine() |> ignore

        | None -> Console.WriteLine("Invalid usage. Pass the source directory as the first argument and destination directory as the second argument.")
    0


