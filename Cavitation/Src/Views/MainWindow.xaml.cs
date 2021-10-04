using System;
using System.Windows;
using System.Windows.Media;
using Cavitation.Core.Rule;

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
            // Thread.CurrentThread.Name = "MainThread";
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
    }
}