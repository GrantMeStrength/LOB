using DashboardAndDailyTasks.Models;
using DashboardAndDailyTasks.Pages;
using DashboardAndDailyTasks.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace DashboardAndDailyTasks;

public sealed partial class MainWindow : Window
{
    private const int InitialWidth = 1200;
    private const int InitialHeight = 800;

    public static MainWindow Instance { get; private set; } = null!;
    private ShellSearchResult? _highlightedSearchResult;
    private bool _isSynchronizingNavigationSelection;

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        SetInitialWindowSize();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        AppWindow.SetIcon("Assets\\AppIcon.ico");
        RootGrid.ActualThemeChanged += RootGrid_ActualThemeChanged;
        RootGrid.RequestedTheme = App.State.Theme;
        UpdateTitleBarTheme();
        string destination = string.IsNullOrWhiteSpace(App.State.DefaultPage)
            ? "Dashboard"
            : App.State.DefaultPage;
        Navigate(destination);
    }

    private void SetInitialWindowSize()
    {
        nint hwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        double scale = GetDpiForWindow(hwnd) / 96d;
        AppWindow.Resize(new SizeInt32(
            (int)Math.Ceiling(InitialWidth * scale),
            (int)Math.Ceiling(InitialHeight * scale)));
    }

    private void RootGrid_ActualThemeChanged(FrameworkElement sender, object args) =>
        UpdateTitleBarTheme();

    private void UpdateTitleBarTheme() =>
        AppWindow.TitleBar.PreferredTheme = RootGrid.ActualTheme == ElementTheme.Dark
            ? TitleBarTheme.Dark
            : TitleBarTheme.Light;

    public void Navigate(string destination, object? parameter = null)
    {
        Type page = destination switch
        {
            "Tickets" => typeof(TicketsPage),
            "Tasks" => typeof(TasksPage),
            "Customers" => typeof(CustomersPage),
            "Settings" => typeof(SettingsPage),
            _ => typeof(DashboardPage)
        };
        if (ContentFrame.CurrentSourcePageType != page || parameter is not null)
            ContentFrame.Navigate(page, parameter, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
        SelectNavigationDestination(destination);
    }

    public void NavigateToCustomer(Guid customerId)
    {
        ContentFrame.Navigate(
            typeof(CustomerDetailsPage),
            customerId,
            new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void SelectNavigationDestination(string destination)
    {
        _isSynchronizingNavigationSelection = true;
        try
        {
            NavView.SelectedItem = destination == "Settings"
                ? NavView.SettingsItem
                : NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .FirstOrDefault(item => Equals(item.Tag, destination));
        }
        finally
        {
            _isSynchronizingNavigationSelection = false;
        }
    }

    public async Task ShowAddCustomerAsync() => await CustomerDialog.ShowAsync(RootGrid.XamlRoot);

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args) =>
        NavView.IsPaneOpen = !NavView.IsPaneOpen;

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoBack)
            ContentFrame.GoBack();
    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoForward)
            ContentFrame.GoForward();
    }

    private void TitleBarContent_SizeChanged(object sender, SizeChangedEventArgs e) =>
        GlobalSearchBox.Width = Math.Clamp(e.NewSize.Width - 116, 240, 760);

    private void GlobalSearch_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        _highlightedSearchResult = null;
        string query = sender.Text.Trim();
        if (query.Length == 0)
        {
            sender.ItemsSource = null;
            sender.IsSuggestionListOpen = false;
            return;
        }

        List<ShellSearchResult> results =
        [
            .. App.State.Customers
                .Where(customer =>
                    customer.Company.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    customer.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    customer.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(customer => new ShellSearchResult(
                    "Customer",
                    customer.Company,
                    customer.Name,
                    "\uE716",
                    customer)),
            .. App.State.Tickets
                .Where(ticket =>
                    query.Equals("ticket", StringComparison.OrdinalIgnoreCase) ||
                    query.Equals("tickets", StringComparison.OrdinalIgnoreCase) ||
                    ticket.Number.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    ticket.Subject.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    ticket.Customer.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(ticket => new ShellSearchResult(
                    "Ticket",
                    $"{ticket.Number} · {ticket.Subject}",
                    ticket.Customer,
                    "\uE8BD",
                    ticket)),
            .. App.State.Tasks
                .Where(task =>
                    !task.IsDeleted &&
                    (task.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     task.RelatedSummary.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(task => new ShellSearchResult(
                    "Task",
                    task.Title,
                    task.DueText,
                    "\uE73E",
                    task))
        ];
        results = results
            .OrderBy(result => result.Title.StartsWith(query, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(result => result.Kind)
            .ThenBy(result => result.Title)
            .Take(8)
            .ToList();
        sender.ItemsSource = results;
        sender.IsSuggestionListOpen = results.Count > 0;
    }

    private void GlobalSearch_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is ShellSearchResult result)
            _highlightedSearchResult = result;
    }

    private void GlobalSearch_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ShellSearchResult? result = args.ChosenSuggestion as ShellSearchResult ??
            (sender.IsSuggestionListOpen ? _highlightedSearchResult : null);
        if (result is not null)
            OpenSearchResult(result);
    }

    private void GlobalSearchSuggestion_Tapped(object sender, TappedRoutedEventArgs args)
    {
        if ((sender as FrameworkElement)?.Tag is not ShellSearchResult result)
            return;

        OpenSearchResult(result);
        args.Handled = true;
    }

    private void OpenSearchResult(ShellSearchResult result)
    {
        GlobalSearchBox.Text = "";
        GlobalSearchBox.ItemsSource = null;
        GlobalSearchBox.IsSuggestionListOpen = false;
        _highlightedSearchResult = null;

        switch (result.Record)
        {
            case Customer customer:
                NavigateToCustomer(customer.Id);
                break;
            case SupportTicket ticket:
                Navigate("Tickets", ticket.Id.ToString());
                break;
            case WorkTask task:
                Navigate("Tasks", task.Id.ToString());
                break;
        }
    }

    private void SearchAccelerator_Invoked(
        KeyboardAccelerator sender,
        KeyboardAcceleratorInvokedEventArgs args)
    {
        GlobalSearchBox.Focus(FocusState.Keyboard);
        args.Handled = true;
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        BackButton.IsEnabled = ContentFrame.CanGoBack;
        ForwardButton.IsEnabled = ContentFrame.CanGoForward;

        string? destination = e.SourcePageType == typeof(DashboardPage) ? "Dashboard" :
            e.SourcePageType == typeof(TicketsPage) ? "Tickets" :
            e.SourcePageType == typeof(TasksPage) ? "Tasks" :
            e.SourcePageType is not null &&
                (e.SourcePageType == typeof(CustomersPage) || e.SourcePageType == typeof(CustomerDetailsPage)) ? "Customers" :
            e.SourcePageType == typeof(SettingsPage) ? "Settings" : null;
        if (destination is null)
            return;

        SelectNavigationDestination(destination);
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_isSynchronizingNavigationSelection)
            return;

        if (args.IsSettingsSelected) Navigate("Settings");
        else if (args.SelectedItem is NavigationViewItem item && item.Tag is string destination) Navigate(destination);
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem { Tag: "Customers" } &&
            ContentFrame.CurrentSourcePageType == typeof(CustomerDetailsPage))
        {
            Navigate("Customers");
        }
    }

    public void ApplyTheme(ElementTheme theme)
    {
        App.State.Theme = theme;
        RootGrid.RequestedTheme = theme;
        UpdateTitleBarTheme();
    }
}
