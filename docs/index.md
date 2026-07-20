---
title: Build line-of-business apps with WinUI — overview
description: A hub for developers building enterprise line-of-business apps with WinUI 3, covering data display, forms, authentication, and deployment.
ms.topic: overview
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Build line-of-business apps with WinUI — overview

This hub is for developers building line-of-business (LOB) apps — internal tools, data-entry apps, reporting dashboards, and enterprise clients — using WinUI 3 and the Windows App SDK.

WinUI 3 offers modern Fluent Design controls, strong data-binding support, and full access to Windows platform APIs. LOB developers coming from WPF, WinForms, or UWP will find familiar XAML patterns alongside new capabilities for authentication, offline data, and enterprise deployment.

Use the sections below to find guidance based on where you are in your journey.

> [!NOTE]
> This is a wayfinding hub. Each section links to existing canonical topics or first-wave stub articles. Stub articles are marked **[Draft]** and contain `> [!TODO]` callouts where content requires SME validation.

> [!TIP]
> **Quick reference:** When building a WinUI 3 LOB app, apply these rules without exception:
>
> **Verified stack (as of these samples):** Windows App SDK **2.3.1** (stable), `net10.0-windows`, .NET SDK **10.0.302+**, MSBuild 18+ required (`dotnet build` recommended over Visual Studio 2022's bundled MSBuild 17.x).
>
> **Always do this:**
> - Use the `Microsoft.UI.Xaml.*` namespace — never `Windows.UI.Xaml.*` (that is UWP)
> - Use `ItemsView` (not `ListView`) for new list/collection UI
> - Use `ObservableCollection<T>` + `x:Bind` for all data binding — never set control properties in code-behind
> - Use `ObservableValidator` (CommunityToolkit.Mvvm) for form validation
> - Load all data asynchronously with `async`/`await`; never block the UI thread
> - Use `WebAccountManager` (WAM) for Entra ID / Microsoft account sign-in
> - Store sensitive values (connection strings, tokens) in Windows Credential Manager — never in source code
> - Use EF Core + SQLite for local structured data storage
>
> **Never do this:**
> - Reference a first-party WinUI 3 DataGrid — it does not exist; use the Community Toolkit DataGrid NuGet package
> - Use `ApplicationView.GetForCurrentView()`, `CoreWindow.GetForCurrentThread()`, or `CoreApplication.MainView` — these are UWP APIs and will not compile in a WinUI 3 desktop app
> - Use `Validation.ErrorTemplate` or `BindingGroup` — these are WPF-only
> - Call `MessageDialog`, `FileOpenPicker`, or `FileSavePicker` without first calling `WinRT.Interop.InitializeWithWindow.Initialize(obj, hwnd)` — it will throw at runtime
> - Run database queries, AI inference, or HTTP calls on the UI thread

---

## Quick decision guide

| I need to... | Recommended approach | Do not use |
|---|---|---|
| Display a list of records | `ItemsView` with a `DataTemplate` | `ListView` for new code; first-party DataGrid (doesn't exist) |
| Display dense tabular/grid data | Community Toolkit `DataGrid` NuGet package | No first-party equivalent |
| Validate form input | `ObservableValidator` from CommunityToolkit.Mvvm | Code-behind event handlers; WPF `Validation` class |
| Show a modal dialog | `ContentDialog` (requires `XamlRoot`) | `MessageDialog` without HWND init |
| Pick a file | `FileOpenPicker` + `InitializeWithWindow` | `OpenFileDialog` (WinForms/WPF only) |
| Sign in with Entra ID or Microsoft account | `WebAccountManager` (WAM) | Raw OAuth `HttpClient` flow |
| Store structured data locally | EF Core + SQLite | Direct file I/O for relational data |
| Call a remote database | EF Core via a service layer (REST/gRPC) | Direct SQL connection with embedded credentials |
| Load data without blocking the UI | `async`/`await` + `DispatcherQueue.TryEnqueue` | Synchronous calls on the UI thread |
| Run on-device AI inference | Phi Silica (Copilot+ PC) or ONNX Runtime | Synchronous inference on the UI thread |

---

## Build a new app

### Display and edit data

- [Display tabular data in a WinUI app](display-tabular-data.md) **[Draft]**
- [Data binding overview](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-overview)
- [Data binding in depth](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-in-depth)
- [Data binding and MVVM](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-and-mvvm)

### Build forms with validation

- [Build a data-entry form with validation](build-validated-form.md) **[Draft]**

### Connect to data

- [Connect a WinUI app to a database](connect-to-a-database.md) **[Draft]**

### Authentication (Entra ID / MSAL)

- [Web Account Manager (WAM)](https://learn.microsoft.com/windows/apps/develop/security/web-account-manager)

### Enterprise deployment

- [Deployment overview](https://learn.microsoft.com/windows/apps/package-and-deploy/deploy-overview)

---

## Modernize an existing app

You can add Windows App SDK APIs incrementally to an existing WPF or WinForms app without rewriting it. See [Use the Windows App SDK in an existing project](https://learn.microsoft.com/windows/apps/windows-app-sdk/use-windows-app-sdk-in-existing-project).

---

## Port from WPF, WinForms, or UWP

### Migrate from WinForms

- [WinForms patterns and their WinUI 3 equivalents](migrate-winforms-patterns.md) **[Draft]**

---

## Add AI to your app

- [Add AI capabilities to a line-of-business WinUI app](ai-for-lob-apps.md) **[Draft]**

---

## Design for productivity

- [Design for productivity in WinUI LOB apps](design-for-lob.md) **[Draft]**

---

## Related content

- [Windows App SDK overview](https://learn.microsoft.com/windows/apps/windows-app-sdk/)
- [WinUI 3 overview](https://learn.microsoft.com/windows/apps/winui/winui3/)
- [LOB samples repo](https://github.com/GrantMeStrength/LOB)
