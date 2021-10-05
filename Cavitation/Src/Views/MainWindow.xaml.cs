using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Cavitation.Core.Cleaner.Rule;

namespace Cavitation.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Interface;

        public MainWindow()
        {
            InitializeComponent();
            Interface = this;
            
            Thread.CurrentThread.Name = "MainThread";
        }

        private void PinButtonOnClick(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;

            Pin.Foreground = Topmost
                ? new SolidColorBrush(Colors.MintCream)
                : new SolidColorBrush(Color.FromRgb(140, 140, 140));
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            if (RulesGroups.Interface == null)
            {
                RulesGroups rulesGroups = new();
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainFrame.Width = e.NewSize.Width - 255;
            MainFrame.Height = e.NewSize.Height - 8;
        }
    }
}