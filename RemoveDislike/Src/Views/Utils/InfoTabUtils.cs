using System.Collections.Generic;
using System.Linq;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Utils;

public static class InfoTabUtils
{
    public static IEnumerable<InfoTab> GetInfoTabs(Dictionary<string, string> dic) => 
        dic.Keys.Select(key => new InfoTab(key, dic[key]));
}