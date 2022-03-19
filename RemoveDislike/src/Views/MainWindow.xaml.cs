using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using RemoveDislike.Views.Pages.Clean;
using RemoveDislike.Views.Pages.ContextMenu;
using RemoveDislike.Views.Pages.SystemInfo;

// using RemoveDislike.Views.Pages.RegistryPage;

namespace RemoveDislike.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Interface = this;

        Thread.CurrentThread.Name = "MainThread";
    }

    private Dictionary<string, Page> Pages { get; } = new()
    {
        ["CleanPage"] = new CleanPage(),
        ["ContextMenuManagementPage"] = new ContextMenuManagementPage(),
        ["SystemInfoPage"] = new SystemInfoPage()
    };

    public static MainWindow Interface { get; private set; }

    private void PinButtonOnClick(object sender, RoutedEventArgs e) => Topmost = !Topmost;

    private void CommonClear_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["CleanPage"];

    private void ContextMenuManagement_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["ContextMenuManagementPage"];

    private void SystemInfo_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["SystemInfoPage"];
}