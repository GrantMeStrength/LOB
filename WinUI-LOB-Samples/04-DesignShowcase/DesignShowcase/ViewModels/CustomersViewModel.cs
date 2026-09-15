using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesignShowcase.Models;

namespace DesignShowcase.ViewModels;

/// <summary>
/// Backing data for the Customers page: a sample list of customer records.
/// </summary>
public sealed partial class CustomersViewModel : ObservableObject
{
    private readonly List<Customer> _allCustomers;

    public CustomersViewModel()
    {
        _allCustomers = new List<Customer>
        {
            new("Ada Lovelace", "Analytical Engines Ltd.", "Active"),
            new("Grace Hopper", "Compiler Systems", "Active"),
            new("Alan Turing", "Bletchley Solutions", "Pending"),
            new("Katherine Johnson", "Orbital Dynamics", "Active"),
            new("Margaret Hamilton", "Apollo Software", "Overdue"),
            new("Dennis Ritchie", "Bell Labs Retail", "Active"),
        };

        ApplyFilter();
    }

    public ObservableCollection<Customer> Customers { get; } = new();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private void ClearSearch() => SearchText = string.Empty;

    private void ApplyFilter()
    {
        // Keep the public collection instance stable while filtering. ItemsView
        // observes collection changes, so no binding reset or replacement is needed.
        IEnumerable<Customer> filtered = _allCustomers;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string query = SearchText.Trim();
            filtered = filtered.Where(customer =>
                customer.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || customer.Company.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || customer.Status.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        Customers.Clear();
        foreach (Customer customer in filtered)
        {
            Customers.Add(customer);
        }

        IsEmpty = Customers.Count == 0;
    }
}
