using System.IO;
using System.Windows;
using fastJSON;

namespace RemoveDislike.Core
{
    public static class ConfigHelper
    {
        public static readonly string ConfigPath = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike";
        public static readonly string ConfigFile = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike\Config.json";
        public static readonly string RuleBase = @$"{ConfigPath}\Rules";

        private static readonly JSONParameters JsonParameters = new()
            { UseExtensions = false, UseEscapedUnicode = false };

        private static bool _isInit;

        public static ConfigModule Config { get; set; }

        private static bool Exists()
        {
            if (!Directory.Exists(RuleBase))
                Directory.CreateDirectory(RuleBase);

            return File.Exists(ConfigFile);
        }

        public static void Load()
        {
            if (_isInit) return;

            Info("[Config] Loading...");
            if (Exists())
            {
                try
                {
                    Config = JSON.ToObject<ConfigModule>(File.ReadAllText(ConfigFile), JsonParameters);
                }
                catch (Exception e)
                {
                    Err($"[Config] Config exception: {e.Message}", null);
                    if (
                        MessageBox.Show(
                            "Config exception, whether to use the default",
                            "exception",
                            MessageBoxButton.YesNo, MessageBoxImage.Error
                        ) == MessageBoxResult.Yes
                    )
                    {
                        Warn("[Config] Use default config");
                        Save();
                    }
                    else
                    {
                        Fatal("[Config] Fatal misconfiguration", e);
                        
                        Environment.Exit(-2);
                    }
                }
            }
            else
            {
                Warn("[Config] Not found Config, Will to regenerated");
                Save();
            }

            _isInit = true;
        }

        public static void Save()
        {
            Info("[Config] Saving...");
            try
            {
                File.WriteAllText(ConfigFile, JSON.ToNiceJSON(Config));
            }
            catch (Exception e)
            {
                Warn(@$"[Config] Save Fail: {e.Message}");
            }
        }

        public static void ReLoad()
        {
            Info("[Config] ReLoading...");

            _isInit = false;
            Load();
        }

        public class ConfigModule
        {
            public bool AllowForce { get; set; } = false;
        }
    }
}