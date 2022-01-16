using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using RemoveDislike.Core.Module;
using RemoveDislike.Views.Models;

namespace RemoveDislike.Views.Pages
{
    public partial class CleanupPage
    {
        public CleanupPage() => InitializeComponent();


        private void List_OnInitialized(object sender, EventArgs e) => LoadRuleItem();

        public void Refresh()
        {
            ItemPanel.Children.Clear();
            CleanupModule.ReLoad();
            LoadRuleItem();
        }

        private void LoadRuleItem()
        {
            foreach (RuleModule value in CleanupModule.RulesFileList.Values)
                ItemPanel.Children.Add(new CleanInfoTab(value)
                {
                    FontSize = 18,
                    Margin = new Thickness(4, 2, 4, 2),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                    Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
                });
        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
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
            foreach (CleanInfoTab child in ItemPanel.Children) child.Play();
        }


        private void More_OnClick(object sender, RoutedEventArgs e) => Refresh();

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