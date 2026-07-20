using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ValidatedForm.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ValidatedForm;

/// <summary>
/// The main content page hosting the "New Customer" validated form.
/// </summary>
public sealed partial class MainPage : Page
{
    public NewCustomerViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>Converts a bool into a <see cref="Visibility"/> for x:Bind functions.</summary>
    public static Visibility BoolToVisibility(bool value) =>
        value ? Visibility.Visible : Visibility.Collapsed;
}
