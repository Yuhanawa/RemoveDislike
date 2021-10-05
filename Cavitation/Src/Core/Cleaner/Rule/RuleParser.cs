using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Cavitation.Core.Utils;

namespace Cavitation.Core.Cleaner.Rule
{
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
    public static class RuleParser
    {
        public static List<Cleaner.Rule.Rule> AutoRead<T>(T any)
        {
            if (typeof(T) == typeof(string))
            {
                if (File.Exists(any.ToString())) return from_file(any.ToString());

                if (Directory.Exists(any.ToString()))
                {
                    List<Cleaner.Rule.Rule> rules = new();
                    foreach (KeyValuePair<string, List<Cleaner.Rule.Rule>> keyValuePair in from_folder(any.ToString()))
                        rules.AddRange(keyValuePair.Value);

                    return rules;
                }

                if (!any.ToString().Contains("\n")) return from_string(any.ToString());
            }
            else if (typeof(T) == typeof(IEnumerable<string>))
            {
                return from_List(any as List<string>);
            }

            throw new ArgumentException();
        }

        public static List<Cleaner.Rule.Rule> from_string(string ruleString)
        {
            return from_List(Legalize(ruleString).Split('\n'));
        }

        public static List<Cleaner.Rule.Rule> from_List(IEnumerable<string> list)
        {
            List<Cleaner.Rule.Rule> rules = new();
            foreach (string[] split in list.Select(rule => rule.Trim().Split('|')))
                switch (split.Length)
                {
                    case 1:
                        rules.Add(new Cleaner.Rule.Rule(split[0].Trim()));
                        break;
                    case 2 when split[1].Trim() == ".":
                        rules.Add(new Cleaner.Rule.Rule(split[0].Trim(), Cleaner.Rule.Rule.ModeEnum.FilesOnDir, "*"));
                        break;
                    case 2 when split[1].Trim() == "*":
                        rules.Add(new Cleaner.Rule.Rule(split[0].Trim(), Cleaner.Rule.Rule.ModeEnum.RecursionAllFiles, "*"));
                        break;
                    case 2:
                        rules.Add(new Cleaner.Rule.Rule(split[0].Trim(), Cleaner.Rule.Rule.ModeEnum.FilesOnDir, split.Skip(1).ToList()));
                        break;
                    case >= 3:
                        rules.Add(split[1].Trim() == "*"
                            ? new Cleaner.Rule.Rule(split[0].Trim(), Cleaner.Rule.Rule.ModeEnum.RecursionAllFiles, split.Skip(2).ToList())
                            : new Cleaner.Rule.Rule(split[0].Trim(), Cleaner.Rule.Rule.ModeEnum.FilesOnDir, split.Skip(1).ToList()));
                        break;
                }

            return rules;
        }

        public static List<Cleaner.Rule.Rule> from_file(string path)
        {
            return from_string(File.ReadAllText(path));
        }

        public static Dictionary<string, List<Cleaner.Rule.Rule>> from_folder(string path)
        {
            return new DirectoryInfo(path).GetFiles("*.cr")
                .ToDictionary(
                    file => file.Name,
                    file => from_string(File.ReadAllText(file.FullName))
                );
        }

        private static string Legalize(string input)
        {
            // Remove comments and Blank lines
            input = Regex.Replace(input, "//.*", "");
            input = Regex.Replace(input, "/\\*(.|[\\r\\n])*?\\*/", "");
            input = Regex.Replace(input, "\\s*\n", "");

            // 
            foreach (Match match in Regex.Matches(input, "%[^\\r\\n%]{1,128}%"))
                input = input.Replace(match.Value, GetEnvironmentVar(match.Value));

            return input.Trim();
        }
    }
}