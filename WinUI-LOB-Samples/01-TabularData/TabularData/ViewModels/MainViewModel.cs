using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TabularData.ViewModels;

/// <summary>
/// Main view model that owns the shared customer collection bound (via
/// <c>x:Bind</c>) to both the card <c>ItemsView</c> and the sortable DataGrid.
/// The 20 fake records are produced asynchronously so the UI thread is never
/// blocked during loading.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// The single source of truth for both views. Reordered in place when the
    /// user sorts a DataGrid column.
    /// </summary>
    public ObservableCollection<CustomerViewModel> Customers { get; } = new();

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

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

        foreach (CustomerViewModel customer in generated)
        {
            Customers.Add(customer);
        }

        IsLoading = false;
    }

    /// <summary>
    /// Sorts <see cref="Customers"/> in place by the given field. Called from
    /// the DataGrid's <c>Sorting</c> event so both views reflect the new order.
    /// </summary>
    public void SortCustomers(string? field, bool ascending)
    {
        if (string.IsNullOrEmpty(field) || Customers.Count == 0)
        {
            return;
        }

        Func<CustomerViewModel, string> selector = field switch
        {
            nameof(CustomerViewModel.Name) => c => c.Name,
            nameof(CustomerViewModel.Company) => c => c.Company,
            nameof(CustomerViewModel.Region) => c => c.Region,
            nameof(CustomerViewModel.Status) => c => c.Status,
            _ => c => c.Name,
        };

        List<CustomerViewModel> ordered = ascending
            ? Customers.OrderBy(selector, StringComparer.OrdinalIgnoreCase).ToList()
            : Customers.OrderByDescending(selector, StringComparer.OrdinalIgnoreCase).ToList();

        for (int targetIndex = 0; targetIndex < ordered.Count; targetIndex++)
        {
            int currentIndex = Customers.IndexOf(ordered[targetIndex]);
            if (currentIndex != targetIndex)
            {
                Customers.Move(currentIndex, targetIndex);
            }
        }
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
