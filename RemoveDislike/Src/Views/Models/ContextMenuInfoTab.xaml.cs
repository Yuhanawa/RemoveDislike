using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace RemoveDislike.Views.Models
{
    public partial class ContextMenuInfoTab
    {
        public static readonly DependencyProperty KeyNameProperty =
            DependencyProperty.Register("KeyName", typeof(string), typeof(ContextMenuInfoTab));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ContextMenuInfoTab));

        public ContextMenuInfoTab(RegistryKey key, string name)
        {
            InitializeComponent();
            DataContext = key;
            KeyName = name;

            var path = key.GetValue("Icon")?.ToString();
            if (path == null)
            {
                // TODO (null value)
            }
            else if (path.EndsWith(".exe"))
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(path);

                if (icon == null) return;

                ImageSource img = Imaging.CreateBitmapSourceFromHBitmap(
                    icon.ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                Icon = img;
            }
            else if (path.Contains(".exe") && path.Contains(","))
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(path.Split(',')[0]);

                if (icon == null) return;

                ImageSource img = Imaging.CreateBitmapSourceFromHBitmap(
                    icon.ToBitmap().GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                Icon = img;
            }
            else
            {
                Icon = new BitmapImage(new Uri(path));
            }
        }

        public bool TabVisibility
        {
            get => !((RegistryKey)DataContext).GetValueNames().Contains("LegacyDisable");
            set
            {
                if (value)
                    ((RegistryKey)DataContext).DeleteValue("LegacyDisable");
                else
                    ((RegistryKey)DataContext).SetValue("LegacyDisable", "");
            }
        }

        public string KeyName
        {
            get => (string)GetValue(KeyNameProperty);
            set => SetValue(KeyNameProperty, value);
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

        private void EIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void EText_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void ESub_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void EVisibility_OnClick(object sender, RoutedEventArgs e)
        {
            TabVisibility = !TabVisibility;

            // Maybe you cannot see it!
            EVisibility.Content = TabVisibility ? "" : "";
        }

        private void EDel_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void EMore_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void EIcon_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            //仅支持文件的拖放
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            //获取拖拽的文件
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length == 0) return;

            foreach (string file in files)
                try
                {
                    // TODO
                }
                catch (Exception ex)
                {
                    Err("Err", ex);
                    return;
                }
        }

        #endregion Event
    }
}