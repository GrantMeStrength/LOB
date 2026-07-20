using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess.Models;
using DatabaseAccess.Services;

namespace DatabaseAccess.ViewModels;

/// <summary>
/// ViewModel for the task tracker page. Owns the observable task collection,
/// the new-task form fields, and the commands that talk to <see cref="TaskService"/>.
/// All data access is delegated to the service, which runs EF Core work off the
/// UI thread.
/// </summary>
public partial class MainPageViewModel : ObservableObject
{
    private readonly TaskService _taskService = new();

    /// <summary>The tasks shown in the UI, bound to an ItemsView.</summary>
    public ObservableCollection<TaskItem> Tasks { get; } = new();

    /// <summary>Title entered in the new-task form.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
    public partial string NewTaskTitle { get; set; } = string.Empty;

    /// <summary>Due date selected in the new-task form.</summary>
    [ObservableProperty]
    public partial DateTimeOffset NewTaskDueDate { get; set; } = DateTimeOffset.Now;

    /// <summary>True while a load/save operation is in flight.</summary>
    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    /// <summary>True when there are no tasks to show.</summary>
    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    /// <summary>
    /// Ensures the database exists and loads the initial task list.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _taskService.InitializeAsync();
        await LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _taskService.GetAllAsync();

            foreach (var existing in Tasks)
            {
                existing.PropertyChanged -= OnTaskPropertyChanged;
            }

            Tasks.Clear();
            foreach (var item in items)
            {
                item.PropertyChanged += OnTaskPropertyChanged;
                Tasks.Add(item);
            }

            IsEmpty = Tasks.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewTaskTitle);

    [RelayCommand(CanExecute = nameof(CanAddTask))]
    private async Task AddTaskAsync()
    {
        IsBusy = true;
        try
        {
            var item = new TaskItem
            {
                Title = NewTaskTitle.Trim(),
                DueDate = NewTaskDueDate.DateTime,
                IsComplete = false
            };

            await _taskService.AddAsync(item);

            // Reset the form and refresh the list from the database.
            NewTaskTitle = string.Empty;
            NewTaskDueDate = DateTimeOffset.Now;
            await LoadTasksAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TaskItem item || e.PropertyName != nameof(TaskItem.IsComplete))
        {
            return;
        }

        // Persist the toggled completion state immediately.
        await _taskService.SetCompletionAsync(item.Id, item.IsComplete);
    }
}
