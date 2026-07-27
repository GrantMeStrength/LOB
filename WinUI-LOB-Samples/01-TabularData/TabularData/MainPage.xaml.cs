using Microsoft.UI.Xaml.Controls;
using TabularData.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TabularData;

/// <summary>
/// Hosts a card <see cref="ItemsView"/> and a columnar inbox table view, both
/// bound to the same <see cref="MainViewModel"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
    }
}
