using System.Collections.Generic;

namespace RemoveDislike.Core.Clean
{
    public class Rule
    {
        public List<string> Feature { get; set; }
        public CleanMode CleanMode { get; set; }
        public string Path { get; set; }
        public override string ToString() => $"[Rule] [{CleanMode}] [{Feature.Count}] {Path} ";

        public Rule(string path, CleanMode cleanMode, List<string> feature)
        {
            Path = path;
            CleanMode = cleanMode;
            Feature = feature;
        }

        public Rule(string path, CleanMode cleanMode, string feature)
        {
            Path = path;
            CleanMode = cleanMode;
            Feature = new List<string> { feature };
        }

        public Rule(string path)
        {
            Path = path;
            CleanMode = CleanMode.All;
        }

        public Rule()
        {
        }
    }
}