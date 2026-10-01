using DashboardAndDailyTasks.ViewModels;
using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        Loaded += DashboardPage_Loaded;
        Unloaded += DashboardPage_Unloaded;
    }

    public DashboardViewModel ViewModel { get; } = new();

    private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        App.State.Changed += State_Changed;
        RefreshLiveContent();
    }

    private void DashboardPage_Unloaded(object sender, RoutedEventArgs e) =>
        App.State.Changed -= State_Changed;

    private void State_Changed(object? sender, EventArgs e) => RefreshLiveContent();

    private void RefreshLiveContent()
    {
        ViewModel.Refresh();
        DashboardDateText.Text = DateTime.Now.ToString("dddd, MMMM d");
        DashboardGreetingText.Text = $"Good {Greeting()}, Alex";
        PopulatePriorityTasks();
        PopulateOpenTickets();
        PopulateMomentum();
        ApplyWidgetLayout();
    }

    private void WidgetGrid_SizeChanged(object sender, SizeChangedEventArgs e) =>
        ApplyWidgetLayout();

    private void ApplyWidgetLayout()
    {
        RevenueOverviewPanel.Visibility = App.State.ShowRevenueOverview ? Visibility.Visible : Visibility.Collapsed;
        SummaryCardsPanel.Visibility = App.State.ShowKpis ? Visibility.Visible : Visibility.Collapsed;
        TicketPerformancePanel.Visibility = App.State.ShowTicketPerformance ? Visibility.Visible : Visibility.Collapsed;
        PriorityPanel.Visibility = App.State.ShowTodayTasks ? Visibility.Visible : Visibility.Collapsed;
        CustomerMomentumPanel.Visibility = App.State.ShowCustomers ? Visibility.Visible : Visibility.Collapsed;
        OpenTicketsPanel.Visibility = App.State.ShowOpenTickets ? Visibility.Visible : Visibility.Collapsed;

        ArrangeWidgetPair(
            InsightGrid,
            TicketPerformancePanel,
            PriorityPanel,
            InsightPrimaryColumn,
            InsightSecondaryColumn,
            InsightSecondaryRow);
        ArrangeWidgetPair(
            MomentumGrid,
            CustomerMomentumPanel,
            OpenTicketsPanel,
            MomentumPrimaryColumn,
            MomentumSecondaryColumn,
            MomentumSecondaryRow);
    }

    private static void ArrangeWidgetPair(
        Grid grid,
        FrameworkElement primary,
        FrameworkElement secondary,
        ColumnDefinition primaryColumn,
        ColumnDefinition secondaryColumn,
        RowDefinition secondaryRow)
    {
        bool showPrimary = primary.Visibility == Visibility.Visible;
        bool showSecondary = secondary.Visibility == Visibility.Visible;
        bool showBoth = showPrimary && showSecondary;
        bool stack = showBoth && grid.ActualWidth < 920;

        grid.Visibility = showPrimary || showSecondary ? Visibility.Visible : Visibility.Collapsed;
        grid.ColumnSpacing = showBoth && !stack ? 16 : 0;
        grid.RowSpacing = stack ? 16 : 0;
        primaryColumn.Width = showBoth && !stack ? new GridLength(3, GridUnitType.Star) : new GridLength(1, GridUnitType.Star);
        secondaryColumn.Width = showBoth && !stack ? new GridLength(2, GridUnitType.Star) : new GridLength(0);
        secondaryRow.Height = stack ? GridLength.Auto : new GridLength(0);

        if (showPrimary)
        {
            Grid.SetColumn(primary, 0);
            Grid.SetRow(primary, 0);
            Grid.SetColumnSpan(primary, stack || !showSecondary ? 2 : 1);
        }

        if (showSecondary)
        {
            Grid.SetColumn(secondary, showBoth && !stack ? 1 : 0);
            Grid.SetRow(secondary, stack ? 1 : 0);
            Grid.SetColumnSpan(secondary, stack || !showPrimary ? 2 : 1);
        }
    }

    private void PopulatePriorityTasks()
    {
        WorkTask[] tasks = App.State.Tasks
            .Where(task =>
                !task.IsDeleted &&
                !task.IsComplete &&
                task.Due.Date == DateTimeOffset.Now.Date)
            .OrderBy(task => task.Due)
            .ThenBy(task => task.Title)
            .Take(3)
            .ToArray();
        DashboardTasksList.ItemsSource = tasks;
    }

    private void PopulateOpenTickets()
    {
        SupportTicket[] pending = App.State.Tickets
            .Where(ticket => ticket.Status == "Pending")
            .OrderByDescending(ticket => ticket.Id)
            .Take(2)
            .ToArray();
        SupportTicket[] inProgress = App.State.Tickets
            .Where(ticket => ticket.Status == "In progress")
            .OrderByDescending(ticket => ticket.Id)
            .Take(2)
            .ToArray();

        PendingTicketsList.ItemsSource = pending;
        PendingTicketsGroup.Visibility = pending.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
        InProgressTicketsList.ItemsSource = inProgress;
        InProgressTicketsGroup.Visibility = inProgress.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void DashboardTaskComplete_Click(object sender, RoutedEventArgs e) =>
        App.State.NotifyChanged();

    private void DashboardTicket_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is SupportTicket ticket)
            MainWindow.Instance.Navigate("Tickets", ticket.Id.ToString());
    }

    private void OpenTaskRelatedRecord_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not TaskLinkOption link)
            return;

        if (link.Kind == "Customer" &&
            Guid.TryParse(link.Id, out Guid id) &&
            App.State.Customers.Any(customer => customer.Id == id))
        {
            MainWindow.Instance.NavigateToCustomer(id);
            return;
        }

        MainWindow.Instance.Navigate("Tickets", link.Id);
    }

    private void PopulateMomentum()
    {
        Customer[] customers = App.State.Customers
            .OrderByDescending(customer => customer.AccountValueAmount)
            .Take(3)
            .ToArray();
        Button[] buttons = [MomentumButton1, MomentumButton2, MomentumButton3];
        TextBlock[] companies = [MomentumCompany1, MomentumCompany2, MomentumCompany3];
        TextBlock[] details = [MomentumDetail1, MomentumDetail2, MomentumDetail3];
        TextBlock[] statuses = [MomentumStatus1, MomentumStatus2, MomentumStatus3];
        TextBlock[] values = [MomentumValue1, MomentumValue2, MomentumValue3];
        for (int i = 0; i < buttons.Length; i++)
        {
            bool visible = i < customers.Length;
            buttons[i].Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (!visible)
                continue;
            buttons[i].Tag = customers[i];
            companies[i].Text = customers[i].Company;
            details[i].Text = $"{customers[i].Industry} · {customers[i].Owner}";
            statuses[i].Text = customers[i].Status;
            values[i].Text = customers[i].AccountValue;
        }
    }

    private static string Greeting() =>
        DateTime.Now.Hour < 12 ? "morning" : DateTime.Now.Hour < 18 ? "afternoon" : "evening";

    private void OpenOperations_Click(object sender, RoutedEventArgs e)
    {
        object? tag = (sender as FrameworkElement)?.Tag;
        if (tag is WorkTask)
        {
            MainWindow.Instance.Navigate("Tasks");
            return;
        }
        if (tag is Customer customer)
        {
            MainWindow.Instance.NavigateToCustomer(customer.Id);
            return;
        }

        string action = tag as string ?? "";
        switch (action)
        {
            case "Revenue forecast":
            case "Northwind expansion":
            case "Adventure Works opportunity":
            case "Tailspin Toys renewal":
            case "Account pipeline":
                MainWindow.Instance.Navigate("Customers");
                break;
            case "Overdue work":
            case "Fabrikam renewal":
            case "Contoso follow-up":
            case "Priority work":
                MainWindow.Instance.Navigate("Tasks");
                break;
            default:
                MainWindow.Instance.Navigate("Tickets");
                break;
        }
    }

    private void OpenActivity_Click(object sender, RoutedEventArgs e)
    {
        string activity = (sender as FrameworkElement)?.Tag as string ?? "";
        MainWindow.Instance.Navigate(activity.Contains("follow-up", StringComparison.OrdinalIgnoreCase) ? "Tasks" :
            activity.Contains("account", StringComparison.OrdinalIgnoreCase) || activity.Contains("Northwind", StringComparison.OrdinalIgnoreCase) ? "Customers" : "Tickets");
    }
}
