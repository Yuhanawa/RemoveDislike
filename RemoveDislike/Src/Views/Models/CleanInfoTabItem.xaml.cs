namespace RemoveDislike.Views.Models
{
    public partial class CleanInfoTabItem
    {
        public CleanInfoTabItem(string subInfo)
        {
            InitializeComponent();
            SubRuleTextBlock.Text = subInfo;
        }
    }
}