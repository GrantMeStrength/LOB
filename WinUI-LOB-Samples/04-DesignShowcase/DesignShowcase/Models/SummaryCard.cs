namespace DesignShowcase.Models;

/// <summary>
/// A read-only summary tile shown on the Dashboard (title, headline value, glyph).
/// </summary>
public sealed class SummaryCard
{
    public SummaryCard(string title, string value, string glyph)
    {
        Title = title;
        Value = value;
        Glyph = glyph;
    }

    public string Title { get; }

    public string Value { get; }

    /// <summary>Segoe Fluent Icons glyph code point (e.g. "\uE716").</summary>
    public string Glyph { get; }
}
