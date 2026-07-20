using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using TabularData.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TabularData;

/// <summary>
/// Hosts Part A (card <see cref="ItemsView"/>) and Part B (sortable Community
/// Toolkit DataGrid), both bound to the same <see cref="MainViewModel"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Sorts the shared collection when a DataGrid column header is clicked and
    /// updates the sort-direction glyphs. The Community Toolkit DataGrid does
    /// not sort automatically, so we reorder the source and set the indicator.
    /// </summary>
    private void CustomerGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        DataGridSortDirection newDirection = e.Column.SortDirection == DataGridSortDirection.Ascending
            ? DataGridSortDirection.Descending
            : DataGridSortDirection.Ascending;

        ViewModel.SortCustomers(e.Column.Tag as string, newDirection == DataGridSortDirection.Ascending);

        foreach (DataGridColumn column in CustomerGrid.Columns)
        {
            column.SortDirection = null;
        }

        e.Column.SortDirection = newDirection;
    }
}
