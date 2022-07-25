namespace RemoveDislike.Views.Models;

public partial class CleanInfoModelItem
{
    public CleanInfoModelItem(string subInfo)
    {
        InitializeComponent();
        SubRuleTextBlock.Text = subInfo;
    }
}