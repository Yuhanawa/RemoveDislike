using System.Collections.Generic;
using Cavitation.Core.Rule;
using static Cavitation.Core.Cleaner.Rule.RuleParser;

namespace Cavitation.Core.Cleaner.Rule
{
    public class RulesGroups
    {
        private static readonly Dictionary<string, string> InternalRules = new()
        {
            { "test0", @"" },
            { "test1", @"" },
            { "test2", @"" },
            { "test3", @"" },
            { "test4", @"" },
            { "test5", @"" },
            { "test6", @"" },
            { "test7", @"" },
            { "test8", @"" },
            { "test9", @"" },
            { "test10", @"" },
            { "test11", @"" },
            { "test12", @"" },
            { "test13", @"" },
            { "test14", @"" },
            { "test15", @"" },
            { "test16", @"" },
            { "test17", @"" },
            { "test18", @"" },
            { "test19", @"" },
            { "test20", @"" },
            { "test21", @"" },
            { "test22", @"" },
            { "test23", @"" },
            { "test24", @"" },
            { "test25", @"" },
            { "test26", @"" },
            { "test27", @"" },
            { "test29", @"" },
            { "test30", @"" },
        };

        public RulesGroups()
        {
            RulesGroupsMap = new Dictionary<string, RulesModel>();
            foreach (KeyValuePair<string, string> keyValuePair in InternalRules)
                RulesGroupsMap.Add(keyValuePair.Key, new RulesModel(from_string(keyValuePair.Value), "Internal"));

            Interface = this;
        }


        public Dictionary<string, RulesModel> RulesGroupsMap { get; }
        public static RulesGroups Interface { get; private set; }

        public static RulesModel GetRulesGroup(string key)
        {
            return Interface.RulesGroupsMap[key];
        }

        public static Dictionary<string, RulesModel> GetRulesGroups()
        {
            return Interface.RulesGroupsMap;
        }
    }
}