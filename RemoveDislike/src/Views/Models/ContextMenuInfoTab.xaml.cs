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

        public ContextMenuInfoTab(RegistryKey registryKey, string keyName)
        {
            InitializeComponent();

            RegistryKey = registryKey;
            KeyName = keyName;

            DataContext = this;
        }

        public RegistryKey RegistryKey { get; set; }

        private static BitmapImage DllIcon =>
            new(new Uri("pack://application:,,,/RemoveDislike;component/Resources/Img/dll.png"));

        private static BitmapImage DefaultIcon =>
            new(new Uri("pack://application:,,,/RemoveDislike;component/Resources/Img/default.png"));

        public bool LegacyDisable
        {
            get => !RegistryKey.GetValueNames().Contains("LegacyDisable");
            set
            {
                if (value) RegistryKey.DeleteValue("LegacyDisable");
                else RegistryKey.SetValue("LegacyDisable", "");
            }
        }

        public bool Extended
        {
            get => RegistryKey.GetValueNames().Contains("Extended");
            set
            {
                if (value) RegistryKey.SetValue("Extended", "");
                else RegistryKey.DeleteValue("Extended");
            }
        }

        public string KeyName
        {
            get => (string)GetValue(KeyNameProperty);
            set => SetValue(KeyNameProperty, value);
        }

        public ImageSource Icon
        {
            get
            {
                var path = RegistryKey.GetValue("Icon")?.ToString();

                if (string.IsNullOrEmpty(path)) return DefaultIcon;

                if (path.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase) ||
                    path.Contains(".exe", StringComparison.CurrentCultureIgnoreCase) && path.Contains(','))
                {
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(path.Split(',')[0]);

                    return icon == null
                        ? DefaultIcon
                        : Imaging.CreateBitmapSourceFromHBitmap(
                            icon.ToBitmap().GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                }

                if (path.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)) return DllIcon;
                if (path.EndsWith(".ico", StringComparison.CurrentCultureIgnoreCase))
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                if (path.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                if (path.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase))
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                if (path.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (path.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
                    return new BitmapImage(new Uri(path, UriKind.Absolute));

                return DefaultIcon;
            }
        }

        #region Event

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
                    // TODO add support for .ico, .png, .jpg, .bmp, .gif
                }
                catch (Exception ex)
                {
                    Err("Err", ex);
                    return;
                }
        }

        private void ControlOnMouseRightDown(object sender, MouseButtonEventArgs e) =>
            MenuGrid.Visibility = MenuGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        #endregion Event
    }
}