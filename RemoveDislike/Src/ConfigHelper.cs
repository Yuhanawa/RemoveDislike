using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using fastJSON;
using RemoveDislike.Utils;

namespace RemoveDislike;

public static class ConfigHelper
{
    public static readonly string ConfigPath = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike";
    public static readonly string ConfigFile = @$"{EnvironmentUtils.Get("APPData")}\RemoveDislike\Config.json";
    public static readonly string RuleBase = @$"{ConfigPath}\Rules";
    public static readonly string ModulesPath = @$"{ConfigPath}\Modules";
    public static readonly string LauncherConfigPath = @$"{ModulesPath}\Launcher.config";

    public static readonly ResourceDictionary ColorScheme = new()
    {
        Source = new Uri("pack://application:,,,/RemoveDislike;component/src/ColorScheme.xaml")
    };


    public static List<string> LauncherConfig = _LauncherConfig;

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

        Info("[Modules][Config] Loading...");
        if (!File.Exists(Path.Combine(ModulesPath, "Launcher.config")))
            File.Create(Path.Combine(ModulesPath, "Launcher.config"));

        if (!File.Exists(Path.Combine(ModulesPath, "WindowTopmost.json")))
        {
            File.Create(Path.Combine(ModulesPath, "WindowTopmost.json"));
            File.WriteAllText(Path.Combine(ModulesPath, "WindowTopmost.json"), @"
[
    {
        'Key': 'D',
        'ModifierKeys': [
            'Control',
            'Alt'
        ]
    },
    {
        'Key': 'T',
        'ModifierKeys': [
            'Alt'
        ]
    }
]
".Replace("'", "\""));
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

    #region LauncherConfig

    private static List<string> _LauncherConfig
    {
        get => File.ReadAllText(LauncherConfigPath).Split('\n').ToList();
        set => File.WriteAllText(LauncherConfigPath, string.Join('\n', value));
    }


    public static void ReloadLauncherConfig() => LauncherConfig = _LauncherConfig;
    public static void SaveLauncherConfig() => _LauncherConfig = LauncherConfig;

    #endregion
}