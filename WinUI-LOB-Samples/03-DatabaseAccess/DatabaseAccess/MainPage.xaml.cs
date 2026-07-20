using System;
using DatabaseAccess.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DatabaseAccess;

/// <summary>
/// The main content page displayed inside the application window.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
    }

    /// <summary>Formats a due date for display in the task list.</summary>
    public static string FormatDueDate(DateTime dueDate) =>
        $"Due {dueDate:ddd, MMM d, yyyy}";

    /// <summary>Maps <c>true</c> to Visible, <c>false</c> to Collapsed.</summary>
    public static Visibility BoolToVisibility(bool value) =>
        value ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Maps <c>true</c> to Collapsed, <c>false</c> to Visible.</summary>
    public static Visibility InvertBoolToVisibility(bool value) =>
        value ? Visibility.Collapsed : Visibility.Visible;
}
