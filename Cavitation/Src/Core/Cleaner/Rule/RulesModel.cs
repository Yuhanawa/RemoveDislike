using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Cavitation.Core.Utils;

namespace Cavitation.Core.Rule
{
    public class RulesModel
    {
        public RulesModel(List<Cleaner.Rule.Rule> rules, string source)
        {
            Rules = rules;
            Source = source;
        }

        public List<Cleaner.Rule.Rule> Rules { get; protected set; }
        public double CleanupSize { get; protected set; }
        public int CleanupCount { get; protected set; }
        public string Source { get; protected set; }
        public int RulesCount => Rules.Count;

        public override string ToString()
        {
            return $"[RuleGroup] Source: {Source} \nRules: {Rules}";
        }

        private static bool Check(Cleaner.Rule.Rule rule)
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

        public void Clear()
        {
            CleanupCount = 0;
            CleanupSize = 0L;
            foreach (Cleaner.Rule.Rule rule in Rules.Where(Check))
            {
                if (!Directory.Exists(rule.Path))
                {
                    TryDelFile(rule.Path);
                    continue;
                }

                var root = new DirectoryInfo(rule.Path);
                switch (rule.Mode)
                {
                    case Cleaner.Rule.Rule.ModeEnum.All:
                    {
                        TryDelDir(root, true);
                        break;
                    }
                    case Cleaner.Rule.Rule.ModeEnum.Folders:
                    {
                        if (rule.Feature[0] == "*")
                            foreach (DirectoryInfo dir in root.GetDirectories())
                                TryDelDir(dir, true);
                        else
                            foreach (DirectoryInfo dir in root.GetDirectories())
                                if (rule.Feature.Any(feature =>
                                    dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                    TryDelDir(dir, true);
                        break;
                    }
                    case Cleaner.Rule.Rule.ModeEnum.FilesOnDir:
                    {
                        if (rule.Feature[0] == "*")
                            foreach (FileInfo file in root.GetFiles())
                                TryDelFile(file);
                        else
                            foreach (FileInfo file in root.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature))
                                    TryDelFile(file);
                        break;
                    }
                    case Cleaner.Rule.Rule.ModeEnum.RecursionAllFiles:
                    {
                        bool universal = rule.Feature[0] == "*";
                        // if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                        void Recursion(DirectoryInfo fileSystemInfo)
                        {
                            foreach (FileInfo file in fileSystemInfo.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature) || universal)
                                    TryDelFile(file);
                            foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                if (dir.GetFileSystemInfos().Length == 0) TryDelDir(dir);
                                else Recursion(dir);
                        }

                        Recursion(root);

                        break;
                    }
                    case Cleaner.Rule.Rule.ModeEnum.DirsAndFiles:
                    {
                        if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                        void Recursion(DirectoryInfo fileSystemInfo)
                        {
                            foreach (FileInfo file in fileSystemInfo.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature))
                                    TryDelFile(file);
                            foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                if (rule.Feature.Any(feature =>
                                    dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                    TryDelDir(dir, dir.GetFileSystemInfos().Length != 0);
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


        private static string TryDelFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                return Log(e.Message);
            }

            return Log($"Successfully deleted: {path}");
        }

        private static string TryDelFile(FileInfo file)
        {
            try
            {
                file.Delete();
            }
            catch (Exception e)
            {
                return Log(e.Message);
            }

            return Log($"Successfully deleted: {file.FullName}");
        }

        private static string TryDelDir(DirectoryInfo dir)
        {
            try
            {
                dir.Delete();
            }
            catch (Exception e)
            {
                return Log(e.Message);
            }

            return Log($"Successfully deleted: {dir.FullName}");
        }

        private static string TryDelDir(DirectoryInfo dir, bool recursive)
        {
            try
            {
                Log($"Deleted: {dir.FullName}");
                dir.Delete(recursive);
            }
            catch (Exception e)
            {
                return Log(e.Message);
            }

            return Log($"Successfully deleted: {dir.FullName}");
        }
    }
}