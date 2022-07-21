using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RemoveDislike.Views.Models;

public partial class InfoTab
{
    public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register("TitleText",
        typeof(string), typeof(InfoTab), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText",
        typeof(string), typeof(InfoTab), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(InfoTab), new PropertyMetadata(default(Brush)));

    public InfoTab() => InitializeComponent();
    public InfoTab(string title, string value) {
        InitializeComponent();
        TitleText = title;
        ValueText = value;
    }

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius) GetValue(Border.CornerRadiusProperty);
        set => SetValue(Border.CornerRadiusProperty, value);
    }

    public Brush BackgroundColor
    {
        get => (Brush)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }
}