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
            Cleaner.CleanerGroup[RuleName].Run();
            CleanPage.Interface.State.Text = $"已清理： {Cleaner.AllCleanupSize} MB";
            CleanPage.Interface.ReLoad();
        }

        private void Del_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("你确认要删除这个文件吗", "此功能不可逆！", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Cleaner.CleanerGroup[RuleName].SelfDestruct();

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