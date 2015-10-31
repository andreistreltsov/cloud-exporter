open System
open System.Security.Permissions
open System.IO

let copyfile fullSourcePath fullDestination = 0
let copyTestFileToTestDir = copyfile "" ""


let initializeWathcherAt watchPath = 
    let handleCreation = fun (x:FileSystemEventArgs) -> printfn "%s" (x.FullPath.ToString())
    let fsWatcher = new FileSystemWatcher(watchPath)
    fsWatcher.Created.Add(handleCreation)
    fsWatcher.EnableRaisingEvents <- true 
    fsWatcher

let specifiedOrDownloadsPath argv = 
    if Array.length(argv) > 1 
    then argv.[1] 
    else "/home/andrei/Downloads"


[<EntryPoint>]
[<PermissionSet(SecurityAction.Demand, Name="FullTrust")>]
let main argv = 
    let watcher = initializeWathcherAt (specifiedOrDownloadsPath argv)

    let a = Console.ReadLine()
    0 // return an integer exit code

