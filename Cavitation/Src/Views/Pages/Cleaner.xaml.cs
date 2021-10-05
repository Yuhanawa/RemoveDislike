using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Cavitation.Core.Cleaner.Rule;
using Cavitation.Core.Rule;
using Cavitation.Views.Models;

namespace Cavitation.Views.Pages
{
    public partial class Cleaner
    {
        public Cleaner()
        {
            InitializeComponent();
        }

        private void List_OnInitialized(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, RulesModel> keyValuePair in RulesGroups.GetRulesGroups())
                RuleList.Children.Add(new InfoTab
                {
                    RuleName = keyValuePair.Key,
                    Source = keyValuePair.Value.Source,
                    Size = keyValuePair.Value.CleanupSize,
                    FontSize = 18,
                    // Margin = new Thickness(0),
                    Margin = new Thickness(4, 2, 4, 2),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                    Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
                });
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenFolder_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (string key in RulesGroups.GetRulesGroups().Keys)
            {
                RulesGroups.GetRulesGroup(key).Clear();
            }
        }

        private void More_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}