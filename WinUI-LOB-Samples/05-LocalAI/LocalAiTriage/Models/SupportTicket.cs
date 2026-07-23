using CommunityToolkit.Mvvm.ComponentModel;

namespace LocalAiTriage.Models;

/// <summary>
/// A single customer support ticket. The read-only fields are the incoming
/// record; the observable fields hold the results produced on-device by the
/// local language model so the UI updates as triage completes.
/// </summary>
public sealed partial class SupportTicket : ObservableObject
{
    public SupportTicket(int id, string customerName, string product, string subject, string body)
    {
        Id = id;
        CustomerName = customerName;
        Product = product;
        Subject = subject;
        Body = body;
    }

    public int Id { get; }

    public string CustomerName { get; }

    public string Product { get; }

    public string Subject { get; }

    public string Body { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTriage))]
    public partial string? Summary { get; set; }

    [ObservableProperty]
    public partial string? Category { get; set; }

    [ObservableProperty]
    public partial bool IsProcessing { get; set; }

    /// <summary>Gets a value indicating whether AI triage has produced a summary.</summary>
    public bool HasTriage => !string.IsNullOrEmpty(Summary);
}
