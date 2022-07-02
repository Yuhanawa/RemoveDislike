global using System;
global using RemoveDislike.Core;
global using RemoveDislike.Core.Utils;
global using static RemoveDislike.Core.Utils.LogUtils;
using System.IO;
using System.Threading;
using RemoveDislike.Core.Module;
using RemoveDislike.Views.Utils;

namespace RemoveDislike;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static bool WillClose { get; set; }

    protected App()
    {
        Info("Output system information");
        OutputSystemInfo();

        Init();

        Info("Application started");
        Info($"Version: {ResourceAssembly.GetName().Version.ToString()}");
        InitializeComponent();
        
        Current.Exit += (_, _) =>
        {
            if (!WillClose)
            {
                WillClose = true;
                Info("Application will close");
                return;
            }

            Info("Application closed");
        };
    }

    private static void OutputSystemInfo()
    {
        Log(">>> OutputSystemInfo : \n");
        SystemInfoUtils.LogInfo();
        Log("\n------------------------\n");
        SystemInfoUtils.LogInfo1();
        Log("\n------------------------\n");
        SystemInfoUtils.LogInfo2();
        Log("\n------------------------\n");
    }

    private static void Init()
    {
        // Release resources
        try
        {
            File.WriteAllText(Path.Combine(ConfigHelper.RuleBase, "CleanTemp.json"),
                "{\"header\":{\"name\":\"CleanTempFiles\",\"author\":\"Yuhanawa\",\"description\":\"Cleancacheandtemporaryfilesofsystemandsoftware\",\"IgnoreCase\":false,\"danger\":false,\"force\":false},\"rules\":{\"Logs:WindowsLogsandUserLog\":{\"*\":[\"%WINDIR%/Logs/\",\"%WINDIR%/SoftwareDistribution/DataStore/Logs/\",\"C:/ProgramData/Microsoft/Search/Data/Applications/Windows/GatherLogs/\",\"%WINDIR%/System32/LogFiles/\"],\"/**/*.{log,logs,Log,Logs,LOG,LOGS}\":[\"%WINDIR%/\",\"%APPDATA%/../Local/Microsoft/Windows/WebCache/\",\"%APPDATA%/../../\",],\"/**/{log,logs,Log,Logs,LOG,LOGS}\":[\"%APPDATA%/../../\"],\"/**/{log,logs,Log,Logs,LOG,LOGS}.{txt}\":[\"%APPDATA%/../../\"],\"/**/{log,logs,Log,Logs,LOG,LOGS}/**\":[\"%APPDATA%/../../\"]},\"Caches:WindowsCacheandUserCache(orTemp)\":{\"*\":[\"%APPDATA%/../Local/Microsoft/Windows/Explorer\",\"%WINDIR%/Temp/\",\"%APPDATA%/Microsoft/Windows/Recent\",\"%WINDIR%/../$WinREAgent/\",\"%WINDIR%/ServiceProfiles/LocalService/AppData/Local/FontCache/\",\"%APPDATA%/../LocalLow/Microsoft/CryptnetUrlCache/Content/\",\"%WINDIR%/SoftwareDistribution/Download/SharedFileCache/\",\"%WINDIR%/Prefetch/\",\"%WINDIR%/Fonts/Deleted/\",\"%WINDIR%/ActionCenterCache/\",\"%APPDATA%/../Local/CrashDumps/\",\"%APPDATA%/../Local/Microsoft/Windows/WebCache\"],\"/**/*.{tmp,temp,Tmp,Temp}\":[\"%APPDATA%/../../\"],\"/**/{tmp,temp,Tmp,Temp}/**\":[\"%APPDATA%/../../\",\"%WINDIR%/\"],\"/**/*.{cache,Cache}\":[\"%APPDATA%/../../\"],\"/**/{cache,Cache}/**\":[\"%APPDATA%/../../\"]}}}");
            File.WriteAllText(Path.Combine(ConfigHelper.RuleBase, "Browser dedicated clear.json"),
                "{\"header\":{\"name\":\"Browserdedicatedclear\",\"author\":\"Yuhanawa\",\"description\":\"Browserdedicatedclear\",\"IgnoreCase\":false,		\"danger\":false,\"force\":false},\"rules\":{\"Main\":{\"/**/{Cache,ShaderCache,GrShaderCache,CodeCache,GPUCache,Logs}/**\":[\"%APPDATA%/../Local/\"],\"/**/*.{log,logs,Log,Logs,LOG,LOGS}\":[\"%APPDATA%/../Local/\"]}}}");
            File.WriteAllText(Path.Combine(ConfigHelper.RuleBase, "IDE dedicated clear.json"),
                "{\"header\":{\"name\":\"IDEdedicatedclear\",\"author\":\"Yuhanawa\",\"description\":\"IDEdedicatedclear\",\"IgnoreCase\":false,		\"danger\":false,\"force\":false},\"rules\":{\"JetBrains\":{\"/**/{log,imageCache,caches,tmp,temp,ShellCaches,SolutionCaches}/**\":[\"%APPDATA%/../Local/JetBrains/\"]},\"VisualStudio\":{\"/**/{SettingsLogs,BackupFiles,Cache,PackageCache}/**\":[\"%APPDATA%/../Local/Microsoft/VisualStudio/\",\"%APPDATA%/../Local/Microsoft/Blend/\"]}}}");
        }
        catch (Exception e)
        {
            Err(e.Message, e);
        }

        ConfigHelper.Load();
        LangUtils.Load(Thread.CurrentThread.CurrentCulture.Name);
        CleanupModule.Load();
    }
}