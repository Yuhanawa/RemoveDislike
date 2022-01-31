using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using fastJSON;
using RemoveDislike.Core.Utils.AntPathMatching;

namespace RemoveDislike.Core.Module;

public static class CleanupModule
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

    public static void Load() =>
        new DirectoryInfo(ConfigHelper.RuleBase)
            .GetFiles("*.json", SearchOption.AllDirectories)
            .ToList().ForEach(file =>
                RulesFileList.Add(file.Name, new RuleModule(file.FullName)));
}

#endregion

public class RuleModule
{
    public RuleModule(string path)
    {
        Path = path;
        var json = JSON.ToObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(path));

        Name = json["header"]["name"].ToString();
        Author = json["header"]["author"].ToString();
        Description = json["header"]["description"].ToString();
        Force = json["header"]["force"] as bool? ?? false;
        IgnoreCase = json["header"]["IgnoreCase"] as bool? ?? false;
        Danger = json["header"]["danger"] as bool? ?? false;

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

        Info($"[RuleModule] Loaded rule: {Name}-{Author} from: {Path}");
    }

    public Dictionary<string, Dictionary<string, List<string>>> SubRules { get; set; } = new();
    public string Path { get; }
    public string FileName => Path.Split('\\').Last();
    public string Identifier => $"{Name} - {Author}";
    public long Size { get; set; }
    public string SizeStr => SizeUtils.ToString(Size);

    private void _Run(ICollection<string> disabledList, Action<string> action = null)
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

    public void Run(ICollection<string> disabledList, Action<string> action = null)
    {
        try
        {
            _Run(disabledList, action);
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
    public bool Force { get; set; }
    public bool IgnoreCase { get; set; }
    public bool Danger { get; set; }

    #endregion
}