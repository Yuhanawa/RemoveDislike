using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RemoveDislike.Views.Models.RegistryManger;

namespace RemoveDislike.Views.Pages.RegistryPage;

public partial class RegistryManagerPage : Page
{
    public RegistryManagerPage() => InitializeComponent();

    private void Panel_OnInitialized(object sender, EventArgs e)
    {
        AddItem("Test", "Test", () => { return false; }, b => { });
        AddItem("Test", "Test", () => { return false; }, b => { });
        AddItem("Test", "Test", () => { return false; }, b => { });
    }


    private void AddItem(string name, string description, RegistryMangerItem.GetRegValueDelegate getRegValue,
        Action<bool> setValue) =>
        Panel.Children.Add(new RegistryMangerItem(
                name,
                description,
                getRegValue,
                setValue)
            {
                FontSize = 18,
                Margin = new Thickness(4, 2, 4, 2),
                BorderBrush = new SolidColorBrush(Color.FromArgb(16, 220, 220, 220)),
                Foreground = new SolidColorBrush(Color.FromRgb(191, 191, 191))
            }
        );
}