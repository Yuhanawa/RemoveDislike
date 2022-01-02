using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using RemoveDislike.Core.Module;
using RemoveDislike.Views.Models;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Pages
{
    public partial class CleanupPage
    {
        public CleanupPage()
        {
            InitializeComponent();
            Interface = this;
        }

        public static CleanupPage Interface { get; private set; }

        private void List_OnInitialized(object sender, EventArgs e) => LoadRuleItem();

        public void Refresh()
        {
            RuleList.Children.Clear();
            LoadRuleItem();
        }

        private void LoadRuleItem()
        {
            foreach (RuleFile value in CleanupModule.RulesFileList.Values)
                RuleList.Children.Add(new CleanInfoTab(value)
                {
                    FontSize = 18,
                    Margin = new Thickness(4, 2, 4, 2),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                    Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
                });
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog
                { Filter = "Json|*.json", Multiselect = true, DefaultExt = "*.json" };

            bool? result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                foreach (string name in fileDialog.FileNames)
                {
                    FileInfo fileInfo = new(name);
                    if (!File.Exists($"{ConfigHelper.RuleBase}/{fileInfo.Name}"))
                        fileInfo.CopyTo($"{ConfigHelper.RuleBase}/{fileInfo.Name}");
                    else
                    {
                        MessageBox.Show($"{ConfigHelper.RuleBase}/{fileInfo.Name} Is not Exists");
                        Warn($"[View] {ConfigHelper.RuleBase}/{fileInfo.Name} Is not Exists");
                    }
                }
            }

            Refresh();
        }

        private void OpenFolder_OnClick(object sender, RoutedEventArgs e) =>
            new Process
            {
                StartInfo =
                {
                    FileName = "explorer.exe",
                    Arguments = ConfigHelper.RuleBase
                }
            }.Start();

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            State.Text = LangUtils.Get("Cleaning up... It may take a long time... Please wait...");
            State.Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191));

            new Thread(ClearAll) { Name = "CleanupThread", IsBackground = true }.Start();
        }

        private async void ClearAll()
        {
            int count = CleanupModule.RulesFileList.Count;

            UIElementCollection children = null;
            MainWindow.Interface.Dispatcher.Invoke(() => { children = RuleList.Children; });

            for (var i = 0; i < children.Count; i++)
            {
                var tab = (CleanInfoTab)children[i];
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    CleanupModule.Run(((RuleFile)tab.DataContext).Path);
                    count--;
                });
            }

            while (count > 0)
                await MainWindow.Interface.Dispatcher.Invoke(async () =>
                {
                    State.Text =
                        $"{LangUtils.Get("Cleaning up... Currently cleaned up:")} {CleanupModule.TotalSizeStr}";
                    Refresh();
                    await Task.Delay(5000);
                });

            MainWindow.Interface.Dispatcher.Invoke(() =>
            {
                State.Text = $"{LangUtils.Get("Cleaned up")} {CleanupModule.TotalSizeStr}";
                Refresh();
            });
        }

        private void More_OnClick(object sender, RoutedEventArgs e) =>
            Refresh();

        private void CleanPage_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length == 0)
                return;

            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                if (!File.Exists($@"{ConfigHelper.RuleBase}/{fileInfo.Name}"))
                    fileInfo.CopyTo($@"{ConfigHelper.RuleBase}/{fileInfo.Name}");
                else
                {
                    MessageBox.Show($@"{ConfigHelper.RuleBase}/{fileInfo.Name} Is not Exists");
                    Info($"[View] {ConfigHelper.RuleBase}/{fileInfo.Name} Is not Exists");
                }
            }

            Refresh();
        }
    }
}