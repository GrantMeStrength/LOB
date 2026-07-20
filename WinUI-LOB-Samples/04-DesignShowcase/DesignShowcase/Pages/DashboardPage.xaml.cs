using Microsoft.UI.Xaml.Controls;
using DesignShowcase.ViewModels;

namespace DesignShowcase.Pages;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    public DashboardViewModel ViewModel { get; } = new();
}
