using System.IO;
using RemoveDislike.Core.Utils;


namespace RemoveDislike.Core
{
    public class Config
    {
        public static readonly string ConfigPath = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike";
        public static readonly string RulesGroupsPath = @$"{ConfigPath}\Rules";

        private bool _isInit;

        public Config()
        {
            Load();
        }

        public bool Exists()
        {
            if (Directory.Exists(ConfigPath)) return true;
            Directory.CreateDirectory(ConfigPath);
            Directory.CreateDirectory(RulesGroupsPath);
            File.Create(@$"{ConfigPath}\Config.json");
            Save();
            return false;
        }

        public void Load()
        {
            if (!Exists() || _isInit) return;

            _isInit = true;
        }

        public void Save()
        {
        }

        public void ReLoad()
        {
            if (!Exists()) return;

            Load();
        }
    }
}