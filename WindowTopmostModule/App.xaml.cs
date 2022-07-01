using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using fastJSON;
using NHotkey;
using NHotkey.Wpf;

#pragma warning disable CS8618

namespace WindowTopmostModule
{
    /// <summary>
    ///     Topmost the active window.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public partial class App
    {
        public App()
        {
            Mutex _ =
                new(true, "WindowTopmostModule", out bool isNotRunning);
            if (!isNotRunning) Environment.Exit(1);

            foreach (Hotkey hotkey in Config)
                HotkeyManager.Current.AddOrReplace("WindowTopmostModule", hotkey.Key,
                    hotkey.ModifierKeys.Aggregate(ModifierKeys.None,
                        (current, k) => current | k), ToTopmost);
        }

        private static HashSet<IntPtr> Set { get; } = new();
        
        private static void ToTopmost(object? sender, HotkeyEventArgs e)
        {
            IntPtr i = GetForegroundWindow();

            SetWindowPos(i, Set.Remove(i) || !Set.Add(i) ? new IntPtr(-2) : new IntPtr(-1),
                0, 0, 0, 0, 0x0002 | 0x0001);
        }

        #region Config

        private static List<Hotkey> Config { get; } = LoadConfig();

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Hotkey
        {
            public Key Key { get; set; }

            // ReSharper disable once CollectionNeverUpdated.Local
            public List<ModifierKeys> ModifierKeys { get; set; }
        }

        private static List<Hotkey> LoadConfig() =>
            JSON.ToObject<List<Hotkey>>(
                File.ReadAllText(Environment.GetEnvironmentVariable("APPDATA") + "/RemoveDislike/Modules/WindowTopmost.json"));

        #endregion

        #region WinApi

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        #endregion
    }
}