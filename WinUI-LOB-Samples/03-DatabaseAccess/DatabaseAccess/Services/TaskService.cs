using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseAccess.Data;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Services;

/// <summary>
/// Encapsulates all EF Core data access for tasks. A fresh
/// <see cref="TaskDbContext"/> is created per operation (DbContext is not
/// thread-safe) and every call is wrapped in <see cref="Task.Run"/> so the
/// SQLite work executes on a background thread — never on the UI thread.
/// </summary>
public sealed class TaskService
{
    /// <summary>
    /// Ensures the database and schema exist. Safe to call on startup.
    /// </summary>
    public Task InitializeAsync() => Task.Run(async () =>
    {
        using var db = new TaskDbContext();
        await db.Database.EnsureCreatedAsync().ConfigureAwait(false);
    });

    /// <summary>
    /// Loads all tasks ordered by due date, on a background thread.
    /// </summary>
    public Task<List<TaskItem>> GetAllAsync() => Task.Run(async () =>
    {
        using var db = new TaskDbContext();
        return await db.Tasks
            .AsNoTracking()
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    });

    /// <summary>
    /// Inserts a new task and returns the persisted entity (with its Id).
    /// </summary>
    public Task<TaskItem> AddAsync(TaskItem item) => Task.Run(async () =>
    {
        using var db = new TaskDbContext();
        db.Tasks.Add(item);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return item;
    });

    /// <summary>
    /// Updates the completion state of an existing task and saves immediately.
    /// </summary>
    public Task SetCompletionAsync(int id, bool isComplete) => Task.Run(async () =>
    {
        using var db = new TaskDbContext();
        var existing = await db.Tasks.FindAsync(id).ConfigureAwait(false);
        if (existing is null)
        {
            return;
        }

        existing.IsComplete = isComplete;
        await db.SaveChangesAsync().ConfigureAwait(false);
    });
}
