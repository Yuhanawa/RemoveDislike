using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Cavitation.Core;
using Cavitation.Views.Pages;

namespace Cavitation.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Interface;
        
        public static Thread MainThread;
        public static Thread I0Thread;
        
        public delegate void Delegate();
        public static List<Delegate> DelegateList;

        public MainWindow()
        {
            Entrance.Init();
            InitializeComponent();
            Interface = this;

            MainThread = Thread.CurrentThread;
            Thread.CurrentThread.Name = "MainThread";
            
            DelegateList = new List<Delegate>();

            I0Thread = new Thread(
                start: () => {
                    while (true) {
                        if (DelegateList.Count == 0)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        DelegateList[0]();
                        DelegateList.RemoveAt(0);

                    }}){ Name = "I0Thread" };
            
            I0Thread.Start();

            Application.Current.Exit += (sender, args) => I0Thread.Abort();
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
        private Page _clearPage;
        private Page _contextMenuManagerPage;

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