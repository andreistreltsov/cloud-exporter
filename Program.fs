open System
open System.Security.Permissions
open System.IO


let printToConsole filePath = printfn "%s" filePath

let copyToDestination filePath destinationPath = 
    File.Copy(filePath, Path.Combine(destinationPath, Path.GetFileName(filePath)), true)

let remove filePath = 
    File.Delete(filePath)

let excludedExt = [| ".crdownloads" |]

let isExportFile (fileName:string) = fileName.Contains(".x.") && (not(fileName.EndsWith(".crdownload")))

let extractFileNameFrom filePath = Path.GetFileName(filePath)

let createFileHandler destinationPath =
    fun filePath -> 
        if isExportFile (extractFileNameFrom filePath)
        then
            printToConsole filePath
            copyToDestination filePath destinationPath
            remove filePath

let initializeWathcher watchPath fileHandler = 
    let listener = fun (x:FileSystemEventArgs) -> fileHandler (x.FullPath.ToString())
    let fsWatcher = new FileSystemWatcher(watchPath)
    fsWatcher.Created.Add(listener)
    fsWatcher.EnableRaisingEvents <- true 
    fsWatcher

let sourcePath argv = 
    if Array.length(argv) > 1 
    then argv.[1] 
    else "/home/andrei/Downloads"

let destinationPath argv = 
    if Array.length(argv) > 2 
    then argv.[2] 
    else "/home/andrei/temp/cloudXporter"


[<EntryPoint>]
[<PermissionSet(SecurityAction.Demand, Name="FullTrust")>]
let main argv = 
    let fileHandler = createFileHandler (destinationPath argv)
    let watcher = initializeWathcher (sourcePath argv) fileHandler

    let a = Console.ReadLine()
    0
