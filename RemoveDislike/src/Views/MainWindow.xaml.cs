using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using RemoveDislike.Views.Pages.Clean;
using RemoveDislike.Views.Pages.ContextMenuPage;
using RemoveDislike.Views.Pages.RegistryPage;

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

    public Dictionary<string, Page> Pages { get; } = new()
    {
        ["CleanPage"] = new CleanPage(),
        ["RegistryManagerPage"] = new RegistryManagerPage(),
        ["ContextMenuManagementPage"] = new ContextMenuManagementPage()

        // ["Home"] = new HomePage(),
        // ["Settings"] = new SettingsPage(),
        // ["About"] = new AboutPage(),
    };

    public static MainWindow Interface { get; private set; }

    private void PinButtonOnClick(object sender, RoutedEventArgs e) => Topmost = !Topmost;

    private void CommonClear_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["CleanPage"];

    private void ContextMenuManagement_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["ContextMenuManagementPage"];

    private void RegistryManagement_OnSelected(object sender, RoutedEventArgs e) =>
        MainFrame.Content = Pages["RegistryManagerPage"];
}