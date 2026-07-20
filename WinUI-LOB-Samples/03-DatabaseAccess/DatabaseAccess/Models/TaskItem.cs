using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DatabaseAccess.Models;

/// <summary>
/// A single task tracked by the app and persisted to SQLite via EF Core.
/// Derives from <see cref="ObservableObject"/> so two-way bound properties
/// (for example <see cref="IsComplete"/>) raise change notifications.
/// </summary>
public partial class TaskItem : ObservableObject
{
    /// <summary>Primary key. Auto-incremented by SQLite.</summary>
    public int Id { get; set; }

    /// <summary>Short description of the task.</summary>
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    /// <summary>Whether the task has been completed.</summary>
    [ObservableProperty]
    public partial bool IsComplete { get; set; }

    /// <summary>The date the task is due.</summary>
    [ObservableProperty]
    public partial DateTime DueDate { get; set; } = DateTime.Today;
}
