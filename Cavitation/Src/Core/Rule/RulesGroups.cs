using System.Collections.Generic;
using static Cavitation.Core.Rule.RuleParser;
using System.Web;

namespace Cavitation.Core.Rule
{
    public class RulesGroups
    {
        public Dictionary<string, RulesModel> RulesGroupsMap { get; private set; }
        public static RulesGroups Interface{ get; private set; }

        public RulesGroups()
        {
            RulesGroupsMap = new Dictionary<string, RulesModel>
            {
                { "Internal-SystemCache", new(from_string(InternalRules.System), "Internal") },
                // { "", new(from_file(@""), "") },
                // { "", new(from_file(@""), "") },
                // { "", new(from_file(@""), "") },
                // { "", new(from_file(@""), "") },
            };

            Interface = this;
        }
        
        public static RulesModel GetRulesGroup(string key) => Interface.RulesGroupsMap[key];
        public static Dictionary<string, RulesModel> GetRulesGroups() => Interface.RulesGroupsMap;
    }
}