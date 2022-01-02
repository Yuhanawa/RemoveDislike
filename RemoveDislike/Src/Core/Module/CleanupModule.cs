using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using fastJSON;

namespace RemoveDislike.Core.Module
{
    public static class CleanupModule
    {
        public static Dictionary<string, RuleFile> RulesFileList { get; } = new();
        public static long TotalSize { get; set; }
        public static string TotalSizeStr => SizeToStr(TotalSize);

        public static void Load()
        {
            foreach (string path in Directory.GetFiles(
                ConfigHelper.RuleBase, "*.json", SearchOption.AllDirectories))
            {
                RulesFileList.Add("WillBeAdded", new RuleFile(path));
                RulesFileList.Add(RulesFileList["WillBeAdded"].Identifier, RulesFileList["WillBeAdded"]);
                RulesFileList.Remove("WillBeAdded");
            }
        }

        public static long Run(string path)
        {
            long size = Cleanup(path);
            foreach (RuleFile rf in RulesFileList.Values.Where(rf => rf.Path == path))
                rf.Size += size;

            TotalSize += size;
            return size;
        }

        public static long Run(string name, string author)
        {
            if (!RulesFileList.ContainsKey(name) || RulesFileList[name].Author != author)
                return 0;

            return Run(RulesFileList[name].Path);
        }


        #region DllImport

        [DllImport("lib\\CleanupModule.dll", CharSet = CharSet.Unicode)]
        private static extern long Cleanup(string path);

        [DllImport("lib\\CleanupModule.dll", CharSet = CharSet.Unicode)]
        internal static extern string SizeToStr(long size);

        #endregion
    }

    public class RuleFile
    {
        public RuleFile(string path)
        {
            Path = path;
            var json = JSON.ToObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(path));

            Name = json["header"]["name"].ToString();
            Author = json["header"]["author"].ToString();
            Description = json["header"]["description"].ToString();
            Force = json["header"]["force"] as bool? ?? false;
            SubRules = json["rules"].Keys.ToList();
        }

        public List<string> SubRules { get; set; }
        public string Path { get; }
        public string Identifier => $"{Name} - {Author}";
        public bool IsEnable { get; set; }
        public long Size { get; set; }

        public string SizeStr => $"{Size / 1024 / 1024:F0} MB"; // CleanupModule.SizeToStr(Size);        


        #region header

        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public bool Force { get; set; }

        #endregion
    }
}