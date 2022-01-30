using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.Win32;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages
{
    public partial class ContextMenuManagementPage
    {
        public ContextMenuManagementPage() => InitializeComponent();

        private void Temp_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadGeneralList(object sender, EventArgs e)
        {
            // GeneralMenu
            RegistryKey shell = RegistryUtils.GeneralMenu.GetShell;

            foreach (string subKeyName in shell.GetSubKeyNames())
                try
                {
                    GeneralList.Children.Add(
                        new ContextMenuInfoTab(shell.OpenSubKey(subKeyName, true), subKeyName));
                }
                catch (Exception exception)
                {
                    Err($"RegistryKey: {subKeyName} Can't be opened.\n",exception);
                }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadStarMenuList(object sender, EventArgs e)
        {
            // StarMenu
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadDesktopMenuList(object sender, EventArgs e)
        {
            // Directory\Background
            RegistryKey shell = RegistryUtils.Directory.Background.GetShell;

            foreach (string subKeyName in shell.GetSubKeyNames())
                try
                {
                    DesktopBackgroundList.Children.Add(
                        new ContextMenuInfoTab(shell.OpenSubKey(subKeyName, true), subKeyName));
                }
                catch (Exception exception)
                {
                    Err($"RegistryKey: {subKeyName} Can't be opened.\n",exception);
                }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadFolderMenuList(object sender, EventArgs e)
        {
            // TODO
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadDirMenuList(object sender, EventArgs e)
        {
            // Directory
            RegistryKey shell = RegistryUtils.Directory.GetShell;

            foreach (string subKeyName in shell.GetSubKeyNames())
                try
                {
                    DirectoryList.Children.Add(
                        new ContextMenuInfoTab(shell.OpenSubKey(subKeyName,true), subKeyName));
                }
                catch (Exception exception)
                {
                    Err($"RegistryKey: {subKeyName} Can't be opened.\n",exception);
                }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadIEMenuList(object sender, EventArgs e)
        {
            // HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\MenuExt
            RegistryKey menuExt =
                Registry.CurrentUser.OpenSubKey(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\MenuExt");

            if (menuExt == null) return;
            foreach (string subKeyName in menuExt.GetSubKeyNames())
                IEList.Children.Add(
                    new ContextMenuInfoTab(menuExt.OpenSubKey(subKeyName, true), subKeyName));
        }

        private void ContextMenuManagerPage_OnInitialized(object sender, EventArgs e)
        {
            //
        }
    }
}