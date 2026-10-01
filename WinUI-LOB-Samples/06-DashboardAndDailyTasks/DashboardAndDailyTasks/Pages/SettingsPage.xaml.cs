using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class SettingsPage : Page
{
    private bool _loading = true;
    public SettingsPage()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ThemeOptions.SelectedIndex = App.State.Theme switch { ElementTheme.Light => 1, ElementTheme.Dark => 2, _ => 0 };
            RevenueOverviewToggle.IsOn = App.State.ShowRevenueOverview;
            KpisToggle.IsOn = App.State.ShowKpis;
            TicketPerformanceToggle.IsOn = App.State.ShowTicketPerformance;
            TodayTasksToggle.IsOn = App.State.ShowTodayTasks;
            CustomersToggle.IsOn = App.State.ShowCustomers;
            OpenTicketsToggle.IsOn = App.State.ShowOpenTickets;
            NotificationToggle.IsOn = App.State.NotificationsEnabled;
            DefaultPagePicker.SelectedIndex = new[] { "Dashboard", "Tickets", "Tasks", "Customers" }.ToList().IndexOf(App.State.DefaultPage);
            _loading = false;
        };
    }
    private void ThemeOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        MainWindow.Instance.ApplyTheme(ThemeOptions.SelectedIndex switch { 1 => ElementTheme.Light, 2 => ElementTheme.Dark, _ => ElementTheme.Default });
    }
    private void DashboardWidgetToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_loading)
            return;

        App.State.ShowRevenueOverview = RevenueOverviewToggle.IsOn;
        App.State.ShowKpis = KpisToggle.IsOn;
        App.State.ShowTicketPerformance = TicketPerformanceToggle.IsOn;
        App.State.ShowTodayTasks = TodayTasksToggle.IsOn;
        App.State.ShowCustomers = CustomersToggle.IsOn;
        App.State.ShowOpenTickets = OpenTicketsToggle.IsOn;
        App.State.NotifyChanged();
    }
    private void NotificationToggle_Toggled(object sender, RoutedEventArgs e) { if (!_loading) App.State.NotificationsEnabled = NotificationToggle.IsOn; }
    private void DefaultPagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_loading && DefaultPagePicker.SelectedItem is ComboBoxItem { Tag: string destination })
            App.State.DefaultPage = destination;
    }
    private async void Reset_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = "Reset sample data?",
            Content = "This replaces all customer, ticket, and task changes with the original sample data.",
            PrimaryButtonText = "Reset data",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;

        App.State.Reset();
        ResetConfirmation.IsOpen = true;
    }
}
