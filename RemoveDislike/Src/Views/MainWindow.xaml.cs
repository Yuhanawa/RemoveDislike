using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RemoveDislike.Core;
using RemoveDislike.Views.Pages;

namespace RemoveDislike.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public delegate void Delegate();
        public static MainWindow Interface { get; private set; }

        public static Thread MainThread { get; set; } = Thread.CurrentThread;
        private static readonly Thread I0Thread = new(() =>
            {
                while (true)
                {
                    if (DelegateList.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            var fn = (Delegate)DelegateList[0].Clone();
                            DelegateList.RemoveAt(0); 
                            fn();
                        });
                    }
                    Thread.Sleep(2500);
                }
                // ReSharper disable once FunctionNeverReturns
            }) { Name = "I0Thread" };
        public static List<Delegate> DelegateList { get; set; } = new();
        private Page _clearPage;
        private Page _contextMenuManagerPage;

        public MainWindow()
        {
            InitializeComponent();
            Interface = this;

            Thread.CurrentThread.Name = "MainThread";
            I0Thread.Start();

            Application.Current.Exit += (_, _) => I0Thread.Abort();
        }

        private void PinButtonOnClick(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;

            Pin.Foreground = Topmost
                ? new SolidColorBrush(Colors.MintCream)
                : new SolidColorBrush(Color.FromRgb(140, 140, 140));
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainFrame.Width = e.NewSize.Width - 255;
            MainFrame.Height = e.NewSize.Height - 8;
        }

        private void CommonClear_OnSelected(object sender, RoutedEventArgs e) =>
            MainFrame.Content = _clearPage;

        private void ContextMenuManager_OnSelected(object sender, RoutedEventArgs e) =>
            MainFrame.Content = _contextMenuManagerPage;

        private void MainFrame_OnInitialized(object sender, EventArgs e)
        {
            _clearPage = new CleanPage();
            _contextMenuManagerPage = new ContextMenuManagerPage();

            CommonClear.IsSelected = true;
        }
    }
}