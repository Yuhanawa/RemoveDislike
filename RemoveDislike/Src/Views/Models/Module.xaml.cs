using System.IO;
using System.Windows;
using Microsoft.Win32;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Models;

public partial class Module
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string),
        typeof(Module), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register(nameof(Key), typeof(string), typeof(Module), new PropertyMetadata(string.Empty));

    public string ConfigPath
    {
        get => (string)GetValue(ConfigPathProperty);
        set => SetValue(ConfigPathProperty, value);
    }

    public static readonly DependencyProperty ConfigPathProperty =
        DependencyProperty.Register(nameof(ConfigPath), typeof(string), typeof(Module), new PropertyMetadata(string.Empty));
    
    
    public Module()
    {
        InitializeComponent();
        Width = 100.0;
        Height = 120.0;
    }

    public Window Window { get; set; }

    public string Key
    {
        get => (string)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void SettingButton_OnClick(object sender, RoutedEventArgs e) => ProcessUtils.TryStart("notepad.exe", Path.Combine(ConfigHelper.ModulesPath,ConfigPath));

    private void StateButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Key))
        {
            try
            {
                RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if ((string)key!.GetValue("RemoveDislike") ==
                    AppDomain.CurrentDomain.BaseDirectory + "modules_launcher.exe") key.DeleteValue("RemoveDislike");
                else key.SetValue("RemoveDislike", AppDomain.CurrentDomain.BaseDirectory + "modules_launcher.exe");
                key.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Err(exception.Message, exception);
            }
        }
        else
        {
            if (ConfigHelper.LauncherConfig.Contains(Key))
                ConfigHelper.LauncherConfig.Remove(Key);
            else ConfigHelper.LauncherConfig.Add(Key);

            ConfigHelper.SaveLauncherConfig();
        }
        Load();
    }

    private void StateButton_OnLoaded(object sender, RoutedEventArgs e) => Load();

    private void Load()
    {
        if (string.IsNullOrEmpty(Key))
        {
            StateButton.IsChecked =
                (string)Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")
                    ?.GetValue("RemoveDislike") == AppDomain.CurrentDomain.BaseDirectory + "modules_launcher.exe";
        }
        else StateButton.IsChecked = ConfigHelper.LauncherConfig.Contains(Key);

        StateButton.Content = LangUtils.Get(StateButton.IsChecked.Value ? "Enabled" : "Disabled");
    }
}