using System.Windows;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Models;

public partial class Module
{
    public Window Window { get; set; }


    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string),
        typeof(Module), new PropertyMetadata(default(string)));

    public string Key
    {
        get => (string)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register("Key", typeof(string), typeof(Module), new PropertyMetadata(string.Empty));

    public Module()
    {
        InitializeComponent();
        Width = 100.0;
        Height = 120.0;
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void SettingButton_OnClick(object sender, RoutedEventArgs e) => Window?.Show();

    private void StateButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Key))
        {
            // TODO : Key is empty
        }
        else
        {
            if (ConfigHelper.LauncherConfig.Contains(Key))
                ConfigHelper.LauncherConfig.Remove(Key);
            else ConfigHelper.LauncherConfig.Add(Key);

            ConfigHelper.SaveLauncherConfig();
            Load();
        }
    }

    private void StateButton_OnLoaded(object sender, RoutedEventArgs e) => Load();

    private void Load()
    {
        if (ConfigHelper.LauncherConfig.Contains(Key))
        {
            StateButton.IsChecked = true;
            StateButton.Content = LangUtils.Get("Enabled");
        }
        else
        {
            StateButton.IsChecked = false;
            StateButton.Content = LangUtils.Get("Disabled");
        }
    }
}