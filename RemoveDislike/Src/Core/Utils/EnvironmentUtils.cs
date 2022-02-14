using System.Collections.Generic;

namespace RemoveDislike.Core.Utils;

public static class EnvironmentUtils
{
    private static readonly Dictionary<string, string> VariableDictionary = new()
    {
        { "SystemRoot", Environment.GetEnvironmentVariable("SystemRoot") },
        { "Windir", Environment.GetEnvironmentVariable("windir") },
        { "HomeDrive", Environment.GetEnvironmentVariable("HomeDrive") },
        { "SystemDrive", Environment.GetEnvironmentVariable("SystemDrive") },
        { "ProgramFiles", Environment.GetEnvironmentVariable("ProgramFiles") },
        { "ProgramFiles(x86)", Environment.GetEnvironmentVariable("ProgramFiles(x86)") },
        { "CommonProgramFiles", Environment.GetEnvironmentVariable("CommonProgramFiles") },
        { "UserProFile", Environment.GetEnvironmentVariable("UserProFile") },
        { "HomePath", Environment.GetEnvironmentVariable("HomePath") },
        { "Appdata", Environment.GetEnvironmentVariable("Appdata") },
        { "Temp", Environment.GetEnvironmentVariable("Temp") },
        { "WinData", @$"{Environment.GetEnvironmentVariable("Appdata")}\..\Local\Microsoft\Windows" }
    };

    public static string Get(string name)
    {
        if (name.StartsWith("%") && name.EndsWith("%")) name = name.TrimStart('%').TrimEnd('%');
        if (!VariableDictionary.ContainsKey(name))
            VariableDictionary.Add(name, Environment.GetEnvironmentVariable(name));

        return VariableDictionary[name];
    }
}