using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DesignShowcase.Models;

namespace DesignShowcase.ViewModels;

/// <summary>
/// Backing data for the Dashboard page: a small set of at-a-glance summary cards.
/// </summary>
public sealed partial class DashboardViewModel : ObservableObject
{
    public DashboardViewModel()
    {
        Cards = new ObservableCollection<SummaryCard>
        {
            new("Total Customers", "1,284", "\uE716"),
            new("Open Tasks", "37", "\uE762"),
            new("Overdue", "5", "\uE7BA"),
            new("Completed", "912", "\uE73E"),
        };
    }

    public ObservableCollection<SummaryCard> Cards { get; }
}
