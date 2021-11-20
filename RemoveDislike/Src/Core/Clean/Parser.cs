using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using fastJSON;
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

        public static List<Rule> FromFile(string path)
        {
            Info(@$"[RuleParser] Loading rules from {path}");
            return JSON.ToObject<List<Rule>>(Correction(File.ReadAllText(path)));
        }

        public static List<Rule> FromString(string json)
        {
            Info(@"[RuleParser] Loading rules from json(string)");
            return JSON.ToObject<List<Rule>>(Correction(json));
        }
    }
}