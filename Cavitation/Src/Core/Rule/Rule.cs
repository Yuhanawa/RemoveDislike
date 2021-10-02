using System.Collections.Generic;

namespace Cavitation.Core.Rule
{
    public class Rule
    {
        public enum ModeEnum
        {
            All,
            Folders,
            RecursionAllFiles,
            FilesOnDir,
            DirsAndFiles
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
    }
}