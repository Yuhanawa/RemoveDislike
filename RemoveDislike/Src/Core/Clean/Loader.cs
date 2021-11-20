using System.Collections.Generic;
using System.IO;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Clean.Parser;
using static RemoveDislike.Core.Clean.Cleaner;


namespace RemoveDislike.Core.Clean
{
    /// <summary>
    ///     Loader
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
            foreach (FileInfo file in new DirectoryInfo(ConfigHelper.RuleBase).GetFiles("*.json"))
                CleanerGroup.Add(file.Name.Remove(file.Name.Length - 5),
                    new Model(FromFile(file.FullName), file.FullName));
        }

        public static void LoadInternalRules()
        {
            #region Temp(AllDrives)

            var rules = new List<Rule>();
            foreach (string drive in Directory.GetLogicalDrives())
            {
                rules.Add(new Rule(drive, CleanMode.RecursionFolders, new List<string>
                {
                    "Temp", "Tmp", ".temp",
                    "Log", "Logs", ".logs", ".log",
                    "$WinREAgent"
                }));
                rules.Add(new Rule(drive, CleanMode.RecursionFiles, new List<string>
                {
                    ".temp", ".tmp", ".log", ".logs",
                    ".cache", ".caches"
                }));
            }

            CleanerGroup.Add($"{I18NUtils.Get("Temp(AllDrives)")}", new Model(rules)
                { Administrator = true, CarpetScan = true });

            #endregion

            #region Temp(Safer)

            CleanerGroup.Add($"{I18NUtils.Get("Temp(Safer)")}", new Model(new List<Rule>
            {
                new(@$"{EnvironmentUtils.Get("Windir")}\Prefetch\"),
                new(@$"{EnvironmentUtils.Get("Windir")}\ServiceProfiles\LocalService\AppData\Local\FontCache\"),
                new(@$"{EnvironmentUtils.WinData}\Explorer\"),
                new(@$"{EnvironmentUtils.WinData}\Fonts\Deleted\"),
                new(@$"{EnvironmentUtils.WinData}\History\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\..\", CleanMode.RecursionFolders, new List<string>
                {
                    "Temp", "Tmp",
                    "Cache", "GrShaderCache", "ShaderCache", "CacheStorage", "Font Cache", "CryptnetUrlCache"
                })
            }) { Administrator = true, CarpetScan = true });

            #endregion
            
        }
    }
}