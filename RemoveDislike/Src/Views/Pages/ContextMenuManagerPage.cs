using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.Win32;
using RemoveDislike.Core.Utils;
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
            // TODO
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadGeneralList(object sender, EventArgs e)
        {
            // GeneralMenu
            RegistryKey shell = RegistryUtils.GeneralMenu.GetShell;
            RegistryKey shellex = RegistryUtils.GeneralMenu.GetShellexcmh;
            RegistryKey _shellex = RegistryUtils.GeneralMenu.Get_shellexcmh;

            foreach (string subKeyName in shell.GetSubKeyNames())
                GeneralShellList.Children.Add(
                    new ContextMenuInfoTab(shell.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in shellex.GetSubKeyNames())
                GeneralShellExList.Children.Add(
                    new ContextMenuInfoTab(shellex.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in _shellex.GetSubKeyNames())
                General_ShellExList.Children.Add(
                    new ContextMenuInfoTab(_shellex.OpenSubKey(subKeyName, true), subKeyName));
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
            RegistryKey shellex = RegistryUtils.Directory.Background.GetShellexcmh;
            RegistryKey _shellex = RegistryUtils.Directory.Background.Get_shellexcmh;

            foreach (string subKeyName in shell.GetSubKeyNames())
                DesktopShellList.Children.Add(
                    new ContextMenuInfoTab(shell.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in shellex.GetSubKeyNames())
                DesktopShellExList.Children.Add(
                    new ContextMenuInfoTab(shellex.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in _shellex.GetSubKeyNames())
                Desktop_ShellExList.Children.Add(
                    new ContextMenuInfoTab(_shellex.OpenSubKey(subKeyName, true), subKeyName));
        }


        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadFileMenuList(object sender, EventArgs e)
        {
            // TODO
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void LoadDirMenuList(object sender, EventArgs e)
        {
            // Directory
            RegistryKey shell = RegistryUtils.Directory.GetShell;
            RegistryKey shellex = RegistryUtils.Directory.GetShellexcmh;
            RegistryKey _shellex = RegistryUtils.Directory.Get_shellexcmh;

            foreach (string subKeyName in shell.GetSubKeyNames())
                DirShellList.Children.Add(
                    new ContextMenuInfoTab(shell.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in shellex.GetSubKeyNames())
                DirShellExList.Children.Add(
                    new ContextMenuInfoTab(shellex.OpenSubKey(subKeyName, true), subKeyName));

            foreach (string subKeyName in _shellex.GetSubKeyNames())
                Dir_ShellExList.Children.Add(
                    new ContextMenuInfoTab(_shellex.OpenSubKey(subKeyName, true), subKeyName));
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
    }
}