using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TabularData.ViewModels;

/// <summary>
/// Main view model that owns the shared customer collection bound (via
/// <c>x:Bind</c>) to both inbox tabular views.
/// The 20 fake records are produced asynchronously so the UI thread is never
/// blocked during loading.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly List<CustomerViewModel> _allCustomers = new();

    /// <summary>
    /// The single source of truth for both views.
    /// </summary>
    public ObservableCollection<CustomerViewModel> Customers { get; } = new();

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResultSummary))]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    public string ResultSummary =>
        $"{Customers.Count} customer{(Customers.Count == 1 ? string.Empty : "s")}";

    public MainViewModel()
    {
        // Kick off async generation from the constructor. The continuation
        // resumes on the UI thread (captured context) to mutate the
        // ObservableCollection safely.
        _ = LoadAsync();
    }

    /// <summary>Generates the fake customer records off the UI thread.</summary>
    public async Task LoadAsync()
    {
        IsLoading = true;

        List<CustomerViewModel> generated = await Task.Run(GenerateCustomers);

        _allCustomers.Clear();
        _allCustomers.AddRange(generated);
        ApplyFilter();

        IsLoading = false;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private void ClearSearch() => SearchText = string.Empty;

    private void ApplyFilter()
    {
        // Both the card and table views bind to this same ObservableCollection.
        // Mutating it keeps those bindings intact and demonstrates one filtered
        // source of truth for multiple collection presentations.
        IEnumerable<CustomerViewModel> filtered = _allCustomers;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string query = SearchText.Trim();
            filtered = filtered.Where(customer =>
                customer.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || customer.Company.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || customer.Region.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || customer.Status.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        Customers.Clear();
        foreach (CustomerViewModel customer in filtered)
        {
            Customers.Add(customer);
        }

        IsEmpty = Customers.Count == 0;
        OnPropertyChanged(nameof(ResultSummary));
    }

    private static List<CustomerViewModel> GenerateCustomers()
    {
        string[] names =
        {
            "Ava Bennett", "Liam Chen", "Sofia Rossi", "Noah Patel", "Emma Nguyen",
            "Lucas Müller", "Mia Johansson", "Ethan Kowalski", "Olivia Reyes", "Mateo Silva",
            "Charlotte Dubois", "Hiro Tanaka", "Amara Okafor", "Daniel Kim", "Isabella Ferrari",
            "Omar Haddad", "Freya Larsen", "Diego Morales", "Priya Sharma", "Sean O'Brien",
        };

        string[] companies =
        {
            "Contoso Ltd", "Fabrikam Inc", "Adventure Works", "Northwind Traders", "Tailspin Toys",
            "Wingtip Toys", "Litware Inc", "Proseware Inc", "Fourth Coffee", "Graphic Design Co",
        };

        string[] regions = { "North", "South", "East", "West", "Central" };
        string[] statuses = { "Active", "Prospect", "Churned", "Pending" };

        var random = new Random(20250720);
        var customers = new List<CustomerViewModel>(names.Length);

        for (int i = 0; i < names.Length; i++)
        {
            customers.Add(new CustomerViewModel(
                names[i],
                companies[i % companies.Length],
                regions[random.Next(regions.Length)],
                statuses[random.Next(statuses.Length)]));
        }

        return customers;
    }
}
