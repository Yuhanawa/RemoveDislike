using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RemoveDislike.Views.Models.ContextMenu;

namespace RemoveDislike.Views.Pages;

public partial class ContextMenuPage
{
    public ContextMenuPage() => InitializeComponent();

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    private void LoadGeneralList(object sender, EventArgs e)
    {
        LoadList(@"*\shell", (StackPanel)sender);
        LoadList(@"*\shellex\ContextMenuHandlers", (StackPanel)sender, true);
    }

    private void LoadDesktopMenuList(object sender, EventArgs e)
    {
        LoadList(@"DesktopBackground\shell", (StackPanel)sender);
        LoadList(@"DesktopBackground\shellex\ContextMenuHandlers", (StackPanel)sender, true);
    }


    private void DirectoryList_OnInitialized(object sender, EventArgs e)
    {
        LoadList(@"Directory\shell", (StackPanel)sender);
        LoadList(@"Directory\Background\shell", (StackPanel)sender);
        LoadList(@"Directory\shellex\ContextMenuHandlers", (StackPanel)sender, true);
        LoadList(@"Directory\Background\shellex\ContextMenuHandlers", (StackPanel)sender, true);
    }

    private void AllFileSystemObjectsList_OnInitialized(object sender, EventArgs e) =>
        LoadList(@"AllFilesystemObjects\\shellex\\ContextMenuHandlers\\", (StackPanel)sender, true);

    private void LoadList(string keyStr, StackPanel panel, bool isEx = false)
    {
        RegistryKey key = Registry.ClassesRoot.OpenSubKey(keyStr, true);

        if (key == null) return;
        foreach (string subKeyName in key.GetSubKeyNames())
            try
            {
                if (subKeyName.StartsWith('{') && subKeyName.EndsWith('}')) continue;
                panel.Children.Add(
                    new ContextMenuInfoTab(key.OpenSubKey(subKeyName, true), subKeyName, isEx)
                );
            }
            catch (Exception exception)
            {
                Err($"RegistryKey: {subKeyName} Can't be opened.\n", exception);
            }
    }

    private void FolderList_OnInitialized(object sender, EventArgs e)
    {
        LoadList(@"Folder\shell", (StackPanel)sender);
        LoadList(@"Folder\shellex\ContextMenuHandlers", (StackPanel)sender, true);
    }

    private void Add_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO Add_OnClick
    }

    private void OpenFolder_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO OpenFolder_OnClick
    }

    private void More_OnClick(object sender, RoutedEventArgs e)
    {
        // ToDO More_OnClick
    }
}