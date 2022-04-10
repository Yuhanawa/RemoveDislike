using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows.Controls;
using Microsoft.Win32;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages;

public partial class InfoPage
{
    #region info

    

    #endregion
    public InfoPage()
    {
        InitializeComponent();
        DataContext = this;
    }
    
    private static void PutInfoTab( Panel target,string title, string value) => 
        target.Children.Add(new InfoTab(title, value));


    private void SummarySoftwarePanel_OnInitialized(object sender, EventArgs e)
    {
        PutInfoTab((Panel)sender,"OS", Environment.OSVersion.ToString());
        PutInfoTab((Panel)sender,"OS Root",  Environment.SystemDirectory);
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
    public static DateTime GetWindowsInstallationDateTime()
    {
        try
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

                DateTime installDate = 
                    DateTime.FromFileTimeUtc(
                        Convert.ToInt64(
                            key.GetValue("InstallDate").ToString()));
     
                return installDate;
        }
        catch
        {
            // ignored
        }


        return DateTime.MinValue;
    }

    public static string GetWindowsInstallationDateTimeStr()
    {
        try
        {
            DateTime date = GetWindowsInstallationDateTime();
            return $"{date.Year}-{date.Month}-{date.Day} {date.Hour}h:{date.Minute}m:{date.Second}s:{date.Millisecond}ms";
        }
        catch
        {
            return string.Empty;
        }
    }
    
    
    #endregion
}