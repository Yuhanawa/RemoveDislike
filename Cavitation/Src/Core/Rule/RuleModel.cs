using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cavitation.Core.Rule
{
    public abstract class CacheModel
    {
        public List<Rule> Rules { get; protected set; }
        public long Size { get; protected set; }
        public int Count { get; protected set; }


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

        public void Clear()
        {
            foreach (Rule rule in Rules.Where(Check))
            {
                if (!Directory.Exists(rule.Path))
                {
                    File.Delete(rule.Path);
                    continue;
                }

                var root = new DirectoryInfo(rule.Path);
                switch (rule.Mode)
                {
                    case Rule.ModeEnum.All:
                    {
                        root.Delete(true);
                        break;
                    }
                    case Rule.ModeEnum.Folders:
                    {
                        if (rule.Feature[0] == "*")
                            foreach (DirectoryInfo dir in root.GetDirectories())
                                dir.Delete(true);
                        else
                            foreach (DirectoryInfo dir in root.GetDirectories())
                                if (rule.Feature.Any(feature =>
                                    dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                    dir.Delete(true);
                        // dir.Delete(dir.GetFileSystemInfos().Length != 0);

                        break;
                    }
                    case Rule.ModeEnum.FilesOnDir:
                    {
                        if (rule.Feature[0] == "*")
                            foreach (FileInfo file in root.GetFiles())
                                file.Delete();
                        else
                            foreach (FileInfo file in root.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature))
                                    file.Delete();

                        break;
                    }
                    case Rule.ModeEnum.RecursionAllFiles:
                    {
                        if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                        void Recursion(DirectoryInfo fileSystemInfo)
                        {
                            foreach (FileInfo file in fileSystemInfo.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature))
                                    file.Delete();
                            foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                if (dir.GetFileSystemInfos().Length == 0) dir.Delete();
                                else Recursion(dir);
                        }

                        Recursion(root);

                        break;
                    }
                    case Rule.ModeEnum.DirsAndFiles:
                    {
                        if (rule.Feature[0] == "*") throw new ArgumentException(" 无效的Feature[0]==\"*\"");

                        void Recursion(DirectoryInfo fileSystemInfo)
                        {
                            foreach (FileInfo file in fileSystemInfo.GetFiles())
                                if (rule.Feature.Any(feature => file.Extension == feature))
                                    file.Delete();
                            foreach (DirectoryInfo dir in fileSystemInfo.GetDirectories())
                                if (rule.Feature.Any(feature =>
                                    dir.Name == feature || dir.GetFileSystemInfos().Length == 0))
                                    dir.Delete(dir.GetFileSystemInfos().Length != 0);
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
    }
}