using System.Collections.ObjectModel;
using DashboardAndDailyTasks.Controls;
using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class TasksPage : Page
{
    private TaskSection _section;
    private bool _updatingTaskText;
    private readonly ObservableCollection<TaskLinkOption> _attachedRecords = [];
    private readonly ObservableCollection<WorkTask> _visibleTasks = [];
    private TaskLinkOption? _highlightedSuggestion;
    private string _selectedSort = "Manual order";
    private bool _isPointerOverDragHandle;
    private bool _dragStartedFromHandle;
    private bool _isDragging;
    private bool _suppressSortRefresh;

    private enum TaskSection
    {
        Open,
        Completed,
        Deleted
    }

    public TasksPage()
    {
        InitializeComponent();
        TaskFilter.SelectedItem = OpenFilterItem;
        TaskSortComboBox.SelectedIndex = 0;
        TaskList.ItemsSource = _visibleTasks;
        DueDatePicker.Date = DateTimeOffset.Now;
        App.State.Changed += State_Changed;
        Unloaded += (_, _) => App.State.Changed -= State_Changed;
        Refresh();
    }

    private void State_Changed(object? sender, EventArgs e) => Refresh();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not string taskId ||
            !Guid.TryParse(taskId, out Guid id) ||
            App.State.Tasks.FirstOrDefault(task => task.Id == id) is not { } task)
        {
            return;
        }

        _section = task.IsDeleted
            ? TaskSection.Deleted
            : task.IsComplete
                ? TaskSection.Completed
                : TaskSection.Open;
        TaskFilter.SelectedItem = _section switch
        {
            TaskSection.Completed => CompletedFilterItem,
            TaskSection.Deleted => DeletedFilterItem,
            _ => OpenFilterItem
        };
        Refresh();
        DispatcherQueue.TryEnqueue(() => TaskList.ScrollIntoView(task));
    }

    private void Refresh()
    {
        _visibleTasks.Clear();
        foreach (WorkTask task in SortTasks(App.State.Tasks.Where(IsInCurrentSection)))
            _visibleTasks.Add(task);
        EmptyStateText.Visibility = _visibleTasks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        EmptyStateText.Text = _section switch
        {
            TaskSection.Completed => "No completed tasks.",
            TaskSection.Deleted => "No deleted tasks.",
            _ => "No open tasks."
        };
    }

    private IEnumerable<WorkTask> SortTasks(IEnumerable<WorkTask> tasks) => _selectedSort switch
    {
        "Due date (earliest first)" => tasks.OrderBy(task => task.Due).ThenBy(task => task.Title),
        "Due date (latest first)" => tasks.OrderByDescending(task => task.Due).ThenBy(task => task.Title),
        "Customers first" => tasks
            .OrderByDescending(task => task.RelatedLinks.Any(link => link.Kind == "Customer"))
            .ThenBy(task => task.Due),
        "Tickets first" => tasks
            .OrderByDescending(task => task.RelatedLinks.Any(link => link.Kind == "Ticket"))
            .ThenBy(task => task.Due),
        "Attachments first" => tasks
            .OrderByDescending(task => task.RelatedLinks.Count > 0)
            .ThenBy(task => task.Due),
        "No attachments first" => tasks
            .OrderBy(task => task.RelatedLinks.Count > 0)
            .ThenBy(task => task.Due),
        _ => tasks
    };

    private bool IsInCurrentSection(WorkTask task) => _section switch
    {
        TaskSection.Completed => !task.IsDeleted && task.IsComplete,
        TaskSection.Deleted => task.IsDeleted,
        _ => !task.IsDeleted && !task.IsComplete
    };

    private List<TaskLinkOption> GetTaskLinks() =>
    [
        .. App.State.Customers
            .OrderBy(customer => customer.Company)
            .Select(customer => new TaskLinkOption(
                "Customer",
                customer.Id.ToString(),
                $"{customer.Company} · {customer.Name}")),
        .. App.State.Tickets
            .OrderByDescending(ticket => ticket.Id)
            .Select(ticket => new TaskLinkOption(
                "Ticket",
                ticket.Id.ToString(),
                $"{ticket.Number} · {ticket.Subject}"))
    ];

    private void NewTaskTitle_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        if (_updatingTaskText || args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        _highlightedSuggestion = null;
        foreach (TaskLinkOption option in _attachedRecords
            .Where(option => !sender.Text.Contains($"@{option.MentionText}", StringComparison.OrdinalIgnoreCase))
            .ToList())
        {
            _attachedRecords.Remove(option);
        }

        int mentionStart = sender.Text.LastIndexOf('@');
        if (mentionStart < 0)
        {
            sender.ItemsSource = null;
            sender.IsSuggestionListOpen = false;
            return;
        }

        string query = sender.Text[(mentionStart + 1)..].Trim();
        TaskLinkOption? existingMention = _attachedRecords.FirstOrDefault(option =>
            query.StartsWith(option.MentionText, StringComparison.OrdinalIgnoreCase));
        if (existingMention is not null && query.Length > existingMention.MentionText.Length)
        {
            sender.IsSuggestionListOpen = false;
            return;
        }

        List<TaskLinkOption> matches = GetTaskLinks()
            .Where(option =>
                !_attachedRecords.Any(link => link.Kind == option.Kind && link.Id == option.Id) &&
                (query.Length == 0 ||
                 option.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .Take(8)
            .ToList();
        sender.ItemsSource = matches;
        sender.IsSuggestionListOpen = matches.Count > 0;
    }

    private void NewTaskTitle_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is TaskLinkOption option)
            _highlightedSuggestion = option;
    }

    private void NewTaskTitle_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        TaskLinkOption? option = args.ChosenSuggestion as TaskLinkOption ??
            (sender.IsSuggestionListOpen ? _highlightedSuggestion : null);
        if (option is not null)
        {
            InsertMention(option);
            return;
        }

        AddTask();
    }

    private void TaskSuggestion_Tapped(object sender, TappedRoutedEventArgs args)
    {
        if ((sender as FrameworkElement)?.Tag is not TaskLinkOption option)
            return;

        InsertMention(option);
        args.Handled = true;
    }

    private void InsertMention(TaskLinkOption option)
    {
        int mentionStart = NewTaskTitle.Text.LastIndexOf('@');
        string prefix = mentionStart >= 0 ? NewTaskTitle.Text[..mentionStart] : NewTaskTitle.Text;
        _updatingTaskText = true;
        NewTaskTitle.Text = $"{prefix}@{option.MentionText} ";
        NewTaskTitle.ItemsSource = null;
        NewTaskTitle.IsSuggestionListOpen = false;
        _highlightedSuggestion = null;
        _updatingTaskText = false;
        if (!_attachedRecords.Any(link => link.Kind == option.Kind && link.Id == option.Id))
            _attachedRecords.Add(option);
    }

    private void TaskFilter_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        _section = sender.SelectedItem == DeletedFilterItem
            ? TaskSection.Deleted
            : sender.SelectedItem == CompletedFilterItem
                ? TaskSection.Completed
                : TaskSection.Open;
        Refresh();
    }

    private void TaskSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedSort = TaskSortComboBox.SelectedItem as string ?? "Manual order";
        TaskList.CanReorderItems = true;
        TaskList.CanDragItems = _isPointerOverDragHandle;
        if (!_suppressSortRefresh)
            Refresh();
    }

    private void AddTask_Click(object sender, RoutedEventArgs e) => AddTask();

    private void AddTask()
    {
        string title = NewTaskTitle.Text.Trim();
        if (title.Length == 0)
        {
            TaskInfoBar.Message = "Enter a task.";
            TaskInfoBar.IsOpen = true;
            NewTaskTitle.Focus(FocusState.Programmatic);
            return;
        }

        List<TaskLinkOption> links = [.. _attachedRecords];

        string cleanTitle = title;
        foreach (TaskLinkOption link in links)
            cleanTitle = cleanTitle.Replace($"@{link.MentionText}", "", StringComparison.OrdinalIgnoreCase);
        cleanTitle = string.Join(" ", cleanTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (cleanTitle.Length == 0)
        {
            TaskInfoBar.Message = "Enter task details in addition to the attached records.";
            TaskInfoBar.IsOpen = true;
            return;
        }

        var task = new WorkTask
        {
            Title = cleanTitle,
            InlineText = title,
            Priority = "Medium",
            Due = DueDatePicker.Date ?? DateTimeOffset.Now
        };
        foreach (TaskLinkOption link in links)
            task.RelatedLinks.Add(link);
        App.State.Tasks.Add(task);
        _updatingTaskText = true;
        NewTaskTitle.Text = "";
        _updatingTaskText = false;
        _attachedRecords.Clear();
        App.State.NotifyChanged();
    }

    private void CompleteTask_Click(object sender, RoutedEventArgs e) => App.State.NotifyChanged();

    private void TaskList_DragItemsCompleted(
        ListViewBase sender,
        DragItemsCompletedEventArgs args)
    {
        PersistVisibleOrder();
        _isDragging = false;
        _dragStartedFromHandle = false;
        TaskList.CanDragItems = _isPointerOverDragHandle;
        App.State.NotifyChanged();
    }

    private void PersistVisibleOrder()
    {
        List<WorkTask> reordered = App.State.Tasks.ToList();
        int visibleIndex = 0;
        for (int index = 0; index < reordered.Count; index++)
        {
            if (IsInCurrentSection(reordered[index]))
                reordered[index] = _visibleTasks[visibleIndex++];
        }

        App.State.Tasks.Clear();
        foreach (WorkTask task in reordered)
            App.State.Tasks.Add(task);
    }

    private void TaskList_DragItemsStarting(
        object sender,
        DragItemsStartingEventArgs args)
    {
        if (!_dragStartedFromHandle)
        {
            args.Cancel = true;
            return;
        }

        _isDragging = true;
    }

    private void DragHandle_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _isPointerOverDragHandle = true;
        TaskList.CanDragItems = true;
    }

    private void DragHandle_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _isPointerOverDragHandle = false;
        if (!_dragStartedFromHandle && !_isDragging)
            TaskList.CanDragItems = false;
    }

    private void DragHandle_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_selectedSort != "Manual order")
        {
            PersistVisibleOrder();
            _selectedSort = "Manual order";
            _suppressSortRefresh = true;
            TaskSortComboBox.SelectedIndex = 0;
            _suppressSortRefresh = false;
        }

        _dragStartedFromHandle = true;
        TaskList.CanDragItems = true;
    }

    private void DragHandle_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_isDragging)
            return;

        _dragStartedFromHandle = false;
        TaskList.CanDragItems = _isPointerOverDragHandle;
    }

    private void DragHandle_GotFocus(object sender, RoutedEventArgs e) =>
        ((MoveCursorGrid)sender).Opacity = 1;

    private void DragHandle_LostFocus(object sender, RoutedEventArgs e) =>
        ((MoveCursorGrid)sender).Opacity = 0;

    private void DragHandle_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is not VirtualKey.Up and not VirtualKey.Down ||
            (sender as FrameworkElement)?.DataContext is not WorkTask task)
        {
            return;
        }

        if (_selectedSort != "Manual order")
        {
            PersistVisibleOrder();
            _selectedSort = "Manual order";
            _suppressSortRefresh = true;
            TaskSortComboBox.SelectedIndex = 0;
            _suppressSortRefresh = false;
        }

        int currentIndex = _visibleTasks.IndexOf(task);
        int targetIndex = e.Key == VirtualKey.Up ? currentIndex - 1 : currentIndex + 1;
        if (currentIndex < 0 || targetIndex < 0 || targetIndex >= _visibleTasks.Count)
            return;

        _visibleTasks.Move(currentIndex, targetIndex);
        PersistVisibleOrder();
        App.State.NotifyChanged();
        e.Handled = true;
    }

    private static Grid? GetTaskGrid(object sender) =>
        sender switch
        {
            Grid grid => grid,
            Border { Child: Grid grid } => grid,
            _ => null
        };

    private static Button? GetDeleteButton(object sender) =>
        GetTaskGrid(sender)?.Children
            .OfType<Button>()
            .FirstOrDefault(button =>
                Grid.GetColumn(button) == 4 &&
                button.Visibility == Visibility.Visible);

    private static MoveCursorGrid? GetMoveHandle(object sender) =>
        GetTaskGrid(sender)?.Children
            .OfType<MoveCursorGrid>()
            .FirstOrDefault();

    private void TaskItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (GetDeleteButton(sender) is { } button)
            button.Opacity = 1;
        if (GetMoveHandle(sender) is { } handle)
            handle.Opacity = 1;
    }

    private void TaskItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (GetDeleteButton(sender) is { FocusState: FocusState.Unfocused } button)
            button.Opacity = 0;
        if (GetMoveHandle(sender) is { } handle)
            handle.Opacity = 0;
    }

    private void TaskDeleteButton_GotFocus(object sender, RoutedEventArgs e) =>
        ((Button)sender).Opacity = 1;

    private void TaskDeleteButton_LostFocus(object sender, RoutedEventArgs e) =>
        ((Button)sender).Opacity = 0;

    private void RemoveTask_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is WorkTask task)
            task.IsDeleted = true;
        App.State.NotifyChanged();
    }

    private void RestoreTask_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is WorkTask task)
            task.IsDeleted = false;
        App.State.NotifyChanged();
    }

    private void OpenRelatedRecord_Click(object sender, RoutedEventArgs e)
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

        MainWindow.Instance.Navigate(
            link.Kind == "Ticket" ? "Tickets" : "Tasks",
            link.Kind == "Ticket" ? link.Id : null);
    }
}
