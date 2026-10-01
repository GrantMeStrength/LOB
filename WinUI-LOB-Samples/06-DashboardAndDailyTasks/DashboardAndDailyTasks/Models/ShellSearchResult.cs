namespace DashboardAndDailyTasks.Models;

public sealed record ShellSearchResult(
    string Kind,
    string Title,
    string Subtitle,
    string Glyph,
    object Record)
{
    public string AutomationName => $"{Kind}: {Title}. {Subtitle}";
}
