using System.Collections.ObjectModel;
using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class TicketsPage : Page
{
    private SupportTicket? _selected;
    private bool _showDone;
    private readonly ObservableCollection<TaskLinkOption> _commentLinks = [];
    private TaskLinkOption? _highlightedCommentLink;
    private bool _updatingCommentText;
    private bool IsAiModelInstalled => !string.IsNullOrWhiteSpace(App.State.SelectedAiModel);

    public TicketsPage()
    {
        InitializeComponent();
        App.State.Changed += State_Changed;
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
        RefreshTickets();
    }

    public static Visibility BoolToVisibility(bool value) =>
        value ? Visibility.Visible : Visibility.Collapsed;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string ticketId &&
            int.TryParse(ticketId, out int id) &&
            App.State.Tickets.FirstOrDefault(ticket => ticket.Id == id) is { } ticket)
        {
            _showDone = ticket.IsDone;
            RefreshTickets();
            SelectTicket(ticket);
        }
    }

    private void State_Changed(object? sender, EventArgs e) => RefreshTickets();

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateResponsiveStates();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        App.State.Changed -= State_Changed;
    }

    private void PageLayout_SizeChanged(object sender, SizeChangedEventArgs e) =>
        UpdateResponsiveStates();

    private void UpdateResponsiveStates()
    {
        if (WorkspaceGrid is null ||
            PageScrollViewer is null ||
            WorkspaceLayoutRow is null ||
            TicketHeaderActionsColumn is null ||
            TicketProductColumn is null)
        {
            return;
        }

        double width = PageLayout.ActualWidth;
        if (width <= 0)
            return;

        bool narrowWorkspace = width < 1080;
        bool compactDetails = width < 680;
        PageLayout.Padding = compactDetails ? new Thickness(16) : new Thickness(24);
        PageLayout.Height = narrowWorkspace ? double.NaN : PageScrollViewer.ActualHeight;
        PageScrollViewer.VerticalScrollMode = narrowWorkspace
            ? ScrollMode.Enabled
            : ScrollMode.Disabled;
        PageScrollViewer.VerticalScrollBarVisibility = narrowWorkspace
            ? ScrollBarVisibility.Auto
            : ScrollBarVisibility.Disabled;
        WorkspaceLayoutRow.Height = narrowWorkspace
            ? GridLength.Auto
            : new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.ColumnSpacing = narrowWorkspace ? 0 : 16;
        QueueColumn.Width = new GridLength(1, GridUnitType.Star);
        DetailColumn.Width = narrowWorkspace
            ? new GridLength(0)
            : new GridLength(2, GridUnitType.Star);
        QueueRow.Height = narrowWorkspace
            ? new GridLength(320)
            : new GridLength(1, GridUnitType.Star);
        DetailRow.Height = narrowWorkspace
            ? GridLength.Auto
            : new GridLength(0);
        Grid.SetColumn(DetailScrollViewer, narrowWorkspace ? 0 : 1);
        Grid.SetRow(DetailScrollViewer, narrowWorkspace ? 1 : 0);
        DetailScrollViewer.VerticalScrollMode = narrowWorkspace
            ? ScrollMode.Disabled
            : ScrollMode.Enabled;
        DetailScrollViewer.VerticalScrollBarVisibility = narrowWorkspace
            ? ScrollBarVisibility.Disabled
            : ScrollBarVisibility.Auto;

        TicketHeaderActionsColumn.Width = compactDetails
            ? new GridLength(0)
            : GridLength.Auto;
        TicketHeaderActionsRow.Height = compactDetails
            ? GridLength.Auto
            : new GridLength(0);
        TicketHeaderGrid.RowSpacing = compactDetails ? 12 : 0;
        Grid.SetColumn(TicketHeaderActions, compactDetails ? 0 : 1);
        Grid.SetRow(TicketHeaderActions, compactDetails ? 1 : 0);
        TicketHeaderActions.HorizontalAlignment = compactDetails
            ? HorizontalAlignment.Left
            : HorizontalAlignment.Right;

        TicketProductColumn.Width = compactDetails
            ? new GridLength(0)
            : new GridLength(1, GridUnitType.Star);
        TicketProductRow.Height = compactDetails
            ? GridLength.Auto
            : new GridLength(0);
        TicketMetadataGrid.RowSpacing = compactDetails ? 12 : 0;
        Grid.SetColumn(TicketProductPanel, compactDetails ? 0 : 1);
        Grid.SetRow(TicketProductPanel, compactDetails ? 1 : 0);
    }

    private void RefreshTickets()
    {
        SupportTicket? selected = _selected;
        List<SupportTicket> visible = App.State.Tickets
            .Where(ticket => _showDone || !ticket.IsDone || ticket == selected)
            .OrderBy(ticket => ticket.IsDone)
            .ThenBy(ticket => ticket.Status)
            .ThenByDescending(ticket => ticket.Id)
            .ToList();
        TicketsList.ItemsSource = visible;
        QueueEmptyState.Visibility = visible.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ToggleDoneButton.Content = _showDone ? "Hide completed tickets" : "Show completed tickets";
        ModelUnavailableBanner.Visibility = IsAiModelInstalled ? Visibility.Collapsed : Visibility.Visible;

        if (selected is not null && visible.Contains(selected))
            SelectTicket(selected);
        else
            SelectTicket(visible.FirstOrDefault());
    }

    private void SelectTicket(SupportTicket? ticket)
    {
        foreach (SupportTicket candidate in App.State.Tickets)
            candidate.IsSelected = candidate == ticket;
        _selected = ticket;
        if (ticket is null)
        {
            DetailPanel.Visibility = Visibility.Collapsed;
            EmptyDetailPanel.Visibility = Visibility.Visible;
            return;
        }

        EmptyDetailPanel.Visibility = Visibility.Collapsed;
        DetailPanel.Visibility = Visibility.Visible;
        TicketNumberText.Text = ticket.Number;
        TicketStatusText.Text = ticket.StatusLabel;
        AutomationProperties.SetName(
            TicketStatusButton,
            $"Change ticket status, currently {ticket.StatusLabel}");
        TicketSubjectText.Text = ticket.Subject;
        TicketCustomerText.Text = ticket.Customer;
        TicketProductText.Text = ticket.Product;
        TicketDescriptionText.Text = ticket.Description;
        CommentsList.ItemsSource = ticket.Comments;
        TriageResult.Visibility = ticket.HasTriage ? Visibility.Visible : Visibility.Collapsed;
        TicketCategoryText.Text = ticket.Category ?? "";
        TicketSummaryText.Text = ticket.Summary ?? "";
        TriageProgress.IsActive = ticket.IsProcessing;
        TriageProgress.Visibility = ticket.IsProcessing ? Visibility.Visible : Visibility.Collapsed;
        TriageButton.IsEnabled = IsAiModelInstalled && !ticket.IsProcessing;
        TicketStatusButton.IsEnabled = true;
    }

    private void TicketList_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is SupportTicket ticket)
            SelectTicket(ticket);
    }

    private void OpenAiSettings_Click(object sender, RoutedEventArgs e) =>
        MainWindow.Instance.Navigate("Settings");

    private void ToggleDone_Click(object sender, RoutedEventArgs e)
    {
        _showDone = !_showDone;
        RefreshTickets();
    }

    private async void Triage_Click(object sender, RoutedEventArgs e)
    {
        if (!IsAiModelInstalled)
            return;

        if (_selected is not { } ticket)
            return;

        ticket.IsProcessing = true;
        SelectTicket(ticket);
        await Task.Delay(350);
        string text = $"{ticket.Subject} {ticket.Description}";
        ticket.Category =
            text.Contains("invoice", StringComparison.OrdinalIgnoreCase) || text.Contains("charge", StringComparison.OrdinalIgnoreCase) ? "Billing" :
            text.Contains("password", StringComparison.OrdinalIgnoreCase) || text.Contains("locked", StringComparison.OrdinalIgnoreCase) ? "Account" :
            text.Contains("feature", StringComparison.OrdinalIgnoreCase) || text.Contains("offline", StringComparison.OrdinalIgnoreCase) ? "Feature Request" :
            "Technical";
        ticket.Summary = $"{ticket.Customer} reports {ticket.Subject.ToLowerInvariant()}. The team should review the {ticket.Product} context and coordinate the next customer response.";
        ticket.IsProcessing = false;
        SelectTicket(ticket);
    }

    private void MarkPending_Click(object sender, RoutedEventArgs e) => SetStatus("Pending");
    private void MarkInProgress_Click(object sender, RoutedEventArgs e) => SetStatus("In progress");
    private void MarkDone_Click(object sender, RoutedEventArgs e) => SetStatus("Done");

    private void StatusPrimary_Click(SplitButton sender, SplitButtonClickEventArgs args)
    {
        if (_selected is null)
            return;

        SetStatus(_selected.Status == "Pending" ? "In progress" : "Done");
    }

    private void SetStatus(string status)
    {
        if (_selected is null)
            return;
        _selected.Status = status;
        App.State.NotifyChanged();
    }

    private void AddComment_Click(object sender, RoutedEventArgs e) => AddComment();

    private void AddComment()
    {
        if (_selected is null || string.IsNullOrWhiteSpace(NewCommentBox.Text))
            return;
        var comment = new TicketComment("You", NewCommentBox.Text.Trim());
        foreach (TaskLinkOption link in _commentLinks)
            comment.RelatedLinks.Add(link);
        _selected.Comments.Add(comment);
        _updatingCommentText = true;
        NewCommentBox.Text = "";
        _updatingCommentText = false;
        _commentLinks.Clear();
    }

    private List<TaskLinkOption> GetCommentLinkOptions() =>
    [
        .. App.State.Customers
            .OrderBy(customer => customer.Company)
            .Select(customer => new TaskLinkOption(
                "Customer",
                customer.Id.ToString(),
                $"{customer.Company} · {customer.Name}")),
        .. App.State.Tickets
            .Where(ticket => ticket != _selected)
            .OrderBy(ticket => ticket.IsDone)
            .ThenByDescending(ticket => ticket.Id)
            .Select(ticket => new TaskLinkOption(
                "Ticket",
                ticket.Id.ToString(),
                $"{ticket.Number} · {ticket.Subject}"))
    ];

    private void NewComment_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        if (_updatingCommentText || args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        _highlightedCommentLink = null;
        foreach (TaskLinkOption option in _commentLinks
            .Where(option => !sender.Text.Contains($"@{option.MentionText}", StringComparison.OrdinalIgnoreCase))
            .ToList())
        {
            _commentLinks.Remove(option);
        }

        int mentionStart = sender.Text.LastIndexOf('@');
        if (mentionStart < 0)
        {
            sender.ItemsSource = null;
            sender.IsSuggestionListOpen = false;
            return;
        }

        string query = sender.Text[(mentionStart + 1)..].Trim();
        TaskLinkOption? existingMention = _commentLinks.FirstOrDefault(option =>
            query.StartsWith(option.MentionText, StringComparison.OrdinalIgnoreCase));
        if (existingMention is not null && query.Length > existingMention.MentionText.Length)
        {
            sender.IsSuggestionListOpen = false;
            return;
        }

        List<TaskLinkOption> matches = GetCommentLinkOptions()
            .Where(option =>
                !_commentLinks.Any(link => link.Kind == option.Kind && link.Id == option.Id) &&
                (query.Length == 0 ||
                 option.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .Take(8)
            .ToList();
        sender.ItemsSource = matches;
        sender.IsSuggestionListOpen = matches.Count > 0;
    }

    private void NewComment_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is TaskLinkOption option)
            _highlightedCommentLink = option;
    }

    private void NewComment_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        TaskLinkOption? option = args.ChosenSuggestion as TaskLinkOption ??
            (sender.IsSuggestionListOpen ? _highlightedCommentLink : null);
        if (option is null)
        {
            AddComment();
            return;
        }

        InsertCommentMention(option);
    }

    private void CommentSuggestion_Tapped(object sender, TappedRoutedEventArgs args)
    {
        if ((sender as FrameworkElement)?.Tag is not TaskLinkOption option)
            return;

        InsertCommentMention(option);
        args.Handled = true;
    }

    private void InsertCommentMention(TaskLinkOption option)
    {
        int mentionStart = NewCommentBox.Text.LastIndexOf('@');
        string prefix = mentionStart >= 0
            ? NewCommentBox.Text[..mentionStart]
            : NewCommentBox.Text;
        _updatingCommentText = true;
        NewCommentBox.Text = $"{prefix}@{option.MentionText} ";
        NewCommentBox.ItemsSource = null;
        NewCommentBox.IsSuggestionListOpen = false;
        _highlightedCommentLink = null;
        _updatingCommentText = false;
        if (!_commentLinks.Any(link => link.Kind == option.Kind && link.Id == option.Id))
            _commentLinks.Add(option);
    }

    private void OpenCommentLink_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not TaskLinkOption link)
            return;

        if (link.Kind == "Customer" &&
            Guid.TryParse(link.Id, out Guid customerId) &&
            App.State.Customers.Any(customer => customer.Id == customerId))
        {
            MainWindow.Instance.NavigateToCustomer(customerId);
            return;
        }

        MainWindow.Instance.Navigate("Tickets", link.Id);
    }
}
