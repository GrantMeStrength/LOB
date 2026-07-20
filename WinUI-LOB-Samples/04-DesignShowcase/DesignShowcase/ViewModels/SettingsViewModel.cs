using CommunityToolkit.Mvvm.ComponentModel;
using DesignShowcase.Services;

namespace DesignShowcase.ViewModels;

/// <summary>
/// Backing data for the Settings page. Currently exposes the control-density
/// toggle, which merges/unmerges the built-in Compact resource dictionary.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    /// <summary>
    /// When true the app uses COMPACT control density; when false, NORMAL density.
    /// </summary>
    [ObservableProperty]
    public partial bool IsCompact { get; set; }

    partial void OnIsCompactChanged(bool value) => AppearanceService.SetCompact(value);
}
