using System;
using System.Windows;
using Cavitation.Core.Cleaner.Rule;

namespace Cavitation.Views.Models
{
    public partial class InfoTab
    {
        public static readonly DependencyProperty RuleNameProperty =
            DependencyProperty.Register("RuleName", typeof(string),
                typeof(InfoTab), new PropertyMetadata("Unknown"));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string),
                typeof(InfoTab), new PropertyMetadata("Unknown"));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double),
                typeof(InfoTab), new PropertyMetadata(-1d));

        public InfoTab()
        {
            InitializeComponent();
        }

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


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            RulesGroups.GetRulesGroup(RuleName).Clear();
        }

        private void Del_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void More_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}