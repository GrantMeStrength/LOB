using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DesignShowcase.ViewModels;

namespace DesignShowcase.Pages;

public sealed partial class CustomersPage : Page
{
    public CustomersPage()
    {
        InitializeComponent();
    }

    public CustomersViewModel ViewModel { get; } = new();

    public static Visibility BoolToVisibility(bool value) =>
        value ? Visibility.Visible : Visibility.Collapsed;

    public static Visibility InvertBoolToVisibility(bool value) =>
        value ? Visibility.Collapsed : Visibility.Visible;
}
