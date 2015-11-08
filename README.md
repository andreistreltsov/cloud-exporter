# What is this

CloudExporter is a simple tool that helps with saving files generated by cloud-based apps locally.

Many document/drawing apps have the ability to export (download) files to the local filesystem.
When a file is being exported, it's typically saved to the Downloads directory.

Then the file needs to be manually moved to a project's working directory or synced location.
Having to frequently update and save multiple documents related to different projects, this quickly gets old.

The program monitors Downloads (or any other specified directory) and automatically moves matching files to an appropriate location as defined in the configuraiton.

# File routing 

The configuration allows to specify a source directory and a set of 'routes'.

Routes are defined by regex patterns.

Each incoming filename is matched against the routes after which the file gets moved to zero or more corresponding destination directories.

# How to use

1. Build the solution
2. Create/edit a configuration file; Use `config.json` as an example
3. Run:  `$./CloudExporter.exe config.json`

