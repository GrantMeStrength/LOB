using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DesignShowcase.Models;

namespace DesignShowcase.ViewModels;

/// <summary>
/// Backing data for the Customers page: a sample list of customer records.
/// </summary>
public sealed partial class CustomersViewModel : ObservableObject
{
    public CustomersViewModel()
    {
        Customers = new ObservableCollection<Customer>
        {
            new("Ada Lovelace", "Analytical Engines Ltd.", "Active"),
            new("Grace Hopper", "Compiler Systems", "Active"),
            new("Alan Turing", "Bletchley Solutions", "Pending"),
            new("Katherine Johnson", "Orbital Dynamics", "Active"),
            new("Margaret Hamilton", "Apollo Software", "Overdue"),
            new("Dennis Ritchie", "Bell Labs Retail", "Active"),
        };
    }

    public ObservableCollection<Customer> Customers { get; }
}
