using System;
using System.IO;
using System.Windows;
using fastJSON;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Utils.LogUtils;


namespace RemoveDislike.Core
{
    public static class ConfigHelper
    {
        public static readonly string ConfigPath = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike";
        public static readonly string ConfigFile = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike\Config.json";
        public static readonly string RuleBase = @$"{ConfigPath}\Rules";

        public static ConfigModule Config { get; set; }

        private static readonly JSONParameters JsonParameters = new()
            { UseExtensions = false, UseEscapedUnicode = false };

        private static bool _isInit;

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

                        BuildDefault();

                        Environment.Exit(-2);
                    }
                }
            }
            else
            {
                Warn("[Config] Not found Config, Will to regenerated");
                BuildDefault();
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

        public static void BuildDefault() =>
            Config = new ConfigModule
            {
                SafetyLevel = 2
            };

        public class ConfigModule
        {
            /// <summary>
            ///     0: Safety
            ///     1: Normal
            ///     2: Danger
            ///     3: Very Danger
            ///     4: Extremely Danger
            ///     5: Risk of death
            ///     6: Refuse to execute
            /// </summary>
            public int SafetyLevel { get; set; }
        }
    }
}