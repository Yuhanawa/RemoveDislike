using System.Windows;
using Microsoft.Win32;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages
{
    public partial class ContextMenuManagerPage
    {
        public ContextMenuManagerPage()
        {
            InitializeComponent();
        }

        private void Temp_OnClick(object sender, RoutedEventArgs e)
        {
            // throw new System.NotImplementedException();
        }

        private void General_OnLoaded(object sender, RoutedEventArgs e)
        {
            RegistryKey general = Core.Regedit.ContextMenu.GetGeneralMenu;
            RegistryKey shell = Core.Regedit.ContextMenu.GeneralMenu.GetShell;
            RegistryKey shellex = Core.Regedit.ContextMenu.GeneralMenu.GetShellexcmh;
            // RegistryKey _shellex =  Core.Regedit.ContextMenu.GeneralMenu.Get_shellexcmh;

            foreach (string subKeyName in shell.GetSubKeyNames())
                GeneralShellList.Children.Add(new ContextMenuInfoTab(true) { Text = subKeyName });

            foreach (string subKeyName in shellex.GetSubKeyNames())
                GeneralShellExList.Children.Add(new ContextMenuInfoTab(true) { Text = subKeyName });
        }
    }
}