using System.Windows;
using Microsoft.Win32;

namespace RemoveDislike.Views.Models.RegistryManger;

public partial class RegistryMangerItem
{
    public RegistryMangerItem(string name, string description, RegistryKey root, string path, string key, object enabledValue, object disabledValue)
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
    public RegistryKey Root {get; private set;}
    public string Path {get; private set;}
    public string Key {get; private set;}

    public object EnabledValue {get; private set;}
    public object DisabledValue {get; private set;}
        

    public object GetValue()
    {
        try
        {
            RegistryKey reg = Root.OpenSubKey(Path, true);
            return reg?.GetValue(Key);
        }
        catch (Exception e)
        {
            Err(e.Message,e);
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
            Err(e.Message,e);
        }
    }

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
    
}