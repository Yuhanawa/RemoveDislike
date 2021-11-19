using System;
using System.Collections;
using Microsoft.Win32;
using static RemoveDislike.Core.Utils.LogUtils;


// ReSharper disable MemberCanBePrivate.Global
namespace RemoveDislike.Core.Utils
{
    /// <summary>
    ///     see
    ///     https://www.cnblogs.com/hsiang/p/6814839.html
    ///     https://www.cnblogs.com/swtseaman/archive/2011/10/14/2212423.html
    /// </summary>
    public static class SystemInfoUtils
    {
        public static readonly string MachineName = Environment.MachineName;
        public static readonly string OsVersionName = GetOsVersion(Environment.OSVersion.Version);
        public static readonly string ServicePack = Environment.OSVersion.ServicePack;
        public static readonly string UserName = Environment.UserName;
        public static readonly string DomainName = Environment.UserDomainName;
        public static readonly string TickCount = Environment.TickCount / 1000 + "s";
        public static readonly string SystemPageSize = Environment.SystemPageSize / 1024 + "KB";
        public static readonly string SystemDir = Environment.SystemDirectory;
        public static readonly string StackTrace = Environment.StackTrace;
        public static readonly string ProcessorCounter = Environment.ProcessorCount.ToString();
        public static readonly string Platform = Environment.OSVersion.Platform.ToString();
        public static readonly string NewLine = Environment.NewLine;
        public static readonly bool Is64Os = Environment.Is64BitOperatingSystem;
        public static readonly bool Is64Process = Environment.Is64BitProcess;

        public static readonly string CurrDir = Environment.CurrentDirectory;
        public static readonly string CmdLine = Environment.CommandLine;
        public static readonly string[] Drives = Environment.GetLogicalDrives();

        public static void LogInfo()
        {
            Info($"NewLine: {NewLine}");
            Info($"CurrDir: {CurrDir}");
            Info($"CmdLine: {CmdLine}");
            Info($"Drives: {string.Join(",", Drives)}");
            Info($"MachineName: {MachineName}");
            Info($"OsVersionName: {OsVersionName}");
            Info($"ServicePack: {ServicePack}");
            Info($"UserName: {UserName}");
            Info($"DomainName: {DomainName}");
            Info($"TickCount: {TickCount}");
            Info($"SystemPageSize: {SystemPageSize}");
            Info($"SystemDir: {SystemDir}");
            Info($"StackTrace: {StackTrace}");
            Info($"ProcessorCounter: {ProcessorCounter}");
            Info($"Platform: {Platform}");
            Info($"Is64Os: {Is64Os}");
            Info($"Is64Process: {Is64Process}");
        }

        public static void LogInfo2()
        {
            Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Info(">>>MachineEnvironmentVariables>>>>>>>>>>>>>>>>>>>>>>>>>>");
            IDictionary dicMachine = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            foreach (string key in dicMachine.Keys)
                Info($"{key} :  {dicMachine[key]}");

            
            Info(">>>UserEnvironmentVariables>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            // HKEY_CURRENT_USER\Environment
            IDictionary dicUser = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
            
            foreach (string key in dicMachine.Keys)
                Info($"{key} :  {dicUser[key]}");

            
            Info(">>>ProcessEnvironmentVariables>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            IDictionary dicProcess = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);

            foreach (string key in dicProcess.Keys)
                Info($"{key} :  {dicProcess[key]}");

            //特殊目录
            foreach (string name in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                if (Enum.TryParse(name, out Environment.SpecialFolder sf))
                    Info($"{name} : {Environment.GetFolderPath(sf)}");
            }
        }

        private static string GetOsVersion(Version ver)
        {
            string strClient = ver.Major switch
            {
                5 when ver.Minor == 1 => "Win XP",
                6 when ver.Minor == 0 => "Win Vista",
                6 when ver.Minor == 1 => "Win 7",
                5 when ver.Minor == 0 => "Win 2000",
                _ => "未知"
            };

            return strClient;
        }

        public static void LogInfo1()
        {
            try
            {
                RegistryKey currentVersionReg =
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                RegistryKey centralProcessorReg =
                    Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                RegistryKey biosReg =
                    Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");

                if (currentVersionReg != null)
                {
                    Info($"系统名称: {currentVersionReg.GetValue("ProductName")}");
                    Info(
                        $"系统版本: {currentVersionReg.GetValue("CurrentVersion")} {currentVersionReg.GetValue("CurrentBuildNumber")})");
                    Info($"系统路径: {currentVersionReg.GetValue("SystemRoot")})");
                    Info($"系统ID:  {currentVersionReg.GetValue("ProductId")})");
                    Info($"系统用户: {currentVersionReg.GetValue("RegisteredOwner")}");
                }
                else Warn("系统信息获取失败");

                    if (centralProcessorReg != null)
                {
                    Info($"CPU型号: {centralProcessorReg.GetValue("ProcessorNameString")}");
                    Info($"CPU主频: {centralProcessorReg.GetValue("~MHz")} MHz");
                }else Warn("CPU信息获取失败");

                if (biosReg != null)
                    Info($"主板型号: {biosReg.GetValue("BaseBoardManufacturer")} {biosReg.GetValue("BaseBoardProduct")}");
                else Warn("主板信息获取失败");
            }
            catch (Exception e)
            {
                Err("", e);
            }
        }
    }
}