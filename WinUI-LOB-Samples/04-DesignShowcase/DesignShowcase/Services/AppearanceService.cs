using Microsoft.UI.Xaml;

namespace DesignShowcase.Services;

/// <summary>
/// Centralizes runtime appearance changes (Light/Dark theme) so the title-bar
/// toggle and Settings page control the same root element.
/// </summary>
public static class AppearanceService
{
    private static FrameworkElement? _root;

    /// <summary>Registers the element whose theme is toggled at runtime.</summary>
    public static void Initialize(FrameworkElement root) => _root = root;

    /// <summary>The current requested theme of the root element.</summary>
    public static ElementTheme CurrentTheme => _root?.RequestedTheme ?? ElementTheme.Default;

    /// <summary>Sets the current requested theme of the root element.</summary>
    public static ElementTheme SetTheme(ElementTheme theme)
    {
        if (_root is null)
        {
            return ElementTheme.Default;
        }

        _root.RequestedTheme = theme;
        return _root.RequestedTheme;
    }

    /// <summary>Flips the root element between Light and Dark and returns the new theme.</summary>
    public static ElementTheme ToggleTheme()
    {
        ElementTheme theme = CurrentTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;

        return SetTheme(theme);
    }
}
