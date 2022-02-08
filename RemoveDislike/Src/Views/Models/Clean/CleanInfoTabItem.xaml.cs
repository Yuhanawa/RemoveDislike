namespace RemoveDislike.Views.Models.Clean;

public partial class CleanInfoTabItem
{
    public CleanInfoTabItem(string subInfo)
    {
        InitializeComponent();
        SubRuleTextBlock.Text = subInfo;
    }
}