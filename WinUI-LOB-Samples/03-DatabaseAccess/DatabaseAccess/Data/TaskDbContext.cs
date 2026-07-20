using System;
using System.IO;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Data;

/// <summary>
/// EF Core database context backing the task tracker with a local SQLite file.
/// The database file lives under the user's LocalApplicationData folder so the
/// app works correctly whether packaged or unpackaged. The connection string is
/// built at runtime — no absolute paths or credentials are embedded in source.
/// </summary>
public sealed class TaskDbContext : DbContext
{
    /// <summary>The tasks table.</summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    /// <summary>
    /// Full path to the SQLite database file, rooted in LocalApplicationData.
    /// </summary>
    public static string DatabasePath { get; } = BuildDatabasePath();

    private static string BuildDatabasePath()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DatabaseAccess");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "tasks.db");
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Build the connection string at runtime from the computed path.
        var connectionString = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath
        }.ToString();

        optionsBuilder.UseSqlite(connectionString);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired();
        });
    }
}
