using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Utils.LogUtils;

namespace RemoveDislike.Core.Clean
{
    /// <summary>
    ///     Need to be rewritten
    /// </summary>
    public static class Parser
    {
        public static string Correction(string str)
        {
            // Remove comments and Blank lines
            str = str.Trim();
            str = Regex.Replace(str, "//.*", "");
            str = Regex.Replace(str, "/\\*(.|[\\r\\n])*?\\*/", "");
            str = Regex.Replace(str, "\\s*\n", "");

            // Replace environment variables
            foreach (Match match in Regex.Matches(str, "%[^\\r\\n%]{1,128}%"))
                str = str.Replace(match.Value, EnvironmentUtils.Get(match.Value));

            return str.Trim();
        }

        public static List<Rule> from_string(string ruleStr) =>
            from_List(Correction(ruleStr).Split('\n'));

        public static List<Rule> from_List(IEnumerable<string> list)
        {
            List<Rule> rules = new();
            foreach (string[] split in list.Select(rule => rule.Trim().Split('|')))
                switch (split.Length)
                {
                    case 1:
                        rules.Add(new Rule(split[0].Trim()));
                        break;
                    case 2 when split[1].Trim() == ".":
                        rules.Add(new Rule(split[0].Trim(), CleanMode.Files, "*"));
                        break;
                    case 2 when split[1].Trim() == "*":
                        rules.Add(new Rule(split[0].Trim(), CleanMode.RecursionFiles,
                            "*"));
                        break;
                    case 2:
                        rules.Add(new Rule(split[0].Trim(), CleanMode.Files,
                            split.Skip(1).ToList()));
                        break;
                    case >= 3:
                        rules.Add(split[1].Trim() == "*"
                            ? new Rule(split[0].Trim(), CleanMode.RecursionFiles,
                                split.Skip(2).ToList())
                            : new Rule(split[0].Trim(), CleanMode.Files,
                                split.Skip(1).ToList()));
                        break;
                }

            return rules;
        }

        public static List<Rule> from_file(string path)
        {
            Log(@$"[RuleParser] {path}");
            return from_string(File.ReadAllText(path));
        }
    }
}