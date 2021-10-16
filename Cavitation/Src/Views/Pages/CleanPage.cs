using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cavitation.Core;
using Cavitation.Core.Clean;
using Cavitation.Views.Models;
using Microsoft.Win32;
using static Cavitation.Views.Utils.CommonUtils;

namespace Cavitation.Views.Pages
{
    public partial class CleanPage
    {
        public static CleanPage Interface;

        public CleanPage()
        {
            InitializeComponent();
            
            Interface = this;
        }

        private void List_OnInitialized(object sender, EventArgs e) => LoadRuleItem();

        public void ReLoad() => ReLoad(false);

        public void ReLoad(bool all)
        {
            if (all)
                Entrance.Cleaner.ReLoad();

            RuleList.Children.Clear();
            LoadRuleItem();
        }

        private void LoadRuleItem()
        {
            foreach (string key in Cleaner.CleanerGroup.Keys)
                RuleList.Children.Add(new CleanInfoTab
                {
                    RuleName = key,
                    Source = Cleaner.CleanerGroup[key].Source == "Internal" ? "Internal" : "External",
                    Size = Cleaner.CleanerGroup[key].CleanupSize,
                    FontSize = 18,
                    // Margin = new Thickness(0),
                    Margin = new Thickness(4, 2, 4, 2),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                    Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
                });
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog
                { Filter = "CleanRules|*.cr", Multiselect = true, DefaultExt = "*.rc" };

            // ReSharper disable once PossibleInvalidOperationException
            if (fileDialog.ShowDialog().Value)
                foreach (string name in fileDialog.FileNames)
                {
                    FileInfo fileInfo = new(name);
                    if (!File.Exists($@"{Config.RulesGroupsPath}/{fileInfo.Name}"))
                    {
                        fileInfo.CopyTo($@"{Config.RulesGroupsPath}/{fileInfo.Name}");
                    }
                    else
                    {
                        MessageBox.Show($@"{Config.RulesGroupsPath}/{fileInfo.Name} Is Exists");
                        Log($@"{Config.RulesGroupsPath}/{fileInfo.Name} Is Exists");
                    }
                }

            ReLoad();
        }

        private void OpenFolder_OnClick(object sender, RoutedEventArgs e) =>
            new Process
            {
                StartInfo =
                {
                    FileName = "explorer.exe",
                    Arguments = Config.RulesGroupsPath
                }
            }.Start();

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            Cleaner.Run();
            State.Text = $"已清理： {Cleaner.AllCleanupSize} MB";
            ReLoad();
        }

        private void More_OnClick(object sender, RoutedEventArgs e) =>
            // TODO add Secondary menu
            ReLoad();

        private void CleanPage_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            //仅支持文件的拖放
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            //获取拖拽的文件
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length == 0)
                return;

            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                if (!File.Exists($@"{Config.RulesGroupsPath}/{fileInfo.Name}"))
                {
                    fileInfo.CopyTo($@"{Config.RulesGroupsPath}/{fileInfo.Name}");
                }
                else
                {
                    MessageBox.Show($@"{Config.RulesGroupsPath}/{fileInfo.Name} Is Exists");
                    Log($@"{Config.RulesGroupsPath}/{fileInfo.Name} Is Exists");
                }
            }
            ReLoad();
        }
    }
}