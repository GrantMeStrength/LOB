---
title: Connect a WinUI app to a database
description: Connect a WinUI 3 app to a database using Entity Framework Core, load data asynchronously off the UI thread, and cache data for offline use.
ms.topic: how-to
ms.date: 07/29/2026
author: GrantMeStrength
ms.author: jken
---

# Connect a WinUI app to a database

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

Line-of-business apps frequently read from and write to a database — an on-device SQLite database, a local SQL Server instance, or a remote database accessed through a service layer. Two data-access options cover most LOB needs: SQLite for local and embedded data, and [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) for connecting to SQL Server. This article describes how to connect a WinUI 3 app to a database using Entity Framework Core (EF Core), load data asynchronously so the UI thread stays responsive, and cache data locally for offline scenarios.

## Overview

:::image type="content" source="images/03-database-access.png" alt-text="The WinUI 3 database access sample showing a task tracker app with a list of tasks loaded from SQLite via EF Core. Each task shows a title, due date, and a CheckBox for completion status.":::

| Scenario | Recommended approach |
|---|---|
| On-device data (settings, local records, offline cache) | EF Core + SQLite |
| Enterprise SQL Server (on-premises or Azure SQL) | `Microsoft.Data.SqlClient`, directly or through the EF Core SQL Server provider |
| Read-only data from an API | `HttpClient` + JSON deserialization, with optional local cache |

`Microsoft.Data.SqlClient` is the current, actively maintained SQL Server client library for .NET, and it is the right choice for LOB apps that connect to enterprise SQL Server. You can use it directly or through the EF Core SQL Server provider (`Microsoft.EntityFrameworkCore.SqlServer`), which builds on it.

> [!IMPORTANT]
> For security and maintainability, enterprise apps should not connect a client desktop app directly to a shared SQL Server database using embedded credentials. Consider a REST API or gRPC service layer that the WinUI app calls over HTTPS. This article covers both direct (SQLite/local) and service-layer patterns.

> [!TODO] SME validation: confirm the recommended architecture for WinUI 3 LOB apps connecting to corporate databases. Determine whether direct EF Core + SQL Server is acceptable in trusted domain-joined scenarios, or whether a service layer is always the correct guidance.

## When to use EF Core

Entity Framework Core is appropriate when:

- You need an ORM to map C# objects to database tables without writing raw SQL.
- You want to minimize database-specific code.
  - EF Core supports many relational databases through a provider model, including SQL Server, SQLite, PostgreSQL, MySQL, MariaDB, Oracle, and others. This allows most application code to remain unchanged when switching database providers, although some provider-specific features may require changes.
- You want database migrations to manage schema evolution.

EF Core runs on .NET and is fully supported in WinUI 3 apps built with .NET.

> [!TODO] SME validation: confirm that EF Core runs correctly in WinUI 3 desktop apps (both packaged and unpackaged). Note any packaging or sandboxing considerations that affect database file access in packaged apps (for example, LocalApplicationData path usage).

## Prerequisites

- A WinUI 3 project targeting .NET
- EF Core NuGet packages (`Microsoft.EntityFrameworkCore` and a provider such as `Microsoft.EntityFrameworkCore.Sqlite` or `Microsoft.EntityFrameworkCore.SqlServer`)

> [!TODO] Confirm minimum EF Core version tested with the current stable Windows App SDK and .NET version. Link to the EF Core Getting Started documentation on learn.microsoft.com.

## Steps

### Step 1: Add EF Core NuGet packages

Add the EF Core and SQLite provider packages:

```console
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

This pulls in `Microsoft.EntityFrameworkCore` as a dependency. Also pin the SQLite native library to avoid a known CVE (see the [Connection strings and secrets](#connection-strings-and-secrets) section below).

### Step 2: Define the data model and DbContext

> [!TODO] Provide a C# example defining:
> - A simple entity class (for example, `Customer` with `Id`, `Name`, `Email`).
> - A `DbContext` subclass that exposes a `DbSet<Customer>`.
> - A connection string pointing to a local SQLite database file in the app's data folder.
>
> For packaged apps, the database file should live under `Windows.Storage.ApplicationData.Current.LocalFolder.Path`. For unpackaged apps, use `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`.
>
> Validate both paths with SME. Do not publish code examples until SME review is complete.

### Step 3: Run migrations and create the database

For a new app, `context.Database.EnsureCreatedAsync()` is the quickest way to create the SQLite database file on first launch. However, `EnsureCreated` does not support schema changes — if you add or rename a column, it will not update an existing database.

For a production LOB app, use EF Core **migrations** (`dotnet ef migrations add` / `dotnet ef database update`) so schema changes can be applied without data loss.

> [!TODO] Add a brief example showing how to add and apply an EF Core migration. Link to the EF Core migrations documentation on learn.microsoft.com.

### Step 4: Load data asynchronously

Loading data on the UI thread blocks the UI and causes the app to become unresponsive. Always load data on a background thread and marshal results back to the UI thread.

In WinUI 3, use `async`/`await` in your ViewModel and ensure that UI property updates are dispatched on the UI thread.

> [!TODO] Provide a C# example showing:
> - An async ViewModel method (for example, `LoadCustomersAsync`) that calls `await context.Customers.ToListAsync()`.
> - Assigning the result to an `ObservableCollection<Customer>` property that the View is bound to.
> - Handling exceptions (database unavailable, connection timeout) and surfacing an error message in the UI.
>
> Clarify with SME whether `DispatcherQueue.TryEnqueue` is required when setting `ObservableCollection` from a background thread in WinUI 3, or whether this is handled automatically by the binding system.

### Step 5: Save changes

> [!TODO] Provide a C# example showing:
> - Adding a new entity to the `DbSet<T>` and calling `await context.SaveChangesAsync()`.
> - Handling `DbUpdateException` for constraint violations (for example, duplicate keys).
>
> Validate with SME.

### Step 6: Implement offline caching

For apps that need to work without a network connection, a local SQLite cache can mirror data from a remote source. The app writes to the local cache and syncs with the remote service when connectivity is restored.

> [!TODO] Describe a practical offline caching pattern for WinUI 3 LOB apps:
> - Storing a local SQLite copy of remote data.
> - Detecting network availability.
> - Queuing writes when offline and syncing when online.
>
> This is a complex topic. SME should determine the level of detail appropriate for this article vs. a dedicated offline sync how-to. Do not publish without SME input.

> [!TODO] Evaluate whether to link to Azure Data Sync, Microsoft Sync Framework, or a custom sync implementation. Determine what is supported and recommended for WinUI 3 LOB apps.

## Connection strings and secrets

> [!IMPORTANT]
> Never embed database connection strings or credentials in source code. For local SQLite, no credentials are needed. For SQL Server or cloud databases, use environment variables, Windows Credential Manager, or a secrets management service.

> [!WARNING]
> The `SQLitePCLRaw.lib.e_sqlite3` package (a transitive dependency of EF Core + SQLite) has a known CVE in versions prior to 2.1.12. Pin `SQLitePCLRaw.bundle_e_sqlite3` to version **2.1.12 or later** in your project file to stay clean until EF Core ships a newer transitive default:
>
> ```xml
> <PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.12" />
> ```

> [!TODO] Link to additional guidance on secrets management for WinUI 3 apps.

## Store credentials securely

Instead of keeping SQL Server credentials or API tokens in a configuration file or connection string, store them in the Windows Credential Locker with `Windows.Security.Credentials.PasswordVault`. Credentials saved this way are encrypted per user and roam with the user's Microsoft account on domain-joined and Entra-joined devices.

Store a credential:

```csharp
using Windows.Security.Credentials;

var vault = new PasswordVault();
vault.Add(new PasswordCredential("Contoso.LOB.Database", userName, password));
```

Retrieve it later:

```csharp
using Windows.Security.Credentials;

var vault = new PasswordVault();
try
{
    PasswordCredential credential = vault.Retrieve("Contoso.LOB.Database", userName);
    credential.RetrievePassword();
    string password = credential.Password;
    // Use the password to build the connection at runtime.
}
catch (Exception)
{
    // No stored credential for this resource/user — prompt the user to sign in.
}
```

If you don't know the user name in advance, enumerate stored credentials for a resource with `vault.FindAllByResource("Contoso.LOB.Database")`.

> [!TODO] SME validation: confirm `PasswordVault` behavior and packaging requirements (packaged vs. unpackaged) for WinUI 3 desktop apps, and the recommended pattern for building a connection string from a retrieved credential.

See [Credential locker](../../develop/security/credential-locker.md) for more on storing tokens and credentials securely using the Windows Credential Manager.

## Get the sample

The database access sample is in the [LOB samples repo](https://github.com/GrantMeStrength/LOB) under the `WinUI-LOB-Samples/03-DatabaseAccess/` folder.

The sample adapts to the system theme. The following screenshots show it running in the light and dark themes.

:::image type="content" source="images/03-database-access.png" alt-text="The database access sample running in the light theme, showing a task tracker list loaded from SQLite via EF Core.":::

:::image type="content" source="images/03-database-access-dark.png" alt-text="The database access sample running in the dark theme, showing a task tracker list loaded from SQLite via EF Core.":::

> [!NOTE]
> The sample repo URL may change if the repo is renamed or moved; this article will be updated if that happens.

## Related content

- [Data binding overview](../../develop/data-binding/data-binding-overview.md)
- [Data binding and MVVM](../../develop/data-binding/data-binding-and-mvvm.md)
- [Build a data-entry form with validation](build-validated-form.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
- [Credential locker](../../develop/security/credential-locker.md)
