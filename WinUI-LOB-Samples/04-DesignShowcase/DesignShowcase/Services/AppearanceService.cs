using System;
using Microsoft.UI.Xaml;

namespace DesignShowcase.Services;

/// <summary>
/// Centralizes runtime appearance changes (Light/Dark theme and control density)
/// so that both the title-bar theme toggle and the Settings density switch operate
/// on the same root element. The root is the window's content root, so changes
/// cascade to every page hosted in the navigation frame.
/// </summary>
public static class AppearanceService
{
    // Verified against Windows App SDK docs (Compact sizing for controls):
    // https://learn.microsoft.com/windows/apps/develop/ui/controls/compact-sizing
    private const string CompactDictionaryUri = "ms-appx:///Microsoft.UI.Xaml/DensityStyles/Compact.xaml";

    private static FrameworkElement? _root;
    private static ResourceDictionary? _compactDictionary;

    /// <summary>Registers the element whose theme/density is toggled at runtime.</summary>
    public static void Initialize(FrameworkElement root) => _root = root;

    /// <summary>True when the compact-density dictionary is currently merged in.</summary>
    public static bool IsCompact { get; private set; }

    /// <summary>The current requested theme of the root element.</summary>
    public static ElementTheme CurrentTheme => _root?.RequestedTheme ?? ElementTheme.Default;

    /// <summary>Flips the root element between Light and Dark and returns the new theme.</summary>
    public static ElementTheme ToggleTheme()
    {
        if (_root is null)
        {
            return ElementTheme.Default;
        }

        _root.RequestedTheme = _root.RequestedTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;

        return _root.RequestedTheme;
    }

    /// <summary>
    /// Adds or removes the built-in Compact-density resource dictionary on the root
    /// element, visibly tightening (or relaxing) control spacing across the app.
    /// </summary>
    public static void SetCompact(bool compact)
    {
        if (_root is null || compact == IsCompact)
        {
            IsCompact = compact;
            return;
        }

        if (compact)
        {
            _compactDictionary ??= new ResourceDictionary { Source = new Uri(CompactDictionaryUri) };
            _root.Resources.MergedDictionaries.Add(_compactDictionary);
        }
        else if (_compactDictionary is not null)
        {
            _root.Resources.MergedDictionaries.Remove(_compactDictionary);
        }

        IsCompact = compact;
    }
}
