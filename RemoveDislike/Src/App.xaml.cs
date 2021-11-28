using RemoveDislike.Core;
using RemoveDislike.Core.Utils;
using static RemoveDislike.Core.Utils.LogUtils;


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

            Entrance.Init();

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