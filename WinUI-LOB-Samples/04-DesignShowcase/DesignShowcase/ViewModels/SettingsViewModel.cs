using CommunityToolkit.Mvvm.ComponentModel;
using DesignShowcase.Services;
using Microsoft.UI.Xaml;

namespace DesignShowcase.ViewModels;

/// <summary>
/// Backing data for the Settings page.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
        IsDarkTheme = AppearanceService.CurrentTheme == ElementTheme.Dark;
    }

    [ObservableProperty]
    public partial bool IsDarkTheme { get; set; }

    partial void OnIsDarkThemeChanged(bool value) =>
        AppearanceService.SetTheme(value ? ElementTheme.Dark : ElementTheme.Light);
}
