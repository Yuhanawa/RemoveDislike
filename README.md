# RemoveDislike

Remove Your Dislike Things

---

Under development, slow development

Basic functionality is about 15% complete

## External Rules

The Rules folder directory :  %AppData%\RemoveDislike\Rules\

### Options:

```json
{
    "header": {
        "name": "Examples",
        "author": "me",
        "description": "This is an examples",
        "force": false
    },
    "rules": {
//      "name:description": {
//        "*.*": [
//          "Here is path"
//        ]
//      },
        "log:log file": {
            "*.{log,logs}": [
                "%APPDATA%/../../../"
            ]
        },
        "tmp:temp file": {
            "*": [
                "%TMP%","$WinREAgent",
				"%WINDIR%/ServiceProfiles/LocalService/AppData/Local/FontCache/",
				"%WINDIR%/Explorer/",
				"%WINDIR%/Prefetch/",
				"%WINDIR%/Fonts/Deleted/",
				"%WINDIR%/ActionCenterCache/"				
            ],
            "*.{tmp,temp}": [
                "%APPDATA%/../../../"
            ]
        }
    }
}

```

## Old (will be removed)

```json5
"ForceDelete": bool,	//Default: false
"Administrator": bool,	// Default: false
"CarpetScan": bool,	// Default: false

"CleanMode":"All",		// Default: "All"
            "Files",		// Delete only eligible Files in this folder, excluding subfolders
            "Folders",		// Delete only eligible Folders in this folder, excluding subfolders
            "RecursionAll",	// Delete eligible files in this folder, including subfolders
            "RecursionFiles",	// Delete eligible Folders in this folder, including subfolders
            "RecursionFolders"	// Delete everything in this folder that matches the criteria, including subfolders
```

### Examples:

```json5
{
    "ForceDelete": false,	// Default: false
    "Administrator": true,	// Default: false
    "CarpetScan": true,		// Default: false
    "Rules": [
        {	// Delete all the folders named "Temp" and "Tmp" in "C:\\"
            "Feature": [
                "Temp",  // not case sensitive
                "Tmp"
            ],
            "CleanMode": "RecursionFolders",
            "Path": "C:\\"
        },
        {	// Delete all files with the suffix ".temp" ".log" ".cache" in "D:\\"
            "Feature": [
                ".temp",  // not case sensitive
                ".log",
                ".cache"
            ],
            "CleanMode": "RecursionFiles",
            "Path": "D:\\"
        }
    ]
}
```

```json5
{
    "ForceDelete": false,
    "Administrator": true,
    "CarpetScan": false,
    "Rules": [
        {
            "CleanMode": "All",	// Default is "All", So it can Ignore
            "Path": "C:\\WINDOWS\\Prefetch\\"
        },
        {
            "Path": "%Temp%"	//Support Environment variables
        }
    ]
}
```

