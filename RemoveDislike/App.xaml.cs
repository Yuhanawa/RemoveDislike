global using System;
global using RemoveDislike.Core;
global using RemoveDislike.Core.Utils;
global using static RemoveDislike.Core.Entrance;
global using static RemoveDislike.Core.Utils.LogUtils;


namespace RemoveDislike
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected App()
        {
            Info("Output system information");
            OutputSystemInfo();
            
            // RemoveDislike.Core.Entrance.Init
            Init();

            Info("Application started");
            InitializeComponent();
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
    }
}