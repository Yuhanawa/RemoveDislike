global using System;
global using RemoveDislike.Core;
global using RemoveDislike.Core.Utils;
global using static RemoveDislike.Core.Entrance;
global using static RemoveDislike.Core.Utils.LogUtils;

using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

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

            StartAsAdmin();

            // To improve readability
            // ReSharper disable once ArrangeStaticMemberQualifier
            Entrance.Init();

            Info("Application started");
            InitializeComponent();
        }

        /// <summary>
        ///     When the current user is an administrator, start the application directory.
        ///     If not an administrator, start it runs as an administrator
        /// </summary>
        /// <returns></returns>
        private static void StartAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            //判断当前登录用户是否为管理员
            if (principal.IsInRole(WindowsBuiltInRole.Administrator)) return;

            //创建启动对象
            ProcessStartInfo startInfo = new()
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Assembly.GetExecutingAssembly().Location,
                //设置启动动作,确保以管理员身份运行
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{IGet("StartAsAdminError")}/n{IGet("details")}:{e.Message}",
                    IGet("AppStartAsAdminErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
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