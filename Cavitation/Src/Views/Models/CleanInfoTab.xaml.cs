using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cavitation.Core.Clean;
using Cavitation.Views.Pages;

namespace Cavitation.Views.Models
{
    public partial class CleanInfoTab
    {
        public CleanInfoTab()
        {
            InitializeComponent();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            CleanPage.Interface.State.Text = "清理中... 可能时间较长... 请稍等...";
            string key = RuleName.ToString();
            MainWindow.DelegateList.Add( () => Clear(key));
        }
        
        private static async void Clear(string key)
        {
            var flag = false;
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    Cleaner.Run(key);
                    MainWindow.Interface.Dispatcher.Invoke(() =>
                        CleanPage.Interface.State.Text = $"清理中... 目前已清理： {Cleaner.AllCleanedSize} MB");
                    flag = true;
                });

                do { await Task.Delay(2000); } while (!flag);

            MainWindow.Interface.Dispatcher.Invoke(() =>
            {
                CleanPage.Interface.State.Text = $"已清理： {Cleaner.AllCleanedSize} MB";
                CleanPage.Interface.ReLoad();
            });
        }

        private void Del_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("你确认要删除这个文件吗", "此功能不可逆！", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Cleaner.Delete(RuleName);

            CleanPage.Interface.ReLoad();
        }

        private void More_OnClick(object sender, RoutedEventArgs e) =>
            // TODO add Secondary menu
            CleanPage.Interface.ReLoad();

        #region Property

        public static readonly DependencyProperty RuleNameProperty =
            DependencyProperty.Register("RuleName", typeof(string),
                typeof(CleanInfoTab), new PropertyMetadata("Unknown"));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string),
                typeof(CleanInfoTab), new PropertyMetadata("Unknown"));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double),
                typeof(CleanInfoTab), new PropertyMetadata(-1d));

        public string RuleName
        {
            get => (string)GetValue(RuleNameProperty);
            set => SetValue(RuleNameProperty, value);
        }


        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        #endregion
    }
}