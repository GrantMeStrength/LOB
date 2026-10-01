using DashboardAndDailyTasks.Models;
using DashboardAndDailyTasks.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class CustomersPage : Page
{
    private bool _isSynchronizingFilters;

    public CustomersViewModel ViewModel { get; } = new();

    public CustomersPage()
    {
        InitializeComponent();
        ViewSelector.SelectedItem = TableSelectorItem;
        UpdateSortIndicators();
        App.State.Changed += State_Changed;
        Unloaded += (_, _) => App.State.Changed -= State_Changed;
    }

    private void State_Changed(object? sender, EventArgs e) => ViewModel.RefreshFromState();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        switch (e.Parameter)
        {
            case CustomerFilter.Active:
                SetStatusFilter("Active", true);
                break;
            case CustomerFilter.UpcomingRenewals:
                ViewModel.SelectedRenewalFilter = "Next 30 days";
                break;
        }
    }

    private void ViewSelector_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args)
    {
        bool showCards = sender.SelectedItem == CardsSelectorItem;
        TableView.Visibility = showCards ? Visibility.Collapsed : Visibility.Visible;
        CardsView.Visibility = showCards ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) =>
        ViewModel.SearchText = ((TextBox)sender).Text;

    private void ResetFilters_Click(object sender, RoutedEventArgs e)
    {
        _isSynchronizingFilters = true;
        try
        {
            ViewModel.ResetFilters();
            SearchBox.Text = "";
            SetMenuFlyoutItemsUnchecked(StatusFilterMenu);
            SetMenuFlyoutItemsUnchecked(RegionFilterMenu);
        }
        finally
        {
            _isSynchronizingFilters = false;
        }
        UpdateSortIndicators();
    }

    private void StatusFilterItem_Click(object sender, RoutedEventArgs e)
    {
        if (!_isSynchronizingFilters &&
            sender is ToggleMenuFlyoutItem { Tag: string status } item)
        {
            ViewModel.SetStatusFilter(status, item.IsChecked);
        }
    }

    private void SetStatusFilter(string status, bool isSelected)
    {
        if (_isSynchronizingFilters)
            return;

        _isSynchronizingFilters = true;
        try
        {
            ViewModel.SetStatusFilter(status, isSelected);
            ToggleMenuFlyoutItem? item = StatusFilterMenu.Items
                .OfType<ToggleMenuFlyoutItem>()
                .FirstOrDefault(candidate => Equals(candidate.Tag, status));
            if (item is not null)
                item.IsChecked = isSelected;
        }
        finally
        {
            _isSynchronizingFilters = false;
        }
    }

    private void RegionFilterItem_Click(object sender, RoutedEventArgs e)
    {
        if (!_isSynchronizingFilters &&
            sender is ToggleMenuFlyoutItem { Tag: string region } item)
        {
            ViewModel.SetRegionFilter(region, item.IsChecked);
        }
    }

    private static void SetMenuFlyoutItemsUnchecked(MenuFlyout menu)
    {
        foreach (ToggleMenuFlyoutItem item in menu.Items.OfType<ToggleMenuFlyoutItem>())
            item.IsChecked = false;
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { SelectedItem: string sort })
            ViewModel.SelectedSort = sort;
        UpdateSortIndicators();
    }

    private void TableHeader_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string column })
            ViewModel.ToggleSort(column);
        UpdateSortIndicators();
    }

    private void UpdateSortIndicators()
    {
        if (CompanySortIcon is null)
            return;

        CompanySortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Company") ? Visibility.Visible : Visibility.Collapsed;
        ContactSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Contact") ? Visibility.Visible : Visibility.Collapsed;
        OwnerSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Owner") ? Visibility.Visible : Visibility.Collapsed;
        RegionSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Region") ? Visibility.Visible : Visibility.Collapsed;
        StatusSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Status") ? Visibility.Visible : Visibility.Collapsed;
        RenewalSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Renewal") ? Visibility.Visible : Visibility.Collapsed;
        ActivitySortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Last activity") ? Visibility.Visible : Visibility.Collapsed;
        ValueSortIcon.Visibility = ViewModel.SelectedSort.StartsWith("Account value") ? Visibility.Visible : Visibility.Collapsed;

        CompanySortIcon.Glyph = ViewModel.SelectedSort == "Company name (Z-A)" ? "\uE70D" : "\uE70E";
        ContactSortIcon.Glyph = ViewModel.SelectedSort == "Contact name (Z-A)" ? "\uE70D" : "\uE70E";
        OwnerSortIcon.Glyph = ViewModel.SelectedSort == "Owner name (Z-A)" ? "\uE70D" : "\uE70E";
        RegionSortIcon.Glyph = ViewModel.SelectedSort == "Region (Z-A)" ? "\uE70D" : "\uE70E";
        StatusSortIcon.Glyph = ViewModel.SelectedSort == "Status (Z-A)" ? "\uE70D" : "\uE70E";
        RenewalSortIcon.Glyph = ViewModel.SelectedSort == "Renewal date (latest first)" ? "\uE70D" : "\uE70E";
        ActivitySortIcon.Glyph = ViewModel.SelectedSort == "Last activity (oldest first)" ? "\uE70E" : "\uE70D";
        ValueSortIcon.Glyph = ViewModel.SelectedSort == "Account value (low to high)" ? "\uE70E" : "\uE70D";

        AutomationProperties.SetName(CompanySortButton, $"Sort by company name, currently {GetSortDirection("Company")}");
        AutomationProperties.SetName(ContactSortButton, $"Sort by contact name, currently {GetSortDirection("Contact")}");
        AutomationProperties.SetName(OwnerSortButton, $"Sort by owner name, currently {GetSortDirection("Owner")}");
        AutomationProperties.SetName(RegionSortButton, $"Sort by region, currently {GetSortDirection("Region")}");
        AutomationProperties.SetName(StatusSortButton, $"Sort by status, currently {GetSortDirection("Status")}");
        AutomationProperties.SetName(RenewalSortButton, $"Sort by renewal date, currently {GetSortDirection("Renewal")}");
        AutomationProperties.SetName(ActivitySortButton, $"Sort by last activity, currently {GetSortDirection("Last activity")}");
        AutomationProperties.SetName(ValueSortButton, $"Sort by account value, currently {GetSortDirection("Account value")}");
    }

    private string GetSortDirection(string prefix)
    {
        if (!ViewModel.SelectedSort.StartsWith(prefix))
            return "not sorted";
        if (prefix == "Renewal")
            return ViewModel.SelectedSort.Contains("soonest first") ? "soonest first" : "latest first";
        if (prefix == "Last activity")
            return ViewModel.SelectedSort.Contains("newest first") ? "newest first" : "oldest first";
        return ViewModel.SelectedSort.Contains("Z-A") || ViewModel.SelectedSort.Contains("high to low")
            ? "descending"
            : "ascending";
    }

    private void CustomerCards_ItemClick(object sender, ItemClickEventArgs e) =>
        NavigateToCustomer(e.ClickedItem);

    private void CustomerTable_ItemClick(object sender, ItemClickEventArgs e)
        => NavigateToCustomer(e.ClickedItem);

    private void NavigateToCustomer(object item)
    {
        if (item is Customer customer)
            MainWindow.Instance.NavigateToCustomer(customer.Id);
    }

    private async void AddCustomer_Click(object sender, RoutedEventArgs e) =>
        await MainWindow.Instance.ShowAddCustomerAsync();
}
