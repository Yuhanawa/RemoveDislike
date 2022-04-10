using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using Hardware.Info;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages;

public partial class InfoPage
{
    #region info

    

    #endregion
    public static readonly HardwareInfo info = new ();
    public InfoPage()
    {
        info.RefreshAll();
        InitializeComponent();
        DataContext = this;
    }
    
    private static void PutInfoTab( Panel target,string title, string value) => 
        target.Children.Add(new InfoTab(title, value){Margin = new Thickness(2)});


    private void SummarySoftwarePanel_OnInitialized(object sender, EventArgs e)
    {
        PutInfoTab((Panel)sender,"OS", Environment.OSVersion.ToString());
        PutInfoTab((Panel)sender,"OS Root",  EnvironmentUtils.Get("SystemRoot"));
        PutInfoTab((Panel)sender,"Architecture",  Environment.Is64BitOperatingSystem?"x64":"x86");
        PutInfoTab((Panel)sender,"Installation", GetWindowsInstallationDateTimeStr() );
        PutInfoTab((Panel)sender,"Product ID",  (from ManagementObject managementObject in new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_OperatingSystem").Get() from PropertyData propertyData in managementObject.Properties where propertyData.Name == "SerialNumber" select (string)propertyData.Value).FirstOrDefault());
        PutInfoTab((Panel)sender,"Computer Name", Environment.MachineName );
        PutInfoTab((Panel)sender,"User Name", Environment.UserName );
        PutInfoTab((Panel)sender,"DomainName",  Environment.UserDomainName);
        PutInfoTab((Panel)sender,"UpTime",  GetSystemUpTimeInfo());
        PutInfoTab((Panel)sender,"TickCount", Environment.TickCount / 1000 + "s" );
        PutInfoTab((Panel)sender,"SystemPageSize", Environment.SystemPageSize / 1024 + "KB" );
    }
    
    private void SummaryHardwarePanel_OnInitialized(object sender, EventArgs e)
    {
        
        
        PutInfoTab((Panel)sender,"CPU", info.CpuList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender,"RAM", $"{info.MemoryStatus.TotalPhysical}b ({((double)info.MemoryStatus.TotalPhysical)/1024/1024/1024:F2}GB)");
        PutInfoTab((Panel)sender,"Motherboard",  info.MotherboardList.Aggregate("", (s, i) => s+ $"{i.Product},{i.Manufacturer}"));
        PutInfoTab((Panel)sender, "Graphics", info.VideoControllerList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender,"Storage", info.DriveList.Select(i => (double)i.Size).Sum()/1024/1024/1024 + "GB");
        PutInfoTab((Panel)sender,"Network", info.NetworkAdapterList.Aggregate("", (s, i) => s + $"{i.Name} "));
        PutInfoTab((Panel)sender,"SoundCard",   info.SoundDeviceList.Aggregate("", (s, i) => s + $"{i.Name} "));
    }

    #region MyRegion

    private static string GetSystemUpTimeInfo()
    {
        try
        {
            TimeSpan time = GetSystemUpTime();
            return $"{time.Hours:D2}h:{time.Minutes:D2}m:{time.Seconds:D2}s:{time.Milliseconds:D3}ms";
        }
        catch
        {
            return string.Empty;
        }
    }

    private static TimeSpan GetSystemUpTime()
    {
        try
        {
            using var uptime = new PerformanceCounter("System", "System Up Time");
            uptime.NextValue();
            return TimeSpan.FromSeconds(uptime.NextValue());
        }
        catch
        {
            return TimeSpan.Zero;
        }
    }
    public static string GetWindowsInstallationDateTimeStr()
    {
        try
        {
            DateTime date = Directory.GetCreationTime(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));;
            return $"{date.Year}-{date.Month}-{date.Day} {date.Hour}h:{date.Minute}m:{date.Second}s:{date.Millisecond}ms";
        }
        catch
        {
            return string.Empty;
        }
    }
    
    
    #endregion
    
}