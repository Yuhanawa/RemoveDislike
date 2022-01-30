# RemoveDislike

Remove Your Dislike Things

---

Under development, slow development

Basic functionality is about 15% complete

## External Rules

The Rules folder directory :  %AppData%\RemoveDislike\Rules\

```json

{
    "header": {
        "name": "CleanTempFiles",
        "author": "Yuhanawa",
        "description": "Clean cache and temporary files of system and software",
        "force": false
    },
    "rules": {
        "Logs:Windows Logs and User Log": {
            "*": [
                "%WINDIR%/Logs/",
                "%WINDIR%/SoftwareDistribution/DataStore/Logs/",
                "C:/ProgramData/Microsoft/Search/Data/Applications/Windows/GatherLogs/",
                "%WINDIR%/System32/LogFiles/"
            ],
            "/**/*.{log,logs,Log,Logs,LOG,LOGS}": [
                "%WINDIR%/",
                "%APPDATA%/../Local/Microsoft/Windows/WebCache/",
                "%APPDATA%/../../"
            ],
            "/**/{log,logs,Log,Logs,LOG,LOGS}": [
                "%APPDATA%/../../"
            ],
            "/**/{log,logs,Log,Logs,LOG,LOGS}.{txt}": [
                "%APPDATA%/../../"
            ],
            "/**/{log,logs,Log,Logs,LOG,LOGS}/**": [
                "%APPDATA%/../../"
            ]
        },
        "Caches:Windows Cache and User Cache(or Temp)": {
            "*": [
                "%WINDIR%/Explorer/",
                "%WINDIR%/Temp/",
                "%APPDATA%/Microsoft/Windows/Recent",
                "C:/$WinREAgent/",
                "%WINDIR%/ServiceProfiles/LocalService/AppData/Local/FontCache/",
                "%APPDATA%/../LocalLow/Microsoft/CryptnetUrlCache/Content/",
                "%WINDIR%/SoftwareDistribution/Download/SharedFileCache/",
                "%WINDIR%/Prefetch/",
                "%WINDIR%/Fonts/Deleted/",
                "%WINDIR%/ActionCenterCache/",
                "%APPDATA%/../Local/CrashDumps/"
            ],
            "/**/*.{tmp,temp,Tmp,Temp}": [
                "%APPDATA%/../../"
            ],
            "/**/{tmp,temp,Tmp,Temp}/**": [
                "%APPDATA%/../../",
                "%WINDIR%/"
            ],
            "/**/*.{cache,Cache}": [
                "%APPDATA%/../../"
            ],
            "/**/{cache,Cache,GPUCache,Code Cache}/**": [
                "%APPDATA%/../../"
            ]
        }
    }
}

```


#### OLD
```json5
{
    "header": {
        "name": "Examples",
        "author": "me",
        "description": "This is an examples",
        "force": false
    },
    "rules": {
//      "name:description": {
//        "Ant-Style: [
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
                "%TMP%","C:$WinREAgent",
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