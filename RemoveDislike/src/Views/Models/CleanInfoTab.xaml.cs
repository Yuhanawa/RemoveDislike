using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using RemoveDislike.Core.Module;
using RemoveDislike.Views.Utils;

namespace RemoveDislike.Views.Models;

/// <summary>
/// </summary>
public partial class CleanInfoTab
{
    /// <summary>
    /// </summary>
    /// <param name="dataContext"></param>
    public CleanInfoTab(object dataContext)
    {
        DataContext = dataContext;
        InitializeComponent();
        TickTgBtn.IsChecked = !Rule.Danger;
    }

    /// <summary>
    /// </summary>
    private RuleModule Rule => (RuleModule)DataContext;

    private RuleModule GetRule() => Dispatcher.Invoke(() => Rule);

    /// <summary>
    ///     To run
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Play_OnClick(object sender, RoutedEventArgs e)
    {
        Play();
        SizeBlock.Text = Rule.SizeStr;
    }

    /// <summary>
    ///     Run
    /// </summary>
    public void Play() =>
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Dispatcher.Invoke(() => PlayBtn.IsEnabled = false);
            GetRule().Run(GetDisabledList(), _ => UpdateSize());
            Dispatcher.Invoke(() =>
            {
                PlayBtn.IsEnabled = true;
                UpdateSize();
            });
        });

    public void UpdateSize() => Dispatcher.Invoke(() => SizeBlock.Text = Rule.SizeStr);

    /// <summary>
    ///     Get Disabled List
    /// </summary>
    /// <returns></returns>
    public List<string> GetDisabledList() => Dispatcher.Invoke(
        () => (from CleanInfoTabItem child in SubPanel.Children
            where child.Tick.IsChecked.HasValue && !child.Tick.IsChecked.Value
            select child.SubRuleTextBlock.Text).ToList());

    /// <summary>
    ///     DelBtn Click Event -- Delete Selected Items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DelBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show(
                LangUtils.Get("Are you sure you want to delete this file"),
                LangUtils.Get("This function is not reversible!"),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            CleanupModule.RulesFileList.Remove(Rule.Name);

        ((CleanInfoTabItem)sender).Visibility = Visibility.Collapsed;
        sender = null;
    }

    /// <summary>
    ///     onMouseRightButtonUp modify SubPanel.Visibility
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ControlMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        SubPanel.Visibility = DelBtn.Visibility = PlayBtn.Visibility =
            SubPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        SizeBlock.Text = Rule.SizeStr == "0.00 B" ? "" : Rule.SizeStr;
    }

    /// <summary>
    ///     SubPanel children init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SubPanel_OnInitialized(object sender, EventArgs e)
    {
        foreach (string sub in Rule.SubRules.Keys)
            SubPanel.Children.Add(new CleanInfoTabItem(sub));
    }

    #region TickTgBtn onClick

    /// <summary>
    ///     OnChecked event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TickTgBtn_OnChecked(object sender, RoutedEventArgs e)
    {
        foreach (CleanInfoTabItem child in SubPanel.Children) child.Tick.IsChecked = true;
    }

    /// <summary>
    ///     UnChecked event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TickTgBtn_OnUnchecked(object sender, RoutedEventArgs e)
    {
        foreach (CleanInfoTabItem child in SubPanel.Children) child.Tick.IsChecked = false;
    }

    #endregion
}