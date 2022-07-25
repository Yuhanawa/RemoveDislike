using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        
        try
        {
#pragma warning disable SYSLIB0014
            using var client = new WebClient();
#pragma warning restore SYSLIB0014
            // Download File From Gist Id 9e9a0e9f38e20710f68973daa2fe7418
            client.Headers.Add("User-Agent", "RemoveDislike (Windows NT 10.0; Win64; x64)");
                    
                
            var gist = JSON.ToObject<Gist>(client.DownloadString("https://api.github.com/gists/9e9a0e9f38e20710f68973daa2fe7418"));
            if (gist.updated_at!=""&&gist.updated_at != Config.RulesUpdatedAt)
            {
                foreach ((string fileName, GistFile gistFile) in gist.files)
                    File.WriteAllText(Path.Combine(RuleBase, fileName), gistFile.content);
                Config.RulesUpdatedAt=gist.updated_at;
            }

            client.Dispose();
            gist = null;
        }
        catch (Exception e)
        {
            Err(e.Message, e);
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

        GC.Collect();
        _isInit = true;
    }

    public static void Save()
    {
        Info("[Config] Saving...");
        Config ??= new ConfigModule();

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

    #region Class
    public class ConfigModule
    {
        public bool AllowForce { get; set; } = false;
        public string RulesUpdatedAt { get; set; } = "114541";
        
    }
    public class Gist
    {
        // ReSharper disable once InconsistentNaming
        public string updated_at { get; set; } = "";
        // ReSharper disable once InconsistentNaming
        public Dictionary<string, GistFile> files { get; set; } = null;
    }    
    public class GistFile
    {
        // ReSharper disable once InconsistentNaming
        public string content { get; set; }
    }        
    #endregion


    #region LauncherConfig

    // ReSharper disable once InconsistentNaming
    private static List<string> _LauncherConfig
    {
        get => File.ReadAllText(LauncherConfigPath).Split('\n').ToList();
        set => File.WriteAllText(LauncherConfigPath, string.Join('\n', value));
    }


    public static void ReloadLauncherConfig() => LauncherConfig = _LauncherConfig;
    public static void SaveLauncherConfig() => _LauncherConfig = LauncherConfig;

    #endregion
}