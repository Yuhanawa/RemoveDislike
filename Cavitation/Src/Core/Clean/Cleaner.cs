using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // { "test", @"" },
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
            CleanerGroup = new Dictionary<string, Model>
            { { "C盘缓存日志", new Model(new List<Rule>
            {
                // new(@"C:\\",ModeEnum.RecursionFolders,new List<string>()
                // {
                //     "Temp","Tmp","Log","Logs",".logs",".log",".temp","Cache","Caches","$WinREAgent",
                // }),
                new(@"C:\\",ModeEnum.RecursionFiles,new List<string>
                {
                    ".temp",".tmp",".log",".logs",".cache",".caches",".old",".bak",".back"
                }),
                new(@$"{EnvironmentUtils.WinData}\Explorer\"),
                new(@$"{EnvironmentUtils.WinData}\Fonts\Deleted\"),
                new(@$"{EnvironmentUtils.WinData}\History\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\ActionCenterCache\"),
                new(@$"{EnvironmentUtils.WinData}\..\..\",ModeEnum.Folders,new List<string>
                {
                    "Cache","GrShaderCache","ShaderCache","CacheStorage","Font Cache",
                }),
            },"Internal") } };

            foreach (string key in Internal.Keys)
                CleanerGroup.Add(key, new Model(from_string(Internal[key]), "Internal"));

            foreach (FileInfo file in new DirectoryInfo(Config.RulesGroupsPath).GetFiles("*.cr"))
                CleanerGroup.Add(file.Name, new Model(from_file(file.FullName), file.FullName));

            _isInit = true;
        }
        
        public static double AllCleanupSize => CleanerGroup.Keys.Sum(key => CleanerGroup[key].CleanupSize);


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
                else return false;
                
                return true;
            }

            public void Run()
            {
                // Data clear
                // CleanupCount = 0;
                // CleanupSize = 0;

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
                                    {
                                        try
                                        { 
                                            if (dir.GetFileSystemInfos().Length == 0) TryDel(dir);
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

            public void SelfDestruct() => TryDel(Source, false);

            private static void Log(string m) => CommonUtils.Log(m);
        }
    }
}