using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace DashboardAndDailyTasks.Models;

public partial class Customer : ObservableObject
{
    public Guid Id { get; } = Guid.NewGuid();
    [ObservableProperty] public partial string Name { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant0))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant1))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant2))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant3))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant4))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant5))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant6))]
    [NotifyPropertyChangedFor(nameof(IsLogoVariant7))]
    public partial string Company { get; set; } = "";
    [ObservableProperty] public partial string Email { get; set; } = "";
    [ObservableProperty] public partial string Phone { get; set; } = "";
    [ObservableProperty] public partial string Region { get; set; } = "";
    [ObservableProperty] public partial string Status { get; set; } = "";
    [ObservableProperty] public partial string Industry { get; set; } = "";
    [ObservableProperty] public partial string Owner { get; set; } = "";
    [ObservableProperty] public partial int AccountValueAmount { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RenewalDateDisplay))]
    public partial DateTimeOffset RenewalDate { get; set; }
    [ObservableProperty] public partial string LastActivity { get; set; } = "";
    [ObservableProperty] public partial string Summary { get; set; } = "";

    public string AccountValue => $"${AccountValueAmount:N0}";
    public string RenewalDateDisplay => RenewalDate.ToString("MMM d, yyyy");
    private int LogoVariant => Company switch
    {
        "Contoso Ltd" => 0,
        "Fabrikam Inc" => 1,
        "Adventure Works" => 2,
        "Northwind Traders" => 3,
        "Tailspin Toys" => 4,
        "Wingtip Toys" => 5,
        "Litware Inc" => 6,
        "Proseware Inc" => 7,
        "Fourth Coffee" => 0,
        "Graphic Design Co" => 1,
        _ => Company.Select((character, index) => character * (index + 1)).Sum() % 8
    };
    public bool IsLogoVariant0 => LogoVariant == 0;
    public bool IsLogoVariant1 => LogoVariant == 1;
    public bool IsLogoVariant2 => LogoVariant == 2;
    public bool IsLogoVariant3 => LogoVariant == 3;
    public bool IsLogoVariant4 => LogoVariant == 4;
    public bool IsLogoVariant5 => LogoVariant == 5;
    public bool IsLogoVariant6 => LogoVariant == 6;
    public bool IsLogoVariant7 => LogoVariant == 7;
    public IReadOnlyList<LogoPixel> LogoPixels
    {
        get
        {
            int seed = Company.Select((character, index) => character * (index + 17)).Sum();
            var pixels = new List<LogoPixel>(25);
            int visibleCount = 0;
            for (int row = 0; row < 5; row++)
            {
                bool[] halfRow = new bool[3];
                for (int column = 0; column < 3; column++)
                {
                    seed = unchecked(seed * 1103515245 + 12345);
                    halfRow[column] = ((seed >> 16) & 1) == 1;
                }

                for (int column = 0; column < 5; column++)
                {
                    bool isVisible = halfRow[column < 3 ? column : 4 - column];
                    pixels.Add(new LogoPixel(isVisible));
                    if (isVisible)
                        visibleCount++;
                }
            }

            if (visibleCount < 7)
                pixels[12] = new LogoPixel(true);
            return pixels;
        }
    }
    public int LastActivitySortOrder => LastActivity switch
    {
        "12 minutes ago" => 0,
        "3 hours ago" => 1,
        "Yesterday" => 2,
        "2 days ago" => 3,
        "Last week" => 4,
        _ => int.MaxValue
    };
    public string AutomationName =>
        $"{Name}, {Company}, {Status}, {Region} region, renews {RenewalDateDisplay}, account value {AccountValue}";
}

public partial class SupportTicket : ObservableObject
{
    public int Id { get; init; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    public partial string Subject { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    public partial string Customer { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    public partial string Product { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    public partial string Priority { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDone))]
    [NotifyPropertyChangedFor(nameof(StatusLabel))]
    [NotifyPropertyChangedFor(nameof(AutomationName))]
    public partial string Status { get; set; } = "";
    [ObservableProperty] public partial string Description { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTriage))]
    public partial string? Summary { get; set; }
    [ObservableProperty] public partial string? Category { get; set; }
    [ObservableProperty] public partial bool IsProcessing { get; set; }
    [ObservableProperty] public partial bool IsSelected { get; set; }

    public ObservableCollection<TicketComment> Comments { get; } = [];
    public string Number => $"TKT-{Id}";
    public bool HasTriage => !string.IsNullOrEmpty(Summary);
    public bool IsDone => Status is "Done" or "Resolved";
    public string StatusLabel => Status;
    public string AutomationName =>
        $"{Number}, {Subject}, {Customer}, {Product}, {Priority} priority, {Status}";
}

public sealed class TicketComment
{
    public TicketComment(string author, string body, DateTimeOffset? createdAt = null)
    {
        Author = author;
        Body = body;
        CreatedAt = createdAt ?? DateTimeOffset.Now;
    }

    public string Author { get; }
    public string Body { get; }
    public DateTimeOffset CreatedAt { get; }
    public string CreatedLabel => CreatedAt.Year == DateTimeOffset.Now.Year
        ? CreatedAt.ToString("MMM d 'at' h:mm tt")
        : CreatedAt.ToString("MMM d, yyyy 'at' h:mm tt");
    public ObservableCollection<TaskLinkOption> RelatedLinks { get; } = [];
    public bool HasRelatedLinks => RelatedLinks.Count > 0;
    public IReadOnlyList<InlineContentPart> InlineParts =>
        InlineContentBuilder.Build(Body, RelatedLinks);
}

public partial class WorkTask : ObservableObject
{
    public Guid Id { get; } = Guid.NewGuid();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeleteAutomationName))]
    [NotifyPropertyChangedFor(nameof(ReorderAutomationName))]
    [NotifyPropertyChangedFor(nameof(RestoreAutomationName))]
    public partial string Title { get; set; } = "";
    [ObservableProperty] public partial string Priority { get; set; } = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DueText))]
    [NotifyPropertyChangedFor(nameof(DueValue))]
    public partial DateTimeOffset Due { get; set; }
    [ObservableProperty] public partial bool IsComplete { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotDeleted))]
    public partial bool IsDeleted { get; set; }
    public string InlineText { get; set; } = "";
    public ObservableCollection<TaskLinkOption> RelatedLinks { get; } = [];
    public bool IsNotDeleted => !IsDeleted;
    public bool HasRelatedLinks => RelatedLinks.Count > 0;
    public string DeleteAutomationName => $"Delete task: {Title}";
    public string ReorderAutomationName => $"Reorder task: {Title}";
    public string RestoreAutomationName => $"Restore task: {Title}";
    public string DueText => Due.Date switch
    {
        var date when date < DateTimeOffset.Now.Date => $"Overdue · {Due:MMM d}",
        var date when date == DateTimeOffset.Now.Date => "Due today",
        var date when date == DateTimeOffset.Now.Date.AddDays(1) => "Due tomorrow",
        _ => $"Due {Due:ddd, MMM d}"
    };
    public string DueValue => Due.Date switch
    {
        var date when date < DateTimeOffset.Now.Date => Due.ToString("MMM d"),
        var date when date == DateTimeOffset.Now.Date => "Today",
        var date when date == DateTimeOffset.Now.Date.AddDays(1) => "Tomorrow",
        _ => Due.ToString("ddd, MMM d")
    };
    public string RelatedSummary => RelatedLinks.Count == 0
        ? "General"
        : string.Join(" · ", RelatedLinks.Select(link => link.MentionText));
    public IReadOnlyList<InlineContentPart> InlineParts =>
        InlineContentBuilder.Build(
            string.IsNullOrWhiteSpace(InlineText) ? Title : InlineText,
            RelatedLinks);
}

public sealed record TaskLinkOption(string Kind, string Id, string Name)
{
    public string DisplayName => string.IsNullOrEmpty(Kind) ? "No linked record" : $"{Kind} · {Name}";
    public string MentionText => Name.Split('·')[0].Trim();
    public string TagText => Kind == "Ticket" ? Name.Replace(" · ", " - ") : MentionText;
    public string RemoveAutomationName => $"Remove attachment: {DisplayName}";
}

public sealed record InlineContentPart(string Text, TaskLinkOption? Link = null)
{
    public Visibility TextVisibility => Link is null ? Visibility.Visible : Visibility.Collapsed;
    public Visibility LinkVisibility => Link is null ? Visibility.Collapsed : Visibility.Visible;
    public string LinkText => Link?.TagText ?? "";
    public string LinkDisplayName => Link?.DisplayName ?? "";
}

public static class InlineContentBuilder
{
    public static IReadOnlyList<InlineContentPart> Build(
        string text,
        IEnumerable<TaskLinkOption> links)
    {
        List<TaskLinkOption> availableLinks = links
            .DistinctBy(link => (link.Kind, link.Id))
            .ToList();
        var usedLinks = new HashSet<TaskLinkOption>();
        var parts = new List<InlineContentPart>();
        string remaining = text;

        while (remaining.Length > 0)
        {
            (TaskLinkOption? link, int index) = availableLinks
                .Select(link => (link, index: remaining.IndexOf(
                    $"@{link.MentionText}",
                    StringComparison.OrdinalIgnoreCase)))
                .Where(match => match.index >= 0)
                .OrderBy(match => match.index)
                .FirstOrDefault();

            if (link is null)
            {
                AddTextParts(parts, remaining);
                break;
            }

            AddTextParts(parts, remaining[..index]);
            parts.Add(new InlineContentPart("", link));
            usedLinks.Add(link);
            remaining = remaining[(index + link.MentionText.Length + 1)..];
        }

        foreach (TaskLinkOption link in availableLinks.Where(link => !usedLinks.Contains(link)))
            parts.Add(new InlineContentPart("", link));

        return parts;
    }

    private static void AddTextParts(List<InlineContentPart> parts, string text)
    {
        foreach (string word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            parts.Add(new InlineContentPart(word));
    }
}

public sealed record LogoPixel(bool IsVisible)
{
    public double Opacity => IsVisible ? 1 : 0;
}
