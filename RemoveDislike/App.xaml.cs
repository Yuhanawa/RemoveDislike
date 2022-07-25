global using System;
global using static RemoveDislike.Utils.LogUtils;
using System.Threading;
using RemoveDislike.Utils;

namespace RemoveDislike;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected App()
    {
        Info("Output system information");

        Init();

        Info("Application started");
        Info($"Version: {ResourceAssembly.GetName().Version.ToString()}");
        InitializeComponent();

        Current.Exit += (_, _) =>
        {
            ConfigHelper.Save();
            
            if (!WillClose)
            {
                WillClose = true;
                Info("Application will close");
                return;
            }

            Info("Application closed");
        };
    }

    public static bool WillClose { get; set; }

    private static void Init()
    {
        ConfigHelper.Load();
        LangUtils.Load(Thread.CurrentThread.CurrentCulture.Name);
        CleanupUtils.Load();
    }
}