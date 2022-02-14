using System.Windows;
using Microsoft.Win32;

namespace RemoveDislike.Views.Models.RegistryManger;

public partial class RegistryMangerItem
{
    public RegistryMangerItem(string name, string description, RegistryKey root, string path, string key,
        object enabledValue, object disabledValue)
    {
        RegName = name;
        Description = description;
        Root = root;
        Path = path;
        Key = key;
        EnabledValue = enabledValue;
        DisabledValue = disabledValue;

        DataContext = this;
        InitializeComponent();
        ToggleBtn.Visibility = Visibility.Visible;
    }

    public RegistryMangerItem(string name, string description, RegistryKey root, string path, string key)
    {
        RegName = name;
        Description = description;
        Root = root;
        Path = path;
        Key = key;

        DataContext = this;
        InitializeComponent();
        Box.Visibility = Visibility.Visible;
    }

    public RegistryKey Root { get; }
    public string Path { get; }
    public string Key { get; }

    public object EnabledValue { get; }
    public object DisabledValue { get; }

    public string RegName { get; set; } = "you shouldn't see it";

    public string Description { get; set; }

    public bool RegValue
    {
        get => GetValue() == EnabledValue;
        set => SetValue(value ? EnabledValue : DisabledValue);
    }

    public string RegTextValue
    {
        get => GetValue() as string;
        set => SetValue(value);
    }


    public object GetValue()
    {
        try
        {
            RegistryKey reg = Root.OpenSubKey(Path, true);
            return reg?.GetValue(Key);
        }
        catch (Exception e)
        {
            Err(e.Message, e);
            return null;
        }
    }

    public void SetValue(object value)
    {
        try
        {
            RegistryKey reg = Root.OpenSubKey(Path, true);
            reg?.SetValue(Key, value);
        }
        catch (Exception e)
        {
            Err(e.Message, e);
        }
    }
}