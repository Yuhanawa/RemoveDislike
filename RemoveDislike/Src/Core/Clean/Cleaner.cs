using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Utils.LogUtils;

namespace RemoveDislike.Core.Clean
{
    /// <summary>
    ///     Cleaner
    /// </summary>
    public class Cleaner
    {
        public Cleaner()
        {
            Load();
        }

        public static Dictionary<string, Model> CleanerGroup { get; } = new();
        public static List<string> BlackList { get; } = new();
        public static string GetSource(string key) => CleanerGroup[key].Source;


        /// <summary>
        ///     Cleaner Model
        /// </summary>
        public class Model
        {
            public List<Rule> Rules { get; set; }

            /// <summary>
            ///     [RuleGroup] Source: {Source} \nRules: {Rules}
            /// </summary>
            /// <returns> [RuleGroup] Source: {Source} \nRules: {Rules} </returns>
            public override string ToString() => $"[RuleGroup] Source: {Source} \nRules: {Rules}";

            private bool RulesCheck()
            {
                if (Rules.Count == 0)
                {
                    Warn($"{Rules} has no rules");
                    return false;
                }

                // Check the safety level
                // ReSharper disable once InvertIf
                if (SafetyLevel > ConfigHelper.Config.SafetyLevel)
                {
                    Info(@$"[File] {Source} is too dangerous({SafetyLevel}), skip it.");
                    return false;
                }

                Info(@$"[File] {Source} is safe({SafetyLevel}), start to clean.");
                return true;
            }

            private bool DirectoryCheck(string path)
            {
                if (Directory.Exists(path))
                {
                    Warn($"[File] The directory {path} is not found");
                    return false;
                }

                if (Directory.GetFileSystemEntries(path).Length == 0)
                {
                    Directory.Delete(path);
                    return false;
                }

                // ReSharper disable once InvertIf
                if (BlackList.Any(any => any == path))
                {
                    Warn($"[File] The directory {path} is in BlackList");
                    return false;
                }

                // Check whether the current user has operation permissions for this folder                    
                return Administrator || FileUtils.HasOperationPermission(path, true);
            }

            private bool FileCheck(string path)
            {
                if ((File.GetAttributes(path) & FileAttributes.System) != 0 ||
                    (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0)
                    return false;

                // Check whether the current user has operation permissions for this file
                return Administrator || FileUtils.HasOperationPermission(path, false);
            }

            private bool PathCheck(string path)
            {
                if (MandatoryDir)
                    return DirectoryCheck(path);

                if (File.Exists(path))
                    return FileCheck(path);

                if (Directory.Exists(path))
                    return DirectoryCheck(path);

                Warn($"[File] {path} The Path is not file or folder ,  Maybe the is not Exists");
                return false;
            }

            /// <summary>
            ///     Start
            /// </summary>
            public void Start()
            {
                if (!RulesCheck()) return;

                foreach (Rule rule in Rules.Where(rule => PathCheck(rule.Path)))
                {
                    // if the path is a file, delete it
                    if (File.Exists(rule.Path))
                    {
                        UpdateData(FileUtils.TryDelFile(rule.Path, true).Size);
                        continue;
                    }

                    try
                    {
                        Run(rule);
                    }
                    catch (Exception e)
                    {
                        Log(@$"[File] {rule} {e}");
                    }
                }
            }

            private void Run(Rule rule)
            {
                var root = new DirectoryInfo(rule.Path);
                switch (rule.CleanMode)
                {
                    case CleanMode.All:
                    {
                        TryDel(root);
                        break;
                    }
                    case CleanMode.Folders:
                    {
                        if (_UniversalCharacterCleanModule(root, rule, true)) return;


                        foreach (DirectoryInfo dir in root.GetDirectories()
                            .Where(dir => DirectoryCheck(dir.FullName)))
                            if (rule.Feature.Any(feature =>
                                StrEq(feature, dir.Name)) || dir.GetFileSystemInfos().Length == 0)
                                TryDel(dir);

                        break;
                    }
                    case CleanMode.Files:
                    {
                        if (_UniversalCharacterCleanModule(root, rule, false)) return;

                        _FileCleanupModule(root, rule);

                        break;
                    }
                    case CleanMode.RecursionFiles:
                    {
                        void Recursion(DirectoryInfo dir)
                        {
                            _FileCleanupModule(dir, rule);
                            _RecursionFileModule(dir, Recursion);
                        }

                        Recursion(root);

                        break;
                    }
                    case CleanMode.RecursionFolders:
                    {
                        void Recursion(DirectoryInfo dir) => _DirCleanupModule(root, rule, Recursion);
                        Recursion(root);

                        break;
                    }
                    case CleanMode.RecursionAll:
                    {
                        void Recursion(DirectoryInfo dir)
                        {
                            _FileCleanupModule(dir, rule);
                            _DirCleanupModule(dir, rule, Recursion);
                        }

                        Recursion(root);

                        break;
                    }
                    default:
                        Fatal($"Non-existent (Non-possible) rule.CleanMode: {rule.CleanMode} , " +
                              "But the program will not exit due to this", null);
                        break;
                }
            }

            private void TryDel(FileSystemInfo fileSystemInfo)
            {
                switch (fileSystemInfo)
                {
                    case DirectoryInfo dir:
                    {
                        UpdateData(FileUtils.TryExDelDir(dir).Size);
                        break;
                    }
                    case FileInfo file:
                    {
                        UpdateData(FileUtils.TryDelFile(file, true).Size);
                        break;
                    }
                }
            }

            private void _FileCleanupModule(DirectoryInfo dir, Rule rule)
            {
                try
                {
                    foreach (FileInfo file in dir.GetFiles().Where(file => PathCheck(file.FullName)))
                        try
                        {
                            if (rule.Feature.Any(feature => StrEq(file.Extension, feature)))
                                TryDel(file);
                        }
                        catch (Exception e)
                        {
                            Err(@$"[File] [In_FileCleanupModule] dir: {dir.FullName} rule: {rule} {e.Message} ", e);
                        }
                }
                catch (Exception e)
                {
                    Err(@$"[File] [On_FileCleanupModule] dir: {dir.FullName} rule: {rule} {e.Message} ", e);
                }
            }

            private void _DirCleanupModule(DirectoryInfo dir, Rule rule, Recursion recursion)
            {
                try
                {
                    foreach (DirectoryInfo sub in dir.GetDirectories()
                        .Where(subDir => DirectoryCheck(subDir.FullName)))
                        try
                        {
                            if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                            else if (rule.Feature.Any(feature => StrEq(sub.Name, feature))) TryDel(sub);
                            else recursion(sub);
                        }
                        catch (Exception e)
                        {
                            Err(@$"[File] [In_DirCleanupModule] dir: {dir.FullName} rule: {rule} {e.Message} ", e);
                        }
                }
                catch (Exception e)
                {
                    Err(@$"[File] [On_DirCleanupModule] dir: {dir.FullName} rule: {rule} {e.Message} ", e);
                }
            }

            private void _RecursionFileModule(DirectoryInfo dir, Recursion recursion)
            {
                try
                {
                    foreach (DirectoryInfo sub in dir.GetDirectories().Where(subDir => PathCheck(subDir.FullName)))
                        try
                        {
                            if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                            else recursion(sub);
                        }
                        catch (Exception e)
                        {
                            Warn(@$"[File] [In_RecursionFileModule] [InForeach] {e.Message}");
                        }
                }
                catch (Exception e)
                {
                    Warn(@$"[File] [On_RecursionFileModule] [OnForeach] {e.Message}");
                }
            }

            private static bool _UniversalCharacterRecognition(string s) => s == "*" || StrEq(s, "all");

            private bool _UniversalCharacterCleanModule(DirectoryInfo root, Rule rule, bool isDir)
            {
                switch (isDir)
                {
                    case true when _UniversalCharacterRecognition(rule.Feature[0]):
                    {
                        foreach (DirectoryInfo dir in root.GetDirectories()
                            .Where(dir => DirectoryCheck(dir.FullName))) TryDel(dir);
                        return true;
                    }
                    case false when _UniversalCharacterRecognition(rule.Feature[0]):
                    {
                        foreach (FileInfo file in root.GetFiles()
                            .Where(dir => FileCheck(dir.FullName))) TryDel(file);
                        return true;
                    }
                    default:
                        return false;
                }
            }

            public void ToDelete() => FileUtils.TryDelFile(Source, true);

            private delegate void Recursion(DirectoryInfo dir);

            #region UpdateData

            private void UpdateData(double size) => UpdateData(size, 1);

            private void UpdateData(double size, int count)
            {
                if (size > 0)
                    CleanedSize += size / 1024;
                if (size >= 0)
                    CleanedCount += count;
            }

            #endregion

            #region Fields

            public string Source { get; set; }
            public bool MandatoryDir { get; set; }
            public bool ForceDelete { get; set; }
            public bool Administrator { get; set; }
            public bool CarpetScan { get; set; }

            /// <summary>
            ///     0: Safety
            ///     1: Normal
            ///     2: Danger
            ///     3: Very Danger
            ///     4: Extremely Danger
            ///     5: Risk of death
            ///     6: Refuse to execute
            /// </summary>
            public int SafetyLevel =>
                Administrator switch
                {
                    false when !ForceDelete && !CarpetScan => 0,
                    true when !ForceDelete && !CarpetScan => 1,
                    _ => ForceDelete switch
                    {
                        true when !CarpetScan => 2,
                        _ => CarpetScan switch
                        {
                            true when !Administrator && !ForceDelete => 3,
                            true when Administrator && !ForceDelete => 4,
                            true when ForceDelete => 5,
                            _ => 6
                        }
                    }
                };

            public double CleanedSize { get; private set; }
            public int CleanedCount { get; private set; }
            public int RulesCount => Rules.Count;

            #endregion

            #region Constructor

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

            #endregion

            #region Utils

            private static bool StrEq(string a, string b) =>
                a == b || string.Equals(a.Trim(), b.Trim(), StringComparison.InvariantCultureIgnoreCase);

            private static void Log(string m) => LogUtils.Log(m);

            #endregion
        }

        #region CleanerGroup Keys Values Count

        public static Dictionary<string, Model>.KeyCollection Keys => CleanerGroup.Keys;
        public static Dictionary<string, Model>.ValueCollection Values => CleanerGroup.Values;
        public static int Count => CleanerGroup.Count;

        #endregion

        #region CleanedData

        public static double AllCleanedSize => CleanerGroup.Keys.Sum(key => CleanerGroup[key].CleanedSize);
        public static double AllCleanedCount => CleanerGroup.Keys.Sum(key => CleanerGroup[key].CleanedCount);
        public static double GetCleanedSize(string key) => CleanerGroup[key].CleanedSize;
        public static double GetCleanedCount(string key) => CleanerGroup[key].CleanedCount;

        #endregion

        #region ReLoad Reload Delete Run

        public static void ReLoad() => Loader.ReLoadAll();

        public static void Load() => Loader.LoadAll();
        public static void Delete(string key) => CleanerGroup[key].ToDelete();

        public static void ToRunAll()
        {
            foreach (string key in CleanerGroup.Keys) ToRun(key);
        }

        public static void ToRun(string key) => CleanerGroup[key].Start();

        #endregion
    }
}