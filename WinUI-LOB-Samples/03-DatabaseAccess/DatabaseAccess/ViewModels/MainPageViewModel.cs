using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess.Models;
using DatabaseAccess.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
    // Completion toggles and task creation can overlap through async UI events.
    // Serialize SQLite mutations so the last visible state is the state persisted.
    private readonly SemaphoreSlim _mutationLock = new(1, 1);
    private readonly Dictionary<int, bool> _persistedCompletion = new();
    private bool _isRestoringTaskState;

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
    [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
    public partial bool IsBusy { get; set; }

    /// <summary>True when there are no tasks to show.</summary>
    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasOperationError))]
    public partial string OperationError { get; set; } = string.Empty;

    public bool HasOperationError => !string.IsNullOrWhiteSpace(OperationError);

    /// <summary>
    /// Ensures the database exists and loads the initial task list.
    /// </summary>
    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            await _taskService.InitializeAsync();
            await LoadTasksAsync();
            OperationError = string.Empty;
        }
        catch (Exception ex) when (IsPersistenceException(ex))
        {
            OperationError = "Tasks could not be loaded from the local database. Check storage access, then retry.";
        }
        finally
        {
            IsBusy = false;
        }
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
            _persistedCompletion.Clear();
            foreach (var item in items)
            {
                item.PropertyChanged += OnTaskPropertyChanged;
                Tasks.Add(item);
                _persistedCompletion[item.Id] = item.IsComplete;
            }

            IsEmpty = Tasks.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAddTask() => !IsBusy && !string.IsNullOrWhiteSpace(NewTaskTitle);

    [RelayCommand(CanExecute = nameof(CanAddTask))]
    private async Task AddTaskAsync()
    {
        IsBusy = true;
        await _mutationLock.WaitAsync();
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
            OperationError = string.Empty;
        }
        catch (Exception ex) when (IsPersistenceException(ex))
        {
            OperationError = "The new task could not be saved to the local database.";
        }
        finally
        {
            _mutationLock.Release();
            IsBusy = false;
        }
    }

    private async void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isRestoringTaskState
            || sender is not TaskItem item
            || e.PropertyName != nameof(TaskItem.IsComplete))
        {
            return;
        }

        bool requestedValue = item.IsComplete;
        await _mutationLock.WaitAsync();
        try
        {
            await _taskService.SetCompletionAsync(item.Id, requestedValue);
            _persistedCompletion[item.Id] = requestedValue;
            OperationError = string.Empty;
        }
        catch (Exception ex) when (IsPersistenceException(ex))
        {
            // A later click may have changed the checkbox while this write waited
            // for the lock. Only roll back the value owned by this handler, and
            // restore the last value confirmed by SQLite rather than guessing.
            bool ownsCurrentValue = item.IsComplete == requestedValue;
            bool hasPersistedValue = _persistedCompletion.TryGetValue(item.Id, out bool persistedValue);
            bool restored = ownsCurrentValue && hasPersistedValue;
            if (restored)
            {
                _isRestoringTaskState = true;
                try
                {
                    item.IsComplete = persistedValue;
                }
                finally
                {
                    _isRestoringTaskState = false;
                }
            }

            OperationError = restored
                ? "The task update could not be saved. The last saved value was restored."
                : "The task update could not be saved.";
        }
        finally
        {
            _mutationLock.Release();
        }
    }

    private static bool IsPersistenceException(Exception exception) =>
        exception is IOException
            or UnauthorizedAccessException
            or SqliteException
            or DbUpdateException
            or TaskNotFoundException;
}
