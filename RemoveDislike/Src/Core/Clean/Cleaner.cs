using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
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
            /// [RuleGroup] Source: {Source} \nRules: {Rules}
            /// </summary>
            /// <returns> [RuleGroup] Source: {Source} \nRules: {Rules} </returns>
            public override string ToString() => $"[RuleGroup] Source: {Source} \nRules: {Rules}";

            private bool Check(string path)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        if (new DirectoryInfo(path).GetFileSystemInfos().Length == 0)
                        {
                            Directory.Delete(path);
                            return false;
                        }

                        if (BlackList.Any(any => any == path))
                            return false;
                        
                        //&& new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
                        return Administrator || FileUtils.HasOperationPermission(path, true);
                        // 检查当前用户是否拥有此文件夹的操作权限
                        // Check whether the current user has operation permissions for this folder
                        
                    }

                    // ReSharper disable once InvertIf
                    if (File.Exists(path))
                    {
                        if ((File.GetAttributes(path) & FileAttributes.System) != 0 ||
                            (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0)
                            return false;

                        // new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
                        return Administrator || FileUtils.HasOperationPermission(path, false);
                        // 检查当前用户是否拥有此文件的操作权限
                        
                    }
                }
                catch (Exception e)
                {
                    Log(@$"[File] {e.Message}");
                    return false;
                }

                Warn($"[File] the file is not file or folder {path}");
                return false;
            }

            /// <summary>
            /// Start
            /// </summary>
            public void Start()
            {
                // Check the safety level
                if (SafetyLevel > ConfigHelper.Config.SafetyLevel)
                {
                    Info(@$"[File] {Source} is too dangerous({SafetyLevel}), skip it.");
                    return;
                }

                Info(@$"[File] {Source} is safe({SafetyLevel}), start to clean.");

                foreach (Rule rule in Rules.Where(rule => Check(rule.Path)))
                {
                    // if the path is a file, delete it
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
                switch (rule.CleanMode)
                {
                    case CleanMode.All:
                    {
                        TryDel(root);
                        break;
                    }
                    case CleanMode.Folders:
                    {
                        if (rule.Feature[0] == "*" || StrEq(rule.Feature[0], "all"))
                        {
                            foreach (DirectoryInfo dir in root.GetDirectories().Where(dir => Check(dir.FullName)))
                                TryDel(dir);
                            break;
                        }

                        foreach (DirectoryInfo dir in root.GetDirectories().Where(dir => Check(dir.FullName)))
                            try
                            {
                                if (rule.Feature.Any(feature =>
                                    StrEq(feature, dir.Name)) || dir.GetFileSystemInfos().Length == 0)
                                    TryDel(dir);
                            }
                            catch (Exception e)
                            {
                                Log(@$"[File] {e.Message}");
                            }

                        break;
                    }
                    case CleanMode.Files:
                    {
                        if (rule.Feature[0] == "*")
                        {
                            foreach (FileInfo file in root.GetFiles().Where(dir => Check(dir.FullName)))
                                TryDel(file);
                            break;
                        }

                        FeatureTryDelFile(root, rule);

                        break;
                    }
                    case CleanMode.RecursionFiles:
                    {
                        void TryRecursion(DirectoryInfo dir)
                        {
                            try
                            {
                                FeatureTryDelFile(dir, rule);

                                foreach (DirectoryInfo sub in dir.GetDirectories().Where(dir2 => Check(dir2.FullName)))
                                    if (sub.GetFileSystemInfos().Length == 0) sub.Delete();
                                    else TryRecursion(sub);
                            }
                            catch (Exception e)
                            {
                                Log(@$"[File] {e.Message}");
                            }
                        }

                        TryRecursion(root);

                        break;
                    }
                    case CleanMode.RecursionFolders:
                    {
                        void TryRecursion(DirectoryInfo fileSystemInfo)
                        {
                            try
                            {
                                foreach (DirectoryInfo sub in fileSystemInfo.GetDirectories()
                                    .Where(dir => Check(dir.FullName)))
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
                                Log(@$"[File] {e.Message}");
                            }
                        }

                        TryRecursion(root);

                        break;
                    }
                    case CleanMode.RecursionAll:
                    {
                        void TryRecursion(DirectoryInfo dir)
                        {
                            try
                            {
                                FeatureTryDelFile(dir, rule);

                                foreach (DirectoryInfo sub in dir.GetDirectories().Where(dir2 => Check(dir2.FullName)))
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
                                Log(@$"[File] {e.Message}");
                            }
                        }

                        TryRecursion(root);

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
                        UpdateData(FileUtils.TryExDelDir(dir, true).Size);
                        break;
                    }
                    case FileInfo file:
                    {
                        UpdateData(FileUtils.TryDelFile(file, true).Size);
                        break;
                    }
                }
            }

            private void FeatureTryDelFile(DirectoryInfo dir, Rule rule)
            {
                foreach (FileInfo file in dir.GetFiles().Where(file => Check(file.FullName)))
                    if (rule.Feature.Any(feature => StrEq(file.Extension, feature)))
                        TryDel(file);
            }

            public void ToDelete() => FileUtils.TryDelFile(Source, true);
            
            #region UpdateData
            private void UpdateData(double size) => UpdateData(size, 1);

            private void UpdateData(double size, int count)
            {
                if (size > 0)
                    CleanedSize += size / 1024 / 1024;
                if (size >= 0)
                    CleanedCount += count;
            }
            #endregion
            
            #region Fields

            public string Source { get; set; }
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