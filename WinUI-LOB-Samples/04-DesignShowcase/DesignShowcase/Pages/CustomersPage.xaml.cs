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
}
