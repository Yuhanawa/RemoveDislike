using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using RemoveDislike.Views.Models.RegistryManger;

namespace RemoveDislike.Views.Pages.RegistryPage;

public partial class RegistryManagerPage : Page
{
    public RegistryManagerPage() => InitializeComponent();

    private void Panel_OnInitialized(object sender, EventArgs e)
    {
        AddItem("鼠标指针颜色", "鼠标指针颜色", Registry.CurrentUser, @"SOFTWARE\Microsoft\Accessibility", "CursorColor");
        AddItem("隐藏登录背景毛玻璃特效", "", Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
            "DisableAcrylicBackgroundOnLogon", 1, 0);
        AddItem("电源图标", "", Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
            "shutdownwithoutlogon", 1, 0);
        AddItem("任务栏亚克力透明", "", Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
            "UseOLEDTaskbarTransparency", 1, 0);
    }

    private void AddItem(string name, string description, RegistryKey root, string path, string key,
        object enabledValue, object disabledValue) =>
        Panel.Children.Add(new RegistryMangerItem(
                name, description, root, path, key, enabledValue, disabledValue)
            {
                FontSize = 18,
                Margin = new Thickness(4, 2, 4, 2),
                BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
            }
        );

    private void AddItem(string name, string description, RegistryKey root, string path, string key) =>
        Panel.Children.Add(new RegistryMangerItem(
                name, description, root, path, key)
            {
                FontSize = 18,
                Margin = new Thickness(4, 2, 4, 2),
                BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
            }
        );
}