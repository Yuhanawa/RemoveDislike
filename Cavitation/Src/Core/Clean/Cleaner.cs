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
                CleanerGroup.Add(file.Name.Remove(file.Name.Length - 3),
                    new Model(from_file(file.FullName), file.FullName));
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

        public static void Run(string key) => CleanerGroup[key].Start();


        public class Model
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

            private static bool StrEq(string a, string b) =>
                a == b || string.Equals(a.Trim(), b.Trim(), StringComparison.InvariantCultureIgnoreCase);

            public void Start()
            {
                // Data clear
                // CleanedCount = 0;
                // CleanedSize = 0;

                foreach (Rule rule in Rules.Where(Check))
                {
                    if (File.Exists(rule.Path))
                    {
                        FileUtils.TryDelFile(rule.Path, true);
                        continue;
                    }

                    try
                    {
                        Run(rule);
                    }
                    catch (Exception e)
                    {
                        Log(@$"[File] {e}");
                    }
                }
            }

            private void Run(Rule rule)
            {
                var root = new DirectoryInfo(rule.Path);
                switch (rule.Mode)
                {
                    case ModeEnum.All:
                    {
                        TryDel(root);
                        break;
                    }
                    case ModeEnum.Folders:
                    {
                        if (rule.Feature[0] == "*" || StrEq(rule.Feature[0], "all"))
                        {
                            foreach (DirectoryInfo dir in root.GetDirectories())
                                TryDel(dir);
                            break;
                        }

                        foreach (DirectoryInfo dir in root.GetDirectories())
                            try
                            {
                                if (rule.Feature.Any(feature =>
                                    StrEq(feature, dir.Name)) || dir.GetFileSystemInfos().Length == 0)
                                    TryDel(dir);
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }

                        break;
                    }
                    case ModeEnum.Files:
                    {
                        if (rule.Feature[0] == "*")
                        {
                            foreach (FileInfo file in root.GetFiles())
                                TryDel(file);
                            break;
                        }

                        FeatureTryDelFile(root, rule);

                        break;
                    }
                    case ModeEnum.RecursionFiles:
                    {
                        void TryRecursion(DirectoryInfo dir)
                        {
                            try
                            {
                                FeatureTryDelFile(dir, rule);

                                foreach (DirectoryInfo sub in dir.GetDirectories())
                                    if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                                    else TryRecursion(sub);
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }
                        }

                        TryRecursion(root);

                        break;
                    }
                    case ModeEnum.RecursionFolders:
                    {
                        void TryRecursion(DirectoryInfo fileSystemInfo)
                        {
                            try
                            {
                                foreach (DirectoryInfo sub in fileSystemInfo.GetDirectories())
                                    try
                                    {
                                        if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                                        else if (rule.Feature.Any(feature => StrEq(sub.Name, feature))) TryDel(sub);
                                        else TryRecursion(sub);
                                    }
                                    catch (Exception e)
                                    {
                                        Log(@$"[File] {e.Message}");
                                    }
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }
                        }

                        TryRecursion(root);

                        break;
                    }
                    case ModeEnum.RecursionAll:
                    {
                        void TryRecursion(DirectoryInfo dir)
                        {
                            try
                            {
                                FeatureTryDelFile(dir, rule);

                                foreach (DirectoryInfo sub in dir.GetDirectories())
                                    try
                                    {
                                        if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                                        else if (rule.Feature.Any(feature => StrEq(sub.Name, feature))) TryDel(sub);
                                        else TryRecursion(sub);
                                    }
                                    catch (Exception e)
                                    {
                                        // ignored
                                    }
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }
                        }

                        TryRecursion(root);

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void UpData(double size) => UpData(size, 1);

            private void UpData(double size, int count)
            {
                CleanedCount += count;
                CleanedSize += size / 1024 / 1024;
            }

            private void TryDel(FileSystemInfo fileSystemInfo)
            {
                switch (fileSystemInfo)
                {
                    case DirectoryInfo dir:
                    {
                        UpData(FileUtils.TryExDelDir(dir, true).Size);
                        break;
                    }
                    case FileInfo file:
                    {
                        UpData(FileUtils.TryDelFile(file, true).Size);
                        break;
                    }
                }
            }

            private void FeatureTryDelFile(DirectoryInfo dir, Rule rule)
            {
                foreach (FileInfo file in dir.GetFiles())
                    if (rule.Feature.Any(feature => StrEq(file.Extension, feature)))
                        TryDel(file);
            }

            public void Delete() => Log(FileUtils.TryDelFile(new FileInfo(Source)).ToString());

            private static void Log(string m) => CommonUtils.Log(m);
        }
    }
}