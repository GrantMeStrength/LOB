---
title: Connect a WinUI app to a database
description: How to use EF Core with SQLite for local data storage in a WinUI 3 line-of-business app, and how to connect to remote databases via a service layer.
ms.topic: how-to
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Connect a WinUI app to a database

Most LOB apps need persistent data storage. WinUI 3 desktop apps can use Entity Framework Core (EF Core) with SQLite for local data, or connect to remote databases through a service layer.

---

## Local database with EF Core + SQLite

### Step 1: Install packages

```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### Step 2: Define your model

```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

### Step 3: Create the DbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MyLobApp", "app.db");

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        options.UseSqlite($"Data Source={dbPath}");
    }
}
```

### Step 4: Load data asynchronously

```csharp
public async Task LoadCustomersAsync()
{
    await using var db = new AppDbContext();
    await db.Database.EnsureCreatedAsync();
    Customers = new ObservableCollection<Customer>(
        await db.Customers.ToListAsync());
}
```

> [!IMPORTANT]
> Always load data with `async`/`await`. Blocking the UI thread with synchronous EF Core calls will freeze the app. Use `DispatcherQueue.TryEnqueue` if you need to update UI-bound collections from a background thread.

---

## Remote database via a service layer

For production LOB apps that connect to SQL Server, PostgreSQL, or Cosmos DB:

1. **Do not embed connection strings in the client app.** A desktop app binary can be decompiled.
2. Create a REST or gRPC API service that handles database access.
3. Use `HttpClient` in the WinUI app to call the service.
4. Authenticate API calls with a bearer token obtained via WAM.

```csharp
public async Task<List<Customer>> GetCustomersAsync()
{
    var response = await _httpClient.GetAsync("api/customers");
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<List<Customer>>()
        ?? new List<Customer>();
}
```

> [!TODO]
> Add a full end-to-end architecture diagram showing WinUI → Azure Function → SQL Database with Managed Identity.

---

## Screenshot

:::image type="content" source="images/03-DatabaseAccess.png" alt-text="Screenshot showing a WinUI app loading customer records from a SQLite database.":::

---

## Credential management

| Approach | When to use |
|----------|-------------|
| `Windows.Security.Credentials.PasswordVault` | Store tokens/passwords locally |
| Azure Key Vault (via service layer) | Production secrets management |
| `appsettings.json` / `IConfiguration` | Non-sensitive config only |

> [!IMPORTANT]
> Never store connection strings, API keys, or passwords in source code or unencrypted files on disk.

---

## Related content

- [EF Core documentation](https://learn.microsoft.com/ef/core/)
- [SQLite provider](https://learn.microsoft.com/ef/core/providers/sqlite/)
- [Web Account Manager (WAM)](https://learn.microsoft.com/windows/apps/develop/security/web-account-manager)
