#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using fastJSON;
using RemoveDislike.Utils.AntPathMatching;

namespace RemoveDislike.Utils;

public static class CleanupUtils
{
    public static Dictionary<string, RuleModule> RulesFileList { get; } = new();

    #region Total Size

    public static long TotalSize => RulesFileList.Sum(x => x.Value.Size);
    public static string TotalSizeStr => SizeUtils.ToString(TotalSize);

    #endregion

    #region Load and Reload

    public static void ReLoad()
    {
        RulesFileList.Clear();
        Load();
    }

    public static void Load()
    {
        RuleModule emptyFolderRule = null!;
        emptyFolderRule = new RuleModule(
            "Empty Folder",
            "Yuhanawa",
            "Empty Folder Clean", 
            false,
            (disabledList, action) =>
            {
                void EmptyFolder(string? path)
                {
                    while (true)
                    {
                        try
                        {
                            if (Directory.Exists(path) &&
                                !disabledList.Contains(path) &&
                                Directory.GetFileSystemEntries(path).Length == 0)
                            {
                                Info(path);
                                Directory.Delete(path);
                                // ReSharper disable once AccessToModifiedClosure
                                if (emptyFolderRule != null) emptyFolderRule.Size++;
                                action?.Invoke(path);
                                if (path.Length > 4 && string.IsNullOrEmpty(Directory.GetParent(path)?.FullName))
                                {
                                    path = Directory.GetParent(path)!.FullName;
                                    continue;
                                }
                            }
                            else if (path != null) Directory.GetDirectories(path).ToList().ForEach(EmptyFolder);
                        }
                        catch (Exception e)
                        {
                            Err("unknown", e);
                        }

                        break;
                    }
                }

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady) continue;
                    foreach (DirectoryInfo directory in drive.RootDirectory.EnumerateDirectories())
                        EmptyFolder(directory.FullName);
                }
            }
        );


        RulesFileList.Add("Empty Folder", emptyFolderRule);

        new DirectoryInfo(ConfigHelper.RuleBase)
            .GetFiles("*.json", SearchOption.AllDirectories)
            .ToList().ForEach(file =>
                RulesFileList.Add(file.Name, new RuleModule(file.FullName)));
    }
    public static object? TryGet(this object json, params string[] keys)
    {
        try { return keys.Aggregate(json, (current, key) => ((dynamic)current)[key]); }
        catch { return null; }
    }

}


#endregion

public class RuleModule
{
    public RuleModule(
        string name,
        string author,
        string description,
        bool danger,
        Action<ICollection<string>, Action<string>?>? featuredRule
    )
    {
        Name = name;
        Author = author;
        Description = description;
        Danger = danger;
        FeaturedRule = featuredRule;
    }

    public RuleModule(string path)
    {
        Path = path;
        var json = JSON.ToObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(path));
        
        Name = FileName.Split("@").Length<=1? FileName: FileName.Split("@")[0];
        Author = FileName.Split("@").Length<=1?"unknown": FileName.Split("@")[1];
        Description = (json.TryGet("header", "description") ?? "").ToString()??"";
        Danger = (json.TryGet("header", "danger") ?? false) as bool? ?? false;

        foreach (string str in json["rules"].Keys.ToList())
        {
            var o = (Dictionary<string, object>)json["rules"][str];
            Dictionary<string, List<string>> dic = o.Keys.ToDictionary(key => key,
                key => ((List<object>)o[key]).ConvertAll(
                        c =>
                        {
                            var output = c.ToString()!;
                            new Regex("%[^%]+%").Matches(c.ToString()!).ToList().ForEach(
                                match =>
                                    output =
                                        output.Replace(
                                            match.Value,
                                            EnvironmentUtils.Get(match.Value.Trim('%'))));
                            return output;
                        })
                    .ToList());
            /*  Parse json
             *  Maybe only God can understand this line of code
             *  "rules": {
             *      "r1": { "sub1": ["str1","str2"], "sub2": ["str1","str2"] },
             *      "r2": { "sub1": ["str1","str2"], "sub2": ["str1","str2"] }
             *     }
             * */

            SubRules.Add(str, dic);
        }

        Info($"[RuleModule] Loaded rule: {Name}@{Author} from: {Path}");
    }

    public Dictionary<string, Dictionary<string, List<string>>> SubRules { get; set; } = new();
    public string Path { get; } = "";
    public string FileName => Path.Split('\\').Last();
    public string Identifier => $"{Name} - {Author}";
    public long Size { get; set; }
    public string SizeStr => SizeUtils.ToString(Size);

    private void _Run(ICollection<string> disabledList, Action<string>? action = null)
    {
        foreach (string key in SubRules.Keys)
        {
            if (disabledList.Contains(key)) return;

            foreach (string patten in SubRules[key].Keys)
            foreach (string targetPath in SubRules[key][patten])
                try
                {
                    new AntDirectory(new Ant(patten))
                        .SearchRecursively(targetPath,
                            path =>
                            {
                                Size += new FileInfo(path).TryDel();
                                action?.Invoke(path);
                            }, true);
                }
                catch (Exception e)
                {
                    Warn($"[RuleModule] {e.Message}\nException: {e}");
                }
        }
    }

    public void Run(ICollection<string> disabledList, Action<string>? action = null)
    {
        try
        {
            if (FeaturedRule == null)
                _Run(disabledList, action);
            else
                FeaturedRule.Invoke(disabledList, action);
            GC.Collect();
        }
        catch (Exception e)
        {
            Err("[RuleModule]", e);
        }
    }

    #region header

    public string Name { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public bool Danger { get; set; }

    public Action<ICollection<string>, Action<string>?>? FeaturedRule { get; set; }

    #endregion
}