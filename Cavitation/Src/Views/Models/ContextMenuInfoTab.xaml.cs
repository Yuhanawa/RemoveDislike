using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cavitation.Views.Models
{
    public partial class ContextMenuInfoTab
    {
        public ContextMenuInfoTab(bool tabVisibility)
        {
            TabVisibility = tabVisibility;
            InitializeComponent();
        }

        public bool TabVisibility;
        
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(ContextMenuInfoTab));
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ContextMenuInfoTab));


        #region Event

        

        private void EIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void EText_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ESub_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void EVisibility_OnClick(object sender, RoutedEventArgs e)
        {
            TabVisibility = !TabVisibility;
            
            // Maybe you cannot see it!
            EVisibility.Content = TabVisibility ? "" : "";
        }

        private void EDel_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void EMore_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

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
                {
                    try
                    {
                        
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }
        #endregion

        private void ContextMenuInfoTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            EVisibility.Content = TabVisibility ? "" : "";
        }
    }
}