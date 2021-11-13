using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Utils.CommonUtils;


namespace RemoveDislike.Core.Clean
{
    public class Rule
    {
        public enum ModeEnum
        {
            All,
            Files,
            Folders,
            RecursionAll,            
            RecursionFiles,
            RecursionFolders,
        }

        public List<string> Feature;
        public ModeEnum Mode;

        public string Path;

        public Rule(string path, ModeEnum mode, List<string> feature)
        {
            Path = path;
            Mode = mode;
            Feature = feature;
        }

        public Rule(string path, ModeEnum mode, string feature)
        {
            Path = path;
            Mode = mode;
            Feature = new List<string> { feature };
        }

        public Rule(string path)
        {
            Path = path;
            Mode = ModeEnum.All;
        }

        public Rule()
        {
        }


        /// <summary>
        ///     File					//del this file
        ///     Dir						//del thia dir
        ///     Dir|.					//del all file on dir
        ///     Dir|.log				//del all .log file in dir
        ///     Dir|*|.log				//del all .log file in  dir and sub
        ///     Dir|*|.log|.txt|.dat	//del all .log .tx .dat file in dir and sub
        ///     Examples:
        ///     %SystemRoot%\Temp\
        ///     %SystemRoot%\ServiceProfiles\LocalService\AppData\Local\FontCache\|*|.db
        ///     D:\\logs|.log|.tmp
        /// </summary>
        public static class Parser
        {
            public static List<Rule> from_string(string ruleStr)
            {
                // Remove comments and Blank lines
                ruleStr = Regex.Replace(ruleStr, "//.*", "");
                ruleStr = Regex.Replace(ruleStr, "/\\*(.|[\\r\\n])*?\\*/", "");
                ruleStr = Regex.Replace(ruleStr, "\\s*\n", "");

                // Replace environment variables
                foreach (Match match in Regex.Matches(ruleStr, "%[^\\r\\n%]{1,128}%"))
                    ruleStr = ruleStr.Replace(match.Value, EnvironmentUtils.Get(match.Value));

                return from_List(ruleStr.Trim().Split('\n'));
            }

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
                            rules.Add(new Rule(split[0].Trim(), ModeEnum.Files, "*"));
                            break;
                        case 2 when split[1].Trim() == "*":
                            rules.Add(new Rule(split[0].Trim(), ModeEnum.RecursionFiles,
                                "*"));
                            break;
                        case 2:
                            rules.Add(new Rule(split[0].Trim(), ModeEnum.Files,
                                split.Skip(1).ToList()));
                            break;
                        case >= 3:
                            rules.Add(split[1].Trim() == "*"
                                ? new Rule(split[0].Trim(), ModeEnum.RecursionFiles,
                                    split.Skip(2).ToList())
                                : new Rule(split[0].Trim(), ModeEnum.Files,
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
}