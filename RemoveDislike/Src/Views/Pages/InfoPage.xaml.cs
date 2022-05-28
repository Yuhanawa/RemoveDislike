using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using Hardware.Info;
using Microsoft.Win32;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages;

public partial class InfoPage
{
    private static readonly HardwareInfo Info = new();

    public InfoPage()
    {
        Info.RefreshAll();
        InitializeComponent();
        DataContext = this;
    }

    private static void PutInfoTab(Panel target, string title, string value = "") =>
        target.Children.Add(new InfoTab(title, value) { Margin = new Thickness(2) });


    private void SummarySoftwarePanel_OnInitialized(object sender, EventArgs e)
    {
        PutInfoTab((Panel)sender, "OS", Environment.OSVersion.ToString());
        PutInfoTab((Panel)sender, "OS Root", EnvironmentUtils.Get("SystemRoot"));
        PutInfoTab((Panel)sender, "Architecture", Environment.Is64BitOperatingSystem ? "x64" : "x86");
        PutInfoTab((Panel)sender, "Installation", GetWindowsInstallationDateTimeStr());
        PutInfoTab((Panel)sender, "Product ID",
            (from ManagementObject managementObject in
                    new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_OperatingSystem").Get()
                from PropertyData propertyData in managementObject.Properties
                where propertyData.Name == "SerialNumber"
                select (string)propertyData.Value).FirstOrDefault());
        PutInfoTab((Panel)sender, "Computer Name", Environment.MachineName);
        PutInfoTab((Panel)sender, "User Name", Environment.UserName);
        PutInfoTab((Panel)sender, "DomainName", Environment.UserDomainName);
        PutInfoTab((Panel)sender, "UpTime", GetSystemUpTime());
        PutInfoTab((Panel)sender, "TickCount", Environment.TickCount / 1000 + "s");
        PutInfoTab((Panel)sender, "SystemPageSize", Environment.SystemPageSize / 1024 + "KB");
    }

    private void SummaryHardwarePanel_OnInitialized(object sender, EventArgs e)
    {
        PutInfoTab((Panel)sender, "CPU", Info.CpuList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender, "RAM",
            $"{Info.MemoryStatus.TotalPhysical}b ({(double)Info.MemoryStatus.TotalPhysical / 1024 / 1024 / 1024:F2}GB)");
        PutInfoTab((Panel)sender, "Motherboard",
            Info.MotherboardList.Aggregate("", (s, i) => s + $"{i.Product},{i.Manufacturer}"));
        PutInfoTab((Panel)sender, "Graphics", Info.VideoControllerList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender, "Storage",
            Info.DriveList.Select(i => (double)i.Size).Sum() / 1024 / 1024 / 1024 + "GB");
        PutInfoTab((Panel)sender, "Network", Info.NetworkAdapterList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender, "SoundCard", Info.SoundDeviceList.Aggregate("", (s, i) => s + $"{i.Name} "));
    }

    private void CPUPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (CPU cpu in Info.CpuList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", cpu.Caption);
            PutInfoTab(panel, "Description", cpu.Description);
            PutInfoTab(panel, "Manufacturer", cpu.Manufacturer);
            PutInfoTab(panel, "Name", cpu.Name);
            PutInfoTab(panel, "ProcessorId", cpu.ProcessorId);
            PutInfoTab(panel, "SocketDesignation", cpu.SocketDesignation);
            PutInfoTab(panel, "CurrentClockSpeed", cpu.CurrentClockSpeed.ToString());
            PutInfoTab(panel, "MaxClockSpeed", cpu.MaxClockSpeed.ToString());
            PutInfoTab(panel, "NumberOfCores", cpu.NumberOfCores.ToString());
            PutInfoTab(panel, "NumberOfLogicalProcessors", cpu.NumberOfLogicalProcessors.ToString());
            PutInfoTab(panel, "L2CacheSize", cpu.L2CacheSize.ToString());
            PutInfoTab(panel, "L3CacheSize", cpu.L3CacheSize.ToString());
            PutInfoTab(panel, "SecondLevelAddressTranslationExtensions",
                cpu.SecondLevelAddressTranslationExtensions.ToString());
            PutInfoTab(panel, "VirtualizationFirmwareEnabled", cpu.VirtualizationFirmwareEnabled.ToString());
            PutInfoTab(panel, "VMMonitorModeExtensions", cpu.VMMonitorModeExtensions.ToString());
            PutInfoTab(panel, "PercentProcessorTime", cpu.PercentProcessorTime.ToString());
            PutInfoTab(panel, "Cores", string.Join(", ", cpu.CpuCoreList.Select(x => x.Name.ToString())));
        }
    }

    private void RAMPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Memory ram in Info.MemoryList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "BankLabel", ram.BankLabel);
            PutInfoTab(panel, "Capacity", ram.Capacity.ToString());
            PutInfoTab(panel, "FormFactor", ram.FormFactor.ToString());
            PutInfoTab(panel, "Manufacturer", ram.Manufacturer);
            PutInfoTab(panel, "MaxVoltage", ram.MaxVoltage.ToString());
            PutInfoTab(panel, "MinVoltage", ram.MinVoltage.ToString());
            PutInfoTab(panel, "PartNumber", ram.PartNumber);
            PutInfoTab(panel, "SerialNumber", ram.SerialNumber);
            PutInfoTab(panel, "Speed", ram.Speed.ToString());
        }
    }

    private void MotherboardPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Motherboard mb in Info.MotherboardList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Manufacturer", mb.Manufacturer);
            PutInfoTab(panel, "Product", mb.Product);
            PutInfoTab(panel, "SerialNumber", mb.SerialNumber);
        }
    }

    private void BIOSPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (BIOS bios in Info.BiosList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", bios.Caption);
            PutInfoTab(panel, "Description", bios.Description);
            PutInfoTab(panel, "Manufacturer", bios.Manufacturer);
            PutInfoTab(panel, "Name", bios.Name);
            PutInfoTab(panel, "ReleaseDate", bios.ReleaseDate);
            PutInfoTab(panel, "SerialNumber", bios.SerialNumber);
            PutInfoTab(panel, "SoftwareElementID", bios.SoftwareElementID);
            PutInfoTab(panel, "Version", bios.Version);
        }
    }

    private void OtherPanel_OnInitialized(object sender, EventArgs e)
    {
        // ignore
    }

    private void PrinterPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Printer pt in Info.PrinterList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", pt.Caption);
            PutInfoTab(panel, "Description", pt.Description);
            PutInfoTab(panel, "HorizontalResolution", pt.HorizontalResolution.ToString());
            PutInfoTab(panel, "Local", pt.Local.ToString());
            PutInfoTab(panel, "Name", pt.Name);
            PutInfoTab(panel, "Network", pt.Network.ToString());
            PutInfoTab(panel, "Shared", pt.Shared.ToString());
            PutInfoTab(panel, "VerticalResolution", pt.VerticalResolution.ToString());
        }
    }

    private void BatteryPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Battery bt in Info.BatteryList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", bt.FullChargeCapacity.ToString());
            PutInfoTab(panel, "DesignCapacity", bt.DesignCapacity.ToString());
            PutInfoTab(panel, "BatteryStatus", bt.BatteryStatus.ToString());
            PutInfoTab(panel, "EstimatedChargeRemaining", bt.EstimatedChargeRemaining.ToString());
            PutInfoTab(panel, "EstimatedRunTime", bt.EstimatedRunTime.ToString());
            PutInfoTab(panel, "ExpectedLife", bt.ExpectedLife.ToString());
            PutInfoTab(panel, "MaxRechargeTime", bt.MaxRechargeTime.ToString());
            PutInfoTab(panel, "TimeOnBattery", bt.TimeOnBattery.ToString());
            PutInfoTab(panel, "TimeToFullCharge", bt.TimeToFullCharge.ToString());
            PutInfoTab(panel, "BatteryStatusDescription", bt.BatteryStatusDescription);
        }
    }

    private void DisplayPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (VideoController vc in Info.VideoControllerList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "AdapterRAM", vc.AdapterRAM.ToString());
            PutInfoTab(panel, "Caption", vc.Caption);
            PutInfoTab(panel, "CurrentBitsPerPixel", vc.CurrentBitsPerPixel.ToString());
            PutInfoTab(panel, "CurrentHorizontalResolution", vc.CurrentHorizontalResolution.ToString());
            PutInfoTab(panel, "CurrentNumberOfColors", vc.CurrentNumberOfColors.ToString());
            PutInfoTab(panel, "CurrentRefreshRate", vc.CurrentRefreshRate.ToString());
            PutInfoTab(panel, "CurrentVerticalResolution", vc.CurrentVerticalResolution.ToString());
            PutInfoTab(panel, "Description", vc.Description);
            PutInfoTab(panel, "DriverDate", vc.DriverDate);
            PutInfoTab(panel, "DriverVersion", vc.DriverVersion);
            PutInfoTab(panel, "Manufacturer", vc.Manufacturer);
            PutInfoTab(panel, "MaxRefreshRate", vc.MaxRefreshRate.ToString());
            PutInfoTab(panel, "MinRefreshRate", vc.MinRefreshRate.ToString());
            PutInfoTab(panel, "Name", vc.Name);
            PutInfoTab(panel, "VideoModeDescription", vc.VideoModeDescription);
            PutInfoTab(panel, "VideoProcessor", vc.VideoProcessor);
        }
    }

    private void StoragePanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Drive drive in Info.DriveList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", drive.Caption);
            PutInfoTab(panel, "Description", drive.Description);
            PutInfoTab(panel, "FirmwareRevision", drive.FirmwareRevision);
            PutInfoTab(panel, "Index", drive.Index.ToString());
            PutInfoTab(panel, "Manufacturer", drive.Manufacturer);
            PutInfoTab(panel, "Model", drive.Model);
            PutInfoTab(panel, "Name", drive.Name);
            PutInfoTab(panel, "Partitions", drive.Partitions.ToString());
            PutInfoTab(panel, "SerialNumber", drive.SerialNumber);
            PutInfoTab(panel, "Size", drive.Size.ToString());
            PutInfoTab(panel, "Partition", string.Join("\n", drive.PartitionList.Select(x => x.ToString())));
        }
    }

    private void NetworkPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (NetworkAdapter na in Info.NetworkAdapterList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "AdapterType", na.AdapterType);
            PutInfoTab(panel, "Caption", na.Caption);
            PutInfoTab(panel, "Description", na.Description);
            PutInfoTab(panel, "MACAddress", na.MACAddress);
            PutInfoTab(panel, "Manufacturer", na.Manufacturer);
            PutInfoTab(panel, "Name", na.Name);
            PutInfoTab(panel, "NetConnectionID", na.NetConnectionID);
            PutInfoTab(panel, "ProductName", na.ProductName);
            PutInfoTab(panel, "Speed", na.Speed.ToString());
            PutInfoTab(panel, "BytesSentPersec", na.BytesSentPersec.ToString());
            PutInfoTab(panel, "BytesReceivedPersec", na.BytesReceivedPersec.ToString());
            PutInfoTab(panel, "DefaultIPGatewayList",
                string.Join("\n", na.DefaultIPGatewayList.Select(x => x.ToString())));
            PutInfoTab(panel, "DHCPServer", na.DHCPServer.ToString());
            PutInfoTab(panel, "DNSServerSearchOrderList",
                string.Join("\n", na.DNSServerSearchOrderList.Select(x => x.ToString())));
            PutInfoTab(panel, "IPAddressList", string.Join("\n", na.IPAddressList.Select(x => x.ToString())));
            PutInfoTab(panel, "IPSubnetList", string.Join("\n", na.IPSubnetList.Select(x => x.ToString())));
        }
    }

    private void SoundDevicePanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (SoundDevice sd in Info.SoundDeviceList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", sd.Caption);
            PutInfoTab(panel, "Description", sd.Description);
            PutInfoTab(panel, "Manufacturer", sd.Manufacturer);
            PutInfoTab(panel, "Name", sd.Name);
            PutInfoTab(panel, "ProductName", sd.ProductName);
        }
    }

    private void MonitorPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Motherboard mb in Info.MotherboardList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Manufacturer", mb.Manufacturer);
            PutInfoTab(panel, "Product", mb.Product);
            PutInfoTab(panel, "SerialNumber", mb.SerialNumber);
        }
    }

    private void InputPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (Keyboard keyboard in Info.KeyboardList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", keyboard.Caption);
            PutInfoTab(panel, "Description", keyboard.Description);
            PutInfoTab(panel, "Name", keyboard.Name);
            PutInfoTab(panel, "NumberOfFunctionKeys", keyboard.NumberOfFunctionKeys.ToString());
        }

        foreach (Mouse mouse in Info.MouseList)
        {
            StackPanel panel = new();
            ((Panel)sender).Children.Add(new GroupBox { Content = panel });
            PutInfoTab(panel, "Caption", mouse.Caption);
            PutInfoTab(panel, "Description", mouse.Description);
            PutInfoTab(panel, "Manufacturer", mouse.Manufacturer);
            PutInfoTab(panel, "Name", mouse.Name);
            PutInfoTab(panel, "NumberOfButtons", mouse.NumberOfButtons.ToString());
        }
    }

    private void SystemPanel_OnInitialized(object sender, EventArgs e)
    {
        ManagementObjectCollection moc = new ManagementClass("Win32_ComputerSystemProduct").GetInstances();
        foreach (ManagementBaseObject o in moc)
        foreach (PropertyData item in o.Properties)
            PutInfoTab((Panel)sender, item.Name, item.Value?.ToString());
    }

    private void ProgramsPanel_OnInitialized(object sender, EventArgs e) =>
        ((DataGrid)sender).ItemsSource =
        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\")?
            .GetSubKeyNames().ToList().Select(x => new Program(x)).Where(x => x.DisplayName != "");


    #region MyRegion

    private class Program
    {
        private readonly RegistryKey _key;
        public Program(string keyName) => _key =
            Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{keyName}");

        /// <summary> ProductName property </summary>
        public string DisplayName => _key.GetValue("DisplayName")?.ToString() ?? "";

        /// <summary> Derived from ProductVersion property </summary>
        public string DisplayVersion => _key.GetValue("DisplayVersion")?.ToString() ?? "";

        /// <summary> Manufacturer property </summary>
        public string Publisher => _key.GetValue("Publisher")?.ToString() ?? "";

        /// <summary> Derived from ProductVersion property </summary>
        public string VersionMinor => _key.GetValue("VersionMinor")?.ToString() ?? "";

        /// <summary> Derived from ProductVersion property </summary>
        public string VersionMajor => _key.GetValue("VersionMajor")?.ToString() ?? "";

        /// <summary> Derived from ProductVersion property </summary>
        public string Version => _key.GetValue("Version")?.ToString() ?? "";

        /// <summary> ARPHELPLINK property </summary>
        public string HelpLink => _key.GetValue("HelpLink")?.ToString() ?? "";

        // /// <summary> ARPHELPTELEPHONE property </summary>
        // public string HelpTelephone => _key.GetValue("HelpTelephone")?.ToString() ?? "";

        /// <summary>
        ///     The last time this product received service. The value of this property is replaced each time a patch is
        ///     applied or removed from the product or the /v Command-Line Option is used to repair the product. If the product has
        ///     received no repairs or patches this property contains the time this product was installed on this computer.
        /// </summary>
        public string InstallDate => _key.GetValue("InstallDate")?.ToString() ?? "";

        /// <summary> ARPINSTALLLOCATION property </summary>
        public string InstallLocation => _key.GetValue("InstallLocation")?.ToString() ?? "";

        /// <summary> SourceDir property </summary>
        public string InstallSource => _key.GetValue("InstallSource")?.ToString() ?? "";

        /// <summary> ARPURLINFOABOUT property </summary>
        public string URLInfoAbout => _key.GetValue("URLInfoAbout")?.ToString() ?? "";

        /// <summary> ARPURLUPDATEINFO property </summary>
        public string URLUpdateInfo => _key.GetValue("URLUpdateInfo")?.ToString() ?? "";

        /// <summary> ARPAUTHORIZEDCDFPREFIX property </summary>
        public string AuthorizedCDFPrefix => _key.GetValue("AuthorizedCDFPrefix")?.ToString() ?? "";

        /// <summary> ARPCOMMENTS property Comments provided to the Add or Remove Programs control panel. </summary>
        public string Comments => _key.GetValue("Comments")?.ToString() ?? "";

        /// <summary> ARPCONTACT property Contact provided to the Add or Remove Programs control panel. </summary>
        public string Contact => _key.GetValue("Contact")?.ToString() ?? "";

        /// <summary> Determined and set by the Windows Installer. </summary>
        public string EstimatedSize => _key.GetValue("EstimatedSize")?.ToString() ?? "";

        /// <summary> ProductLanguage property </summary>
        public string Language => _key.GetValue("Language")?.ToString() ?? "";

        /// <summary> Determined and set by the Windows Installer. </summary>
        public string ModifyPath => _key.GetValue("ModifyPath")?.ToString() ?? "";

        /// <summary> ARPREADME property Readme provided to the Add or Remove Programs control panel. </summary>
        public string Readme => _key.GetValue("Readme")?.ToString() ?? "";

        /// <summary> Determined and set by Windows Installer. </summary>
        public string UninstallString => _key.GetValue("UninstallString")?.ToString() ?? "";

        /// <summary> MSIARPSETTINGSIDENTIFIER property </summary>
        public string SettingsIdentifier => _key.GetValue("MSIARPSETTINGSIDENTIFIER")?.ToString() ?? "";
    }

    #endregion

    #region tools

    private static string GetSystemUpTime()
    {
        try
        {
            using var uptime = new PerformanceCounter("System", "System Up Time");
            uptime.NextValue();
            TimeSpan time = TimeSpan.FromSeconds(uptime.NextValue());
            return $"{time.Hours:D2}h:{time.Minutes:D2}m:{time.Seconds:D2}s:{time.Milliseconds:D3}ms";
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GetWindowsInstallationDateTimeStr()
    {
        try
        {
            DateTime date = Directory.GetCreationTime(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            return
                $"{date.Year}-{date.Month}-{date.Day} {date.Hour}h:{date.Minute}m:{date.Second}s:{date.Millisecond}ms";
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion
}