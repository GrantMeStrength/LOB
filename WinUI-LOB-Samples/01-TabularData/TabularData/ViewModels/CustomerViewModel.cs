using CommunityToolkit.Mvvm.ComponentModel;

namespace TabularData.ViewModels;

/// <summary>
/// Represents a single customer record shown in both the card
/// <c>ItemsView</c> (Part A) and the Community Toolkit DataGrid (Part B).
/// Derives from <see cref="ObservableObject"/> per the MVVM convention;
/// the display fields are immutable once generated.
/// </summary>
public sealed partial class CustomerViewModel : ObservableObject
{
    public CustomerViewModel(string name, string company, string region, string status)
    {
        Name = name;
        Company = company;
        Region = region;
        Status = status;
    }

    /// <summary>Customer contact name.</summary>
    public string Name { get; }

    /// <summary>Company the customer belongs to.</summary>
    public string Company { get; }

    /// <summary>Sales region.</summary>
    public string Region { get; }

    /// <summary>Account status (Active, Prospect, Churned, Pending).</summary>
    public string Status { get; }
}
