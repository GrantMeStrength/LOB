using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DashboardAndDailyTasks.Models;

namespace DashboardAndDailyTasks.ViewModels;

public sealed partial class CustomersViewModel : ObservableObject
{
    private readonly List<Customer> _allCustomers = [];
    private readonly HashSet<string> _selectedRegions = [];
    private readonly HashSet<string> _selectedStatuses = [];

    public ObservableCollection<Customer> Customers { get; } = [];

    public IReadOnlyList<string> SortOptions { get; } =
    [
        "Company name (A-Z)",
        "Company name (Z-A)",
        "Contact name (A-Z)",
        "Contact name (Z-A)",
        "Owner name (A-Z)",
        "Owner name (Z-A)",
        "Region (A-Z)",
        "Region (Z-A)",
        "Status (A-Z)",
        "Status (Z-A)",
        "Renewal date (soonest first)",
        "Renewal date (latest first)",
        "Last activity (newest first)",
        "Last activity (oldest first)",
        "Account value (high to low)",
        "Account value (low to high)"
    ];

    public IReadOnlyList<string> RenewalFilterOptions { get; } =
    [
        "All renewal dates",
        "Overdue",
        "Next 30 days",
        "31-60 days",
        "More than 60 days"
    ];

    public IReadOnlyList<string> StatusFilterOptions { get; } =
    [
        "Active",
        "Prospect",
        "Pending",
        "At risk"
    ];

    public IReadOnlyList<string> RegionFilterOptions { get; } =
    [
        "North",
        "South",
        "East",
        "West",
        "Central"
    ];

    [ObservableProperty]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    public partial string SelectedSort { get; set; } = "Company name (A-Z)";

    [ObservableProperty]
    public partial string ResultSummary { get; set; } = "";

    [ObservableProperty]
    public partial string StatusFilterLabel { get; set; } = "All statuses";

    [ObservableProperty]
    public partial string RegionFilterLabel { get; set; } = "All regions";

    [ObservableProperty]
    public partial string SelectedRenewalFilter { get; set; } = "All renewal dates";

    public string TotalPortfolioValue { get; private set; } = "$0";
    public int TotalCustomerCount { get; private set; }
    public int ActiveAccountCount { get; private set; }
    public int AtRiskAccountCount { get; private set; }

    public CustomersViewModel() => RefreshFromState();

    public void RefreshFromState()
    {
        _allCustomers.Clear();
        _allCustomers.AddRange(App.State.Customers);
        TotalCustomerCount = _allCustomers.Count;
        ActiveAccountCount = _allCustomers.Count(customer => customer.Status == "Active");
        AtRiskAccountCount = _allCustomers.Count(customer => customer.Status == "At risk");
        TotalPortfolioValue = $"${_allCustomers.Sum(customer => customer.AccountValueAmount) / 1_000_000d:0.0}M";
        OnPropertyChanged(nameof(TotalCustomerCount));
        OnPropertyChanged(nameof(ActiveAccountCount));
        OnPropertyChanged(nameof(AtRiskAccountCount));
        OnPropertyChanged(nameof(TotalPortfolioValue));
        ApplyFilters();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    partial void OnSelectedSortChanged(string value) => ApplyFilters();

    public void ResetFilters()
    {
        _selectedStatuses.Clear();
        _selectedRegions.Clear();
        SearchText = "";
        SelectedSort = "Company name (A-Z)";
        StatusFilterLabel = "All statuses";
        RegionFilterLabel = "All regions";
        SelectedRenewalFilter = "All renewal dates";
        ApplyFilters();
    }

    public void SetStatusFilter(string status, bool isSelected)
    {
        if (isSelected)
            _selectedStatuses.Add(status);
        else
            _selectedStatuses.Remove(status);

        StatusFilterLabel = _selectedStatuses.Count switch
        {
            0 => "All statuses",
            1 => _selectedStatuses.Single(),
            _ => $"{_selectedStatuses.Count} statuses"
        };
        ApplyFilters();
    }

    public void SetRegionFilter(string region, bool isSelected)
    {
        if (isSelected)
            _selectedRegions.Add(region);
        else
            _selectedRegions.Remove(region);

        RegionFilterLabel = _selectedRegions.Count switch
        {
            0 => "All regions",
            1 => _selectedRegions.Single(),
            _ => $"{_selectedRegions.Count} regions"
        };
        ApplyFilters();
    }

    partial void OnSelectedRenewalFilterChanged(string value) => ApplyFilters();

    public void ToggleSort(string column)
    {
        SelectedSort = column switch
        {
            "Company" when SelectedSort == "Company name (A-Z)" => "Company name (Z-A)",
            "Company" => "Company name (A-Z)",
            "Contact" when SelectedSort == "Contact name (A-Z)" => "Contact name (Z-A)",
            "Contact" => "Contact name (A-Z)",
            "Owner" when SelectedSort == "Owner name (A-Z)" => "Owner name (Z-A)",
            "Owner" => "Owner name (A-Z)",
            "Region" when SelectedSort == "Region (A-Z)" => "Region (Z-A)",
            "Region" => "Region (A-Z)",
            "Status" when SelectedSort == "Status (A-Z)" => "Status (Z-A)",
            "Status" => "Status (A-Z)",
            "Renewal" when SelectedSort == "Renewal date (soonest first)" => "Renewal date (latest first)",
            "Renewal" => "Renewal date (soonest first)",
            "Activity" when SelectedSort == "Last activity (newest first)" => "Last activity (oldest first)",
            "Activity" => "Last activity (newest first)",
            "Value" when SelectedSort == "Account value (high to low)" => "Account value (low to high)",
            "Value" => "Account value (high to low)",
            _ => SelectedSort
        };
    }

    private void ApplyFilters()
    {
        IEnumerable<Customer> filtered = _allCustomers;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(customer =>
                customer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                customer.Company.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                customer.Owner.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                customer.Industry.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (_selectedStatuses.Count > 0)
            filtered = filtered.Where(customer => _selectedStatuses.Contains(customer.Status));

        if (_selectedRegions.Count > 0)
            filtered = filtered.Where(customer => _selectedRegions.Contains(customer.Region));

        DateTimeOffset today = DateTimeOffset.Now.Date;
        filtered = SelectedRenewalFilter switch
        {
            "Overdue" => filtered.Where(customer => customer.RenewalDate < today),
            "Next 30 days" => filtered.Where(customer =>
                customer.RenewalDate >= today && customer.RenewalDate <= today.AddDays(30)),
            "31-60 days" => filtered.Where(customer =>
                customer.RenewalDate > today.AddDays(30) && customer.RenewalDate <= today.AddDays(60)),
            "More than 60 days" => filtered.Where(customer => customer.RenewalDate > today.AddDays(60)),
            _ => filtered
        };

        filtered = SelectedSort switch
        {
            "Company name (Z-A)" => filtered.OrderByDescending(customer => customer.Company),
            "Contact name (A-Z)" => filtered.OrderBy(customer => customer.Name),
            "Contact name (Z-A)" => filtered.OrderByDescending(customer => customer.Name),
            "Owner name (A-Z)" => filtered.OrderBy(customer => customer.Owner),
            "Owner name (Z-A)" => filtered.OrderByDescending(customer => customer.Owner),
            "Region (A-Z)" => filtered.OrderBy(customer => customer.Region),
            "Region (Z-A)" => filtered.OrderByDescending(customer => customer.Region),
            "Status (A-Z)" => filtered.OrderBy(customer => customer.Status),
            "Status (Z-A)" => filtered.OrderByDescending(customer => customer.Status),
            "Renewal date (soonest first)" => filtered.OrderBy(customer => customer.RenewalDate),
            "Renewal date (latest first)" => filtered.OrderByDescending(customer => customer.RenewalDate),
            "Last activity (newest first)" => filtered.OrderBy(customer => customer.LastActivitySortOrder),
            "Last activity (oldest first)" => filtered.OrderByDescending(customer => customer.LastActivitySortOrder),
            "Account value (high to low)" => filtered.OrderByDescending(customer => customer.AccountValueAmount),
            "Account value (low to high)" => filtered.OrderBy(customer => customer.AccountValueAmount),
            _ => filtered.OrderBy(customer => customer.Company)
        };

        Customers.Clear();
        foreach (Customer customer in filtered)
            Customers.Add(customer);

        ResultSummary = Customers.Count == 1
            ? "1 customer account"
            : $"{Customers.Count} customer accounts";
    }
}
