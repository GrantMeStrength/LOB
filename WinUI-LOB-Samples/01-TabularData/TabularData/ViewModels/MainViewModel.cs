using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TabularData.ViewModels;

/// <summary>
/// Main view model that owns the shared customer collection bound (via
/// <c>x:Bind</c>) to both inbox tabular views.
/// The 20 fake records are produced asynchronously so the UI thread is never
/// blocked during loading.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// The single source of truth for both views.
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
