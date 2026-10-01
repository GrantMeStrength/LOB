# Dashboard and daily tasks

This WinUI 3 sample demonstrates a responsive line-of-business dashboard for
customer portfolio management, support tickets, and daily tasks. It uses the
Windows App SDK, packaged app activation, compiled XAML bindings, and the
CommunityToolkit.Mvvm source generators.

This is sample 6 in the
[WinUI 3 line-of-business samples](../../README.md) repository.

## Features

- Responsive dashboard with configurable widgets
- Search across customers, tickets, and tasks
- Customer portfolio table and card views
- Ticket queue with comments and optional model-assisted triage
- Daily task creation, record mentions, keyboard reordering, and recovery
- Light, dark, system, and High Contrast theme support
- Keyboard navigation and UI Automation names for primary workflows

The application uses generated in-memory sample data. It does not connect to a
service, persist business records, or send data to an AI model.

## Prerequisites

- Windows 10 version 1809 or later
- Developer Mode enabled
- .NET 10 SDK
- WinApp CLI 0.7 or later for the recommended build-and-run workflow

The solution targets x64, ARM64, and x86. Use x64 unless you specifically need
another architecture; do not build this WinUI application as Any CPU.

## Build and run

From this directory:

```powershell
dotnet restore .\DashboardAndDailyTasks.slnx
dotnet build .\DashboardAndDailyTasks.slnx -c Debug -p:Platform=x64
winapp run .\DashboardAndDailyTasks\DashboardAndDailyTasks.csproj --arch x64
```

The app is packaged. Launch it through `winapp run`, Visual Studio, or its
installed package rather than running the generated executable directly.

## Project structure

| Folder | Purpose |
| --- | --- |
| `Models` | Observable customer, ticket, and task data |
| `ViewModels` | Presentation state and filtering logic |
| `Pages` | Dashboard, portfolio, ticket, task, detail, and settings UI |
| `Services` | Shared sample state and dialog coordination |
| `Controls` | Small reusable layout and input controls |

`AppState` is intentionally an in-memory sample service. A production app
should replace it with injected persistence and domain services, keep secrets
out of source control, and handle authentication and authorization at service
boundaries.

## Accessibility and theming

The UI uses semantic WinUI controls, heading levels, keyboard accelerators,
automation names, theme resources, and explicit High Contrast resources.
When changing the UI, verify keyboard-only operation, 200% text scaling, a
Windows Contrast theme, and both light and dark themes.

## Quality checks

The project references the WinUI analyzer as a private build dependency. Treat
its `WUI` diagnostics as actionable defects unless a narrowly documented
exception applies.
