using System.Collections.Generic;
using System.IO;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Clean.Parser;
using static RemoveDislike.Core.Clean.Cleaner;


namespace RemoveDislike.Core.Clean
{
    /// <summary>
    /// Loader
    /// </summary>
    public static class Loader
    {
        private static bool _isInit;

        public static void LoadAll()
        {
            if (_isInit) return;

            LoadBlackList();
            LoadInternalRules();
            LoadExternalRules();

            _isInit = true;
        }

        public static void ReLoadAll()
        {
            _isInit = false;
            BlackList.Clear();
            CleanerGroup.Clear();
            LoadAll();
        }

        private static void LoadBlackList()
        {
            BlackList.Add(EnvironmentUtils.Get("ProgramFiles"));
            BlackList.Add(EnvironmentUtils.Get("ProgramFiles(x86)"));
            BlackList.Add(EnvironmentUtils.Get("ProgramW6432"));
            BlackList.Add(EnvironmentUtils.Get("SystemRoot"));
            BlackList.Add(EnvironmentUtils.Get("SystemDrive"));
            BlackList.Add(EnvironmentUtils.Get("Windir"));
            BlackList.Add(@$"{EnvironmentUtils.Get("AppData")}\..\Local\Packages");
            BlackList.Add(@$"{EnvironmentUtils.Get("AppData")}\..\Local\Programs");
        }

        private static void LoadExternalRules()
        {
            foreach (FileInfo file in new DirectoryInfo(ConfigHelper.RulesGroupsPath).GetFiles("*.cr"))
                CleanerGroup.Add(file.Name.Remove(file.Name.Length - 3),
                    new Model(from_file(file.FullName), file.FullName));
        }

        private static void LoadInternalRules()
        {
            var rules = new List<Rule>();
            foreach (string drive in Directory.GetLogicalDrives())
            {
                rules.Add(new Rule(drive, CleanMode.RecursionFolders, new List<string>
                {
                    "Temp", "Tmp", ".temp",
                    "Log", "Logs", ".logs", ".log",
                    // "Cache", "Caches",
                    "$WinREAgent"
                }));
                rules.Add(new Rule(drive, CleanMode.RecursionFiles, new List<string>
                {
                    ".temp", ".tmp", ".log", ".logs",
                    ".cache", ".caches", ".old", ".bak", ".back"
                }));
            }

            CleanerGroup.Add("全盘缓存日志", new Model(rules));

            CleanerGroup.Add("固定缓存日志", new Model(new List<Rule>
            {
                new(@"D:\Users\yuhan\Desktop\1.txt"),
                new(@$"{EnvironmentUtils.Get("Windir")}\SoftwareDistribution\"),
                new(@$"{EnvironmentUtils.Get("Windir")}\Prefetch\"),
                new(@$"{EnvironmentUtils.Get("Windir")}\ServiceProfiles\LocalService\AppData\Local\FontCache\"),
                new(@$"{EnvironmentUtils.WinData}\Explorer\"),
                new(@$"{EnvironmentUtils.WinData}\Fonts\Deleted\"),
                new(@$"{EnvironmentUtils.WinData}\History\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\..\", CleanMode.Folders, new List<string>
                {
                    "Temp", "Tmp",
                    "Cache", "GrShaderCache", "ShaderCache", "CacheStorage", "Font Cache", "CryptnetUrlCache"
                })
            }) { Administrator = true, CarpetScan = true });

            Dictionary<string, string> textRules = new()
            {
                { "test", @"" }
            };
            foreach (string key in textRules.Keys)
                CleanerGroup.Add(key, new Model(from_string(textRules[key])));
        }
    }
}