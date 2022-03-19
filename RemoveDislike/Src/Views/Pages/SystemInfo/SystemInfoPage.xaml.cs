using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Win32;

namespace RemoveDislike.Views.Pages.SystemInfo;

public partial class SystemInfoPage : Page
{
    public SystemInfoPage()
    {
        InitializeComponent();
        DataContext = this;

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
                SysData.Add("SystemName", currentVersionReg.GetValue("ProductName")?.ToString());
                SysData.Add("SystemVersion",
                    currentVersionReg.GetValue("CurrentVersion") +
                    currentVersionReg.GetValue("CurrentBuildNumber")?.ToString());
                SysData.Add("SystemRoot", currentVersionReg.GetValue("SystemRoot")?.ToString());
                SysData.Add("ProductId", currentVersionReg.GetValue("ProductId")?.ToString());
                SysData.Add("RegisteredOwner", currentVersionReg.GetValue("RegisteredOwner")?.ToString());
            }
            else Warn("Failed to obtain system information");

            if (centralProcessorReg != null)
            {
                SysData.Add("ProcessorNameString", centralProcessorReg.GetValue("ProcessorNameString")?.ToString());
                SysData.Add("MHz", $"{centralProcessorReg.GetValue("~MHz")} MHz");
            }
            else Warn("Failed to obtain CPU information");

            if (biosReg != null)
            {
                SysData.Add(
                    "BaseBoardManufacturer",
                    $"{biosReg.GetValue("BaseBoardManufacturer")} {biosReg.GetValue("BaseBoardProduct")}");
            }
            else Warn("Failed to obtain BaseBoard information");
        }
        catch (Exception e)
        {
            Err("unknown err", e);
        }
    }

    public Dictionary<string, string> SysData { get; set; } = new()
    {
        ["MachineName"] = Environment.MachineName,
        ["OsVersionName"] = SystemInfoUtils.GetOsVersion(Environment.OSVersion.Version),
        ["ServicePack"] = Environment.OSVersion.ServicePack,
        ["UserName"] = Environment.UserName,
        ["DomainName"] = Environment.UserDomainName,
        ["TickCount"] = Environment.TickCount / 1000 + "s",
        ["SystemPageSize"] = Environment.SystemPageSize / 1024 + "KB",
        ["SystemDir"] = Environment.SystemDirectory,
        ["StackTrace"] = Environment.StackTrace,
        ["ProcessorCounter"] = Environment.ProcessorCount.ToString(),
        ["Platform"] = Environment.OSVersion.Platform.ToString(),
        ["NewLine"] = Environment.NewLine,
        ["Is64Os"] = Environment.Is64BitOperatingSystem.ToString(),
        ["Is64Process"] = Environment.Is64BitProcess.ToString(),
        ["CurrDir"] = Environment.CurrentDirectory,
        ["CmdLine"] = Environment.CommandLine,
        ["Drives"] = Environment.GetLogicalDrives().ToString()
    };
}