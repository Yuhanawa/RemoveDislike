using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Models.ContextMenu;

public partial class ContextMenuInfoTab
{
    public static readonly DependencyProperty KeyNameProperty =
        DependencyProperty.Register(nameof(KeyName), typeof(string), typeof(ContextMenuInfoTab));

    public ContextMenuInfoTab(RegistryKey registryKey, string keyName, bool isEx = false)
    {
        InitializeComponent();

        RegistryKey = registryKey;
        KeyName = keyName;
        IsEx = isEx;

        DataContext = this;
    }

    public RegistryKey RegistryKey { get; set; }

    private static BitmapImage DllIcon =>
        new(new Uri("pack://application:,,,/RemoveDislike;component/Resources/Img/dll.png"));

    private static BitmapImage DefaultIcon =>
        new(new Uri("pack://application:,,,/RemoveDislike;component/Resources/Img/default.png"));


    public bool IsEx { get; set; }

    public bool Enabled
    {
        get
        {
            if (IsEx)
            {
                if (DefaultValue == "") return false;

                return DefaultValue[DefaultValue.Length - 1 - 1] != '-';
            }

            return !LegacyDisable;
        }
        set
        {
            if (IsEx)
            {
                if (Enabled == value || DefaultValue == "") return;
                DefaultValue = Enabled
                    ? DefaultValue.Insert(DefaultValue.Length - 1, "-")
                    : DefaultValue.Remove(DefaultValue.Length - 1 - 1, 1);
            }
            else
            {
                if (Enabled == value) return;
                LegacyDisable = !value;
            }
        }
    }

    public string DefaultValue
    {
        get => RegistryKey.GetValue("") == null ? "" : RegistryKey.GetValue("")!.ToString();
        set => RegistryKey.SetValue("", value);
    }


    public bool LegacyDisable
    {
        get => RegistryKey.GetValueNames().Contains("LegacyDisable");
        set
        {
            if (value) RegistryKey.SetValue("LegacyDisable", "");
            else RegistryKey.DeleteValue("LegacyDisable");
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
                (path.Contains(".exe", StringComparison.CurrentCultureIgnoreCase) && path.Contains(',')))
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

    private void SelectIcon()
    {
        OpenFileDialog opd = new()
        {
            Multiselect = false,
            FilterIndex = 1,
            Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*",
            Title = LangUtils.Get("Select an image")
        };
        if (opd.ShowDialog() == true) RegistryKey.SetValue("Icon", opd.FileName);
    }

    #region Event

    private void EIcon_OnDragEnter(object sender, DragEventArgs e) =>
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

    private void EIcon_OnDrop(object sender, DragEventArgs e)
    {
        try
        {
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once AssignNullToNotNullAttribute
            RegistryKey.SetValue("Icon", ((string[])e.Data.GetData(DataFormats.FileDrop))!.GetValue(0).ToString());
        }
        catch (Exception ex)
        {
            Err("Err", ex);
        }
    }

    private void EIcon_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => SelectIcon();

    private void ControlOnMouseRightDown(object sender, MouseButtonEventArgs e) =>
        SubMenu.Visibility = SubMenu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;


    #region Menu

    private void ChangeIconButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => SelectIcon();

    private void DeleteIconButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (MessageBox.Show(LangUtils.Get("Are you sure you want to delete the icon?"),
                LangUtils.Get("Delete icon"), MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            RegistryKey.SetValue("Icon", "");
    }

    // TODO: HELP NEEDED RegJump
    private void OpenRegButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) =>
        MessageBox.Show("RegJump \"{RegistryKey}\"", "I can not do it", MessageBoxButton.OK,
            MessageBoxImage.Information);

    private void DeleteButton_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (MessageBox.Show(LangUtils.Get("Are you sure you want to delete the icon?"),
                LangUtils.Get("Delete icon"), MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
            MessageBoxResult.Yes) return;

        RegistryKey.DeleteValue("");
        RegistryKey.Close();
        Visibility = Visibility.Collapsed;
        IsEnabled = false;
        GC.Collect();
    }

    #endregion

    #endregion Event
}