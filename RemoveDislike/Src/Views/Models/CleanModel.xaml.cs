using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using RemoveDislike.Utils;

namespace RemoveDislike.Views.Models;

/// <summary>
/// </summary>
public partial class CleanInfoModel
{
    /// <summary>
    /// </summary>
    /// <param name="dataContext"></param>
    public CleanInfoModel(object dataContext)
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
        () => (from CleanInfoModelItem child in SubPanel.Children
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
            CleanupUtils.RulesFileList.Remove(Rule.Name);

        ((CleanInfoModelItem)sender).Visibility = Visibility.Collapsed;
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
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (string sub in Rule.SubRules.Keys)
        {
            CleanInfoModelItem item = new (sub)
            {
                Tick = { IsChecked = !Rule.Danger&&!sub.StartsWith('!'), }
            };
            item.Tick.Checked += (_, _) => RefreshStatus();
            item.Tick.Unchecked += (_, _) => RefreshStatus();
            SubPanel.Children.Add(item);
        }
         
        RefreshStatus();
    }
    
    private bool _passEvent;
    private void RefreshStatus()
    {
        bool all = true, any = false;
        foreach (CleanInfoModelItem child in SubPanel.Children)
        {
            if (any!=true&&child.Tick.IsChecked.HasValue && child.Tick.IsChecked.Value)
                any = true;
            if (all&&(!child.Tick.IsChecked.HasValue||!child.Tick.IsChecked.Value))
                all = false;
        }
        
        if (!_passEvent&&TickTgBtn.IsChecked!=(all||any))
        {
            _passEvent = true;
            TickTgBtn.IsChecked = all||any;
        }
       
        TickTgBtn.Content = all?"√":any?"?":"口";
    }
    
    #region TickTgBtn onClick

    /// <summary>
    ///     OnChecked event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TickTgBtn_OnChecked(object sender, RoutedEventArgs e)
    {
        if (!_passEvent)
        {
            foreach (CleanInfoModelItem child in SubPanel.Children)
            { _passEvent = true; child.Tick.IsChecked = true;}            
        }

        _passEvent = !_passEvent;
    }

    /// <summary>
    ///     UnChecked event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TickTgBtn_OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (!_passEvent)
        {
            foreach (CleanInfoModelItem child in SubPanel.Children)
            { _passEvent = true; child.Tick.IsChecked = false;}
        }
        
        _passEvent = !_passEvent;
    }

    #endregion
}