using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cavitation.Core.Utils;
using static Cavitation.Core.Clean.Rule.Parser;
using static Cavitation.Core.Clean.Rule;


namespace Cavitation.Core.Clean
{
    public class Cleaner
    {
        private static readonly Dictionary<string, Model> CleanerGroup = new();

        private bool _isInit;

        public Cleaner()
        {
            Load();
        }

        public void ReLoad()
        {
            CleanerGroup.Clear();
            _isInit = false;
            Load();
        }

        public void Load()
        {
            if (_isInit) return;

            AddInternalRules();
            AddExternalRules();

            _isInit = true;
        }

        private static void AddExternalRules()
        {
            foreach (FileInfo file in new DirectoryInfo(Config.RulesGroupsPath).GetFiles("*.cr"))
                CleanerGroup.Add(file.Name, new Model(from_file(file.FullName), file.FullName));
        }

        private static void AddInternalRules()
        {
            var rules = new List<Rule>();
            foreach (string drive in Directory.GetLogicalDrives())
            {
                rules.Add(new Rule(drive, ModeEnum.RecursionFolders, new List<string>
                {
                    "Temp", "Tmp", ".temp",
                    "Log", "Logs", ".logs", ".log",
                    "Cache", "Caches", "$WinREAgent"
                }));
                rules.Add(new Rule(@"C:\\", ModeEnum.RecursionFiles, new List<string>
                {
                    ".temp", ".tmp", ".log", ".logs",
                    ".cache", ".caches", ".old", ".bak", ".back"
                }));
            }

            CleanerGroup.Add("全盘缓存日志", new Model(rules));

            CleanerGroup.Add("固定缓存日志", new Model(new List<Rule>
            {
                new(@$"{EnvironmentUtils.WinData}\Explorer\"),
                new(@$"{EnvironmentUtils.WinData}\Fonts\Deleted\"),
                new(@$"{EnvironmentUtils.WinData}\History\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\..\..\", ModeEnum.Folders, new List<string>
                {
                    "Cache", "GrShaderCache", "ShaderCache", "CacheStorage", "Font Cache"
                })
            }));

            Dictionary<string, string> textRules = new()
            {
                { "test", @"" }
            };
            foreach (string key in textRules.Keys)
                CleanerGroup.Add(key, new Model(from_string(textRules[key])));
        }
        
        public static double AllCleanedSize => CleanerGroup.Keys.Sum(key => CleanerGroup[key].CleanedSize);
        public static double AllCleanedCount => CleanerGroup.Keys.Sum(key => CleanerGroup[key].CleanedCount);
        public static double GetCleanedSize(string key) => CleanerGroup[key].CleanedSize;
        public static double GetCleanedCount(string key) => CleanerGroup[key].CleanedCount;

        public static void Delete(string key) => CleanerGroup[key].Delete();
        public static Dictionary<string, Model>.KeyCollection Keys => CleanerGroup.Keys;
        public static Dictionary<string, Model>.ValueCollection Values => CleanerGroup.Values;
        public static int Count => CleanerGroup.Count;
        public static string GetSource(string key) => CleanerGroup[key].Source;

        public static void RunAll()
        {
            foreach (string key in CleanerGroup.Keys) Run(key);
        }

        public static void Run(string key) => CleanerGroup[key].Run();


        private class Model
        {
            public Model(List<Rule> rules, string source)
            {
                Rules = rules;
                Source = source;
            }

            public Model(List<Rule> rules)
            {
                Rules = rules;
                Source = "Internal";
            }

            public List<Rule> Rules { get; protected set; }
            public double CleanedSize { get; protected set; }
            public int CleanedCount { get; protected set; }
            public string Source { get; protected set; }
            public int RulesCount => Rules.Count;

            public override string ToString() => $"[RuleGroup] Source: {Source} \nRules: {Rules}";

            private static bool Check(Rule rule)
            {
                if (Directory.Exists(rule.Path))
                {
                    // ReSharper disable once InvertIf
                    if (new DirectoryInfo(rule.Path).GetFileSystemInfos().Length == 0)
                    {
                        Directory.Delete(rule.Path);
                        return false;
                    }
                }
                else if (File.Exists(rule.Path))
                {
                    if ((File.GetAttributes(rule.Path) & FileAttributes.System) != 0 ||
                        (File.GetAttributes(rule.Path) & FileAttributes.ReadOnly) != 0)
                        return false;
                }
                else
                {
                    return false;
                }

                return true;
            }

            public void Run()
            {
                // Data clear
                // CleanedCount = 0;
                // CleanedSize = 0;

                foreach (Rule rule in Rules.Where(Check))
                {
                    if (!Directory.Exists(rule.Path))
                    {
                        TryDel(rule.Path, false);
                        continue;
                    }

                    try
                    {
                        var root = new DirectoryInfo(rule.Path);
                        switch (rule.Mode)
                        {
                            case ModeEnum.All:
                            {
                                TryDel(root, true);
                                break;
                            }
                            case ModeEnum.Folders:
                            {
                                if (rule.Feature[0] == "*")
                                    foreach (DirectoryInfo dir in root.GetDirectories())
                                        TryDel(dir, true);
                                else
                                    foreach (DirectoryInfo dir in root.GetDirectories())
                                        try
                                        {
                                            if (rule.Feature.Any(feature =>
                                                string.Equals(dir.Name, feature,
                                                    StringComparison.CurrentCultureIgnoreCase) ||
                                                dir.GetFileSystemInfos().Length == 0))
                                                TryDel(dir, true);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }

                                break;
                            }
                            case ModeEnum.Files:
                            {
                                if (rule.Feature[0] == "*")
                                    foreach (FileInfo file in root.GetFiles())
                                        TryDel(file);
                                else
                                    foreach (FileInfo file in root.GetFiles())
                                        try
                                        {
                                            if (rule.Feature.Any(feature => string.Equals(file.Extension, feature,
                                                StringComparison.CurrentCultureIgnoreCase)))
                                                TryDel(file);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }

                                break;
                            }
                            case ModeEnum.RecursionFiles:
                            {
                                bool universal = rule.Feature[0] == "*";

                                void TryRecursion(DirectoryInfo fileSystemInfo)
                                {
                                    try
                                    {
                                        Recursion(fileSystemInfo);
                                    }
                                    catch (Exception e)
                                    {
                                        Log(@$"[File] {e.Message}");
                                    }
                                }

                                void Recursion(DirectoryInfo fileSystemInfo)
                                {
                                    foreach (FileInfo file in fileSystemInfo.GetFiles())
                                        try
                                        {
                                            if (rule.Feature.Any(feature =>
                                                string.Equals(file.Extension, feature,
                                                    StringComparison.CurrentCultureIgnoreCase)) || universal)
                                                TryDel(file);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }

                                    foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                        try
                                        {
                                            if (dir.GetFileSystemInfos().Length == 0)
                                            {
                                                TryDel(dir);
                                            }
                                            else
                                            {
                                                TryRecursion(dir);
                                                if (dir.GetFileSystemInfos().Length == 0) TryDel(dir);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }
                                }

                                TryRecursion(root);

                                break;
                            }
                            case ModeEnum.RecursionFolders:
                            {
                                if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                                void TryRecursion(DirectoryInfo fileSystemInfo)
                                {
                                    try
                                    {
                                        Recursion(fileSystemInfo);
                                    }
                                    catch (Exception e)
                                    {
                                        Log(@$"[File] {e.Message}");
                                    }
                                }

                                void Recursion(DirectoryInfo fileSystemInfo)
                                {
                                    foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                        try
                                        {
                                            if (rule.Feature.Any(feature =>
                                                string.Equals(dir.Name, feature,
                                                    StringComparison.CurrentCultureIgnoreCase) ||
                                                dir.GetFileSystemInfos().Length == 0))
                                                TryDel(dir, dir.GetFileSystemInfos().Length != 0);
                                            else TryRecursion(dir);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }
                                }

                                TryRecursion(root);

                                break;
                            }
                            case ModeEnum.RecursionAll:
                            {
                                if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                                void TryRecursion(DirectoryInfo fileSystemInfo)
                                {
                                    try
                                    {
                                        Recursion(fileSystemInfo);
                                    }
                                    catch (Exception e)
                                    {
                                        Log(@$"[File] {e.Message}");
                                    }
                                }

                                void Recursion(DirectoryInfo fileSystemInfo)
                                {
                                    foreach (FileInfo file in fileSystemInfo.GetFiles())
                                        try
                                        {
                                            if (rule.Feature.Any(feature =>
                                                string.Equals(file.Extension, feature,
                                                    StringComparison.CurrentCultureIgnoreCase)))
                                                TryDel(file);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }

                                    foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                        try
                                        {
                                            if (rule.Feature.Any(feature =>
                                                string.Equals(dir.Name, feature,
                                                    StringComparison.CurrentCultureIgnoreCase) ||
                                                dir.GetFileSystemInfos().Length == 0))
                                                TryDel(dir, dir.GetFileSystemInfos().Length != 0);
                                            else TryRecursion(dir);
                                        }
                                        catch (Exception e)
                                        {
                                            Log(@$"[File] {e.Message}");
                                        }
                                }

                                TryRecursion(root);

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    catch (Exception e)
                    {
                        Log(@$"[File] {e}");
                    }
                }
            }

            private void AddCleanupData(double size, int count)
            {
                CleanedCount += count;
                CleanedSize += size / 1024 / 1024;
            }

            private void TryDel(string path, bool isDir)
            {
                if (isDir)
                    TryDel(new DirectoryInfo(path));
                else
                    TryDel(new FileInfo(path));
            }

            private void TryDel(FileSystemInfo dir) => TryDel(dir, false);

            private void TryDel(FileSystemInfo fileSystemInfo, bool recursive)
            {
                try
                {
                    switch (fileSystemInfo)
                    {
                        case DirectoryInfo dir:
                        {
                            if (recursive)
                                foreach (FileSystemInfo info in dir.GetFileSystemInfos())
                                    TryDel(info, true);

                            try
                            {
                                dir.Delete();
                                AddCleanupData(0.0, 1);
                                Log($"[File] Successfully Deleted Folder : {dir.FullName}");
                            }
                            catch (Exception e)
                            {
                                Log(@$"[File] {e.Message.Trim()} Path: {dir.FullName}");
                            }

                            break;
                        }
                        case FileInfo file:
                        {
                            try
                            {
                                file.Delete();
                                AddCleanupData(file.Length, 1);
                                Log($"[File] Successfully Deleted File : {file.FullName}");
                            }
                            catch (Exception e)
                            {
                                Log(@$"[File] {e.Message} Path: {file.FullName}");
                            }

                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log(@$"[File] {e.Message.Trim()} Path: {fileSystemInfo.FullName}");
                }
            }

            public void Delete() => TryDel(Source, false);

            private static void Log(string m) => CommonUtils.Log(m);
        }
    }
}