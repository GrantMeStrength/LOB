namespace DashboardAndDailyTasks.Models;

public sealed class SummaryCard
{
    public SummaryCard(
        string title,
        string value,
        string detail,
        string glyph,
        bool isCritical = false,
        string? destination = null,
        CustomerFilter? customerFilter = null)
    {
        Title = title;
        Value = value;
        Detail = detail;
        Glyph = glyph;
        IsCritical = isCritical;
        Destination = destination;
        CustomerFilter = customerFilter;
    }

    public string Title { get; }
    public string Value { get; }
    public string Detail { get; }
    public string Glyph { get; }
    public bool IsCritical { get; }
    public bool IsNotCritical => !IsCritical;
    public string? Destination { get; }
    public CustomerFilter? CustomerFilter { get; }
    public bool HasDestination => Destination is not null;
    public bool HasNoDestination => Destination is null;
    public string AutomationName => $"{Title}, {Value}. {Detail}";
}

public enum CustomerFilter
{
    Active,
    UpcomingRenewals
}
