using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Hardware.Info;
using RemoveDislike.Utils;
using RemoveDislike.Views.Pages;

namespace RemoveDislike;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    #region window
    public MainWindow()
    {
        InitializeComponent();
        Interface = this;

        Thread.CurrentThread.Name = "MainThread";
    }
    public static MainWindow Interface { get; private set; }

    private void PinButtonOnClick(object sender, RoutedEventArgs e) => Topmost = !Topmost;
    #endregion


    #region switch page
    private Dictionary<string, Page> Pages { get; } = new()
    {
        ["Clean"] = new CleanPage(),
        ["ContextMenu"] = new ContextMenuPage(),
        ["Info"] = new InfoPage(),
        ["System Adjustment"] = new AdjustmentPage(),
        ["Module"] = new ModulePage(),
    };
    

    private void CleanMenuItem_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["Clean"];

    private void ContextMenuMenuItem_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["ContextMenu"];

    private void InfoMenuMenuItem_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["Info"];
    
    
    private void SystemAdjustmentMenuItem_OnSelected(object sender, RoutedEventArgs e)=>
        MainFrame.Content = Pages["System Adjustment"];

    private void ModuleMenuItem_OnSelected(object sender, RoutedEventArgs e)=>
        MainFrame.Content = Pages["Module"];
    #endregion


    private void CpuUsageProgressBar_OnLoaded(object sender, RoutedEventArgs e) =>
        /*
         * ! TODO
         * ! Help needed
         * ! There is Inaccurate in getting the value of the CPU usage
         * ！How to get the value of the CPU usage?
         */
        new Thread(_ =>
        {
            PerformanceCounter cpuUsage = new ("Processor", "% Processor Time", "_Total");
            Thread.Sleep(1000);
            float firstCall = cpuUsage.NextValue();
            
            while (!App.WillClose)
            {
                Thread.Sleep(1000);
                CpuUsageProgressBar.Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        float usage = cpuUsage.NextValue();
                        if (usage>100) usage = 100; else if (usage<0) usage = 0;

                        CpuUsageProgressBar.Value = usage;
                        CpuUsageTextBlock.Text = $"{LangUtils.Get("CPU Usage")}: {usage:F2}%";
                        CpuUsageProgressBar.Foreground = usage < 50 
                            ? new SolidColorBrush(Color.FromRgb((byte)(usage/50*255), 255, 0)) 
                            : new SolidColorBrush(Color.FromRgb(255, (byte)((100-usage)/50*255), 0));
                    })           
                );
            }
        }){ Name = "CPU Usage listener"}.Start();

    private void RamUsageProgressBar_OnLoaded(object sender, RoutedEventArgs e) =>
        new Thread(_ =>
        {
            var hardwareInfo = new HardwareInfo();
            hardwareInfo.RefreshMemoryStatus();
            
            while (!App.WillClose)
            {
                Thread.Sleep(50);
                RamUsageProgressBar.Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        hardwareInfo.RefreshMemoryStatus();
                        float total = hardwareInfo.MemoryStatus.TotalPhysical;
                        float used = hardwareInfo.MemoryStatus.TotalPhysical-hardwareInfo.MemoryStatus.AvailablePhysical;
                        float usage =  used*100/total;
                        if (usage>100) usage = 100;

                        RamUsageProgressBar.Value = usage;
                        RamUsageTextBlock.Text = $"{LangUtils.Get("RAM Usage")}: {usage:F2}%";
                        RamUsageProgressBar.Foreground = usage < 50
                            ? new SolidColorBrush(Color.FromRgb((byte)(usage/50*255), 255, 0)) 
                            : new SolidColorBrush(Color.FromRgb(255, (byte)((100-usage)/50*255), 0));
                    })
                );
            }
        }){ Name = "RAM Usage listener"} .Start();

    private void PerformancePanel_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => ProcessUtils.TryStart("taskmgr.exe");
}