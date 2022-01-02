using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RemoveDislike.Core.Module;
using RemoveDislike.Views.Pages;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Models
{
    public partial class CleanInfoTab
    {
        public CleanInfoTab(object dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            CleanupPage.Interface.State.Text =
                LangUtils.Get("Cleaning up... It may take a long time... Please wait...");
            Clear(((RuleFile)DataContext).Path);
        }

        private static async void Clear(string key)
        {
            var flag = false;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                CleanupModule.Run(key);
                MainWindow.Interface.Dispatcher.Invoke(() =>
                    CleanupPage.Interface.State.Text =
                        $"{LangUtils.Get("Cleaning up... Currently cleaned up:")} {CleanupModule.TotalSizeStr}");
                flag = true;
            });

            do
            {
                await Task.Delay(2000);
            } while (!flag);

            MainWindow.Interface.Dispatcher.Invoke(() =>
            {
                CleanupPage.Interface.State.Text = $"{LangUtils.Get("Cleaned up:")} {CleanupModule.TotalSizeStr}";
                CleanupPage.Interface.Refresh();
            });
        }

        private void Del_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                LangUtils.Get("Are you sure you want to delete this file"),
                LangUtils.Get("This function is not reversible!"),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                CleanupModule.RulesFileList.Remove(((RuleFile)DataContext).Name);

            CleanupPage.Interface.Refresh();
        }

        private void ControlMouseRightButtonUp(object sender, MouseButtonEventArgs e) =>
            SubPanel.Visibility =
                SubPanel.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;

        private void SubPanel_OnInitialized(object sender, EventArgs e)
        {
            foreach (string sub in ((RuleFile)DataContext).SubRules)
                SubPanel.Children.Add(new CleanInfoTabItem(sub));
            
        }
    }
}