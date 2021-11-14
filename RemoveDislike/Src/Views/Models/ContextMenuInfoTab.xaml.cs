using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RemoveDislike.Views.Models
{
    public partial class ContextMenuInfoTab
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ContextMenuInfoTab));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ContextMenuInfoTab));

        public bool TabVisibility;

        public ContextMenuInfoTab(bool tabVisibility)
        {
            TabVisibility = tabVisibility;
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private void ContextMenuInfoTab_OnLoaded(object sender, RoutedEventArgs e) =>
            // Maybe you can't see it 
            EVisibility.Content = TabVisibility ? "" : "";


        #region Event

        private void EIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) =>
            throw new NotImplementedException();

        private void EText_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e) =>
            throw new NotImplementedException();

        private void ESub_OnClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();

        private void EVisibility_OnClick(object sender, RoutedEventArgs e)
        {
            TabVisibility = !TabVisibility;

            // Maybe you cannot see it!
            EVisibility.Content = TabVisibility ? "" : "";
        }

        private void EDel_OnClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();

        private void EMore_OnClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();

        private void EIcon_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            {
                //仅支持文件的拖放
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                    return;

                //获取拖拽的文件
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files == null || files.Length == 0)
                    return;

                foreach (string file in files)
                    try
                    {
                    }
                    catch
                    {
                        return;
                    }
            }
        }

        #endregion
    }
}