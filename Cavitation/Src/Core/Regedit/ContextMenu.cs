using System;
using System.Collections.Generic;
using Cavitation.Core.Utils;
using Microsoft.Win32;


// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

//!!! Dont do Format or Cleanup !!!

namespace Cavitation.Core.Regedit
{
    public class ContextMenu
    {
        public static RegistryKey GetRoot() => Registry.ClassesRoot;
        public static RegistryKey GetGeneralMenu() => Registry.ClassesRoot.OpenSubKey(@"*");
        public static RegistryKey GetDirectory() => Registry.ClassesRoot.OpenSubKey(@"Directory");

        public static class GeneralMenu
        {
            public static RegistryKey GetGeneralMenu() => Registry.ClassesRoot.OpenSubKey(@"*");
            public static RegistryKey GetOpenWithList() => Registry.ClassesRoot.OpenSubKey(@"*\OpenWithList");
            public static RegistryKey GetShell() => Registry.ClassesRoot.OpenSubKey(@"*\shell");
            public static RegistryKey GetShellex() => Registry.ClassesRoot.OpenSubKey(@"*\shellex");
            public static RegistryKey Get_shellex() => Registry.ClassesRoot.OpenSubKey(@"*\-shellex");
        }

        public static class Directory
        {
            public static RegistryKey GetDirectory() => Registry.ClassesRoot.OpenSubKey(@"Directory");
            public static RegistryKey GetBackground() => Registry.ClassesRoot.OpenSubKey(@"Directory\Background");
            public static class Background
            {
                public static RegistryKey GetBackground() => Registry.ClassesRoot.OpenSubKey(@"Directory\Background");
                public static RegistryKey GetShell() => Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell");
                public static RegistryKey GetShellex() => Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shellex");
                public static RegistryKey Get_shellex() => Registry.ClassesRoot.OpenSubKey(@"Directory\Background\-shellex");                
            }
            public static RegistryKey GetDefaultIcon() => Registry.ClassesRoot.OpenSubKey(@"Directory\DefaultIcon");
            public static RegistryKey GetShell() => Registry.ClassesRoot.OpenSubKey(@"Directory\shell");
            public static RegistryKey GetShellex() => Registry.ClassesRoot.OpenSubKey(@"Directory\shellex");
            public static RegistryKey Get_shellex() => Registry.ClassesRoot.OpenSubKey(@"Directory\-shellex");
        }
    }
}