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
        private static readonly Dictionary<string, string> Internal = new()
        {
            { "test0", @"" },
            { "test1", @"" },
            { "test2", @"" },
            { "test3", @"" },
            { "test4", @"" },
            { "test5", @"" },
            { "test6", @"" }
        };

        private bool _isInit;

        public Cleaner()
        {
            Load();
        }

        public static Dictionary<string, Model> CleanerGroup { get; private set; }

        public static void Run()
        {
            foreach (string key in CleanerGroup.Keys) CleanerGroup[key].Run();
        }

        public void ReLoad()
        {
            CleanerGroup = null;
            _isInit = false;
            Load();
        }

        public void Load()
        {
            if (_isInit) return;
            CleanerGroup = new Dictionary<string, Model>();
            foreach (string key in Internal.Keys)
                CleanerGroup.Add(key, new Model(from_string(Internal[key]), "Internal"));

            foreach (FileInfo file in new DirectoryInfo(Config.RulesGroupsPath).GetFiles("*.cr"))
                CleanerGroup.Add(file.Name, new Model(from_file(file.FullName), file.FullName));

            _isInit = true;
        }


        public class Model
        {
            public Model(List<Rule> rules, string source)
            {
                Rules = rules;
                Source = source;
            }

            public List<Rule> Rules { get; protected set; }
            public double CleanupSize { get; protected set; }
            public int CleanupCount { get; protected set; }
            public string Source { get; protected set; }
            public int RulesCount => Rules.Count;

            public override string ToString() => $"[RuleGroup] Source: {Source} \nRules: {Rules}";

            private static bool Check(Rule rule)
            {
                if (!File.Exists(rule.Path) && !Directory.Exists(rule.Path)) return false;

                if (new DirectoryInfo(rule.Path).GetFileSystemInfos().Length == 0)
                {
                    Directory.Delete(rule.Path);
                    return false;
                }

                if ((File.GetAttributes(rule.Path) & FileAttributes.System) != 0 ||
                    (File.GetAttributes(rule.Path) & FileAttributes.ReadOnly) != 0)
                    return false;

                return true;
            }

            public void Run()
            {
                CleanupCount = 0;
                CleanupSize = 0;

                foreach (Rule rule in Rules.Where(Check))
                {
                    if (!Directory.Exists(rule.Path))
                    {
                        TryDel(rule.Path, false);
                        continue;
                    }

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
                                    if (rule.Feature.Any(feature =>
                                        dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                        TryDel(dir, true);
                            break;
                        }
                        case ModeEnum.FilesOnDir:
                        {
                            if (rule.Feature[0] == "*")
                                foreach (FileInfo file in root.GetFiles())
                                    TryDel(file);
                            else
                                foreach (FileInfo file in root.GetFiles())
                                    if (rule.Feature.Any(feature => file.Extension == feature))
                                        TryDel(file);
                            break;
                        }
                        case ModeEnum.RecursionAllFiles:
                        {
                            bool universal = rule.Feature[0] == "*";

                            void Recursion(DirectoryInfo fileSystemInfo)
                            {
                                foreach (FileInfo file in fileSystemInfo.GetFiles())
                                    if (rule.Feature.Any(feature => file.Extension == feature) || universal)
                                        TryDel(file);
                                foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                    if (dir.GetFileSystemInfos().Length == 0) TryDel(dir);
                                    else Recursion(dir);
                            }

                            Recursion(root);

                            break;
                        }
                        case ModeEnum.DirsAndFiles:
                        {
                            if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                            void Recursion(DirectoryInfo fileSystemInfo)
                            {
                                foreach (FileInfo file in fileSystemInfo.GetFiles())
                                    if (rule.Feature.Any(feature => file.Extension == feature))
                                        TryDel(file);
                                foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                    if (rule.Feature.Any(feature =>
                                        dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                        TryDel(dir, dir.GetFileSystemInfos().Length != 0);
                                    else
                                        Recursion(dir);
                            }

                            Recursion(root);

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private void AddCleanupData(double size, int count)
            {
                CleanupCount += count;
                CleanupSize += size/1024/1024;
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
                            Log(@$"[File] {e.Message} Path: {dir.FullName}");
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

            public void SelfDestruct() => TryDel(Source, false);

            private static void Log(string m) => CommonUtils.Log(m);
        }
    }
}