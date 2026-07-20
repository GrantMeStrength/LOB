# WinUI 3 Line-of-Business Sample Hub

Four self-contained WinUI 3 (Windows App SDK) desktop samples demonstrating common
line-of-business patterns. Each sample is an independent, buildable Visual Studio
solution. They are linked from the documentation at
[learn.microsoft.com/windows/apps/get-started/line-of-business/](https://learn.microsoft.com/windows/apps/get-started/line-of-business/).

## Samples

| # | Folder | Demonstrates |
|---|--------|--------------|
| 1 | [`WinUI-LOB-Samples/01-TabularData`](WinUI-LOB-Samples/01-TabularData) | `ItemsView` + `DataTemplate` card layout **and** a Community Toolkit `DataGrid` with sortable columns |
| 2 | [`WinUI-LOB-Samples/02-ValidatedForm`](WinUI-LOB-Samples/02-ValidatedForm) | Input validation with `ObservableValidator` (`[Required]`, `[EmailAddress]`), per-keystroke validation, inline errors, Save gated on `HasErrors` |
| 3 | [`WinUI-LOB-Samples/03-DatabaseAccess`](WinUI-LOB-Samples/03-DatabaseAccess) | EF Core + SQLite task tracker, all data access **async and off the UI thread** |
| 4 | [`WinUI-LOB-Samples/04-DesignShowcase`](WinUI-LOB-Samples/04-DesignShowcase) | `NavigationView`, Mica backdrop, compact/normal density toggle, light/dark theme toggle, summary card grid |

## Screenshots

| Sample | Screenshot |
|--------|-----------|
| 1 — TabularData (cards) | ![Cards / ItemsView](screenshots/01-TabularData-cards.png) |
| 1 — TabularData (DataGrid, sorted) | ![Sortable DataGrid](screenshots/01-TabularData-datagrid.png) |
| 2 — ValidatedForm (invalid email, Save disabled) | ![Validated form](screenshots/02-ValidatedForm.png) |
| 3 — DatabaseAccess (task tracker) | ![Task tracker](screenshots/03-DatabaseAccess.png) |
| 4 — DesignShowcase (dashboard + Mica) | ![Design showcase](screenshots/04-DesignShowcase.png) |

## Shared conventions (all samples)

- **WinUI 3 desktop**, Windows App SDK **2.3.1** (stable channel), C# only, `net10.0-windows`.
- **MVVM** throughout — ViewModels derive from `ObservableObject` / `ObservableValidator` (`CommunityToolkit.Mvvm`).
- **`x:Bind`** for all data binding (never `Binding` — see the DataGrid exception below).
- **`ItemsView`** for all collection UI (never `ListView`).
- **`Microsoft.UI.Xaml.*`** only — no UWP `Windows.UI.Xaml.*`, `ApplicationView`, `CoreWindow`, or `CoreApplication`.
- **System theme brushes only** — no hardcoded colors.
- All data loading is **async**; EF Core queries never run on the UI thread.
- No connection strings or credentials in source; the SQLite path is built at runtime from `LocalApplicationData`.

## Building & running

Each sample requires .NET SDK **10.0.302+**, Windows App SDK, Developer Mode enabled, and the
`winapp` CLI. From a sample's project folder:

```powershell
dotnet build
winapp run          # launch the packaged app (do not run the .exe directly)
```

Every sample builds with **zero errors and zero warnings** and has been launched and validated.

## Key findings (SME-relevant discoveries)

These are the "known unknowns" that were resolved while building the samples — the docs should
reflect these exact values:

1. **Community Toolkit DataGrid NuGet package name (Sample 1).**
   The intuitive/guessed name `CommunityToolkit.WinUI.Controls.DataGrid` **does not exist on
   NuGet**. The DataGrid has *not* migrated to the 8.x `CommunityToolkit.WinUI.Controls.*`
   segmented naming. The correct, current package is:
   ```
   CommunityToolkit.WinUI.UI.Controls.DataGrid   (v7.1.2)
   xmlns: using:CommunityToolkit.WinUI.UI.Controls
   ```
   It requires merging `ms-appx:///CommunityToolkit.WinUI.UI.Controls.DataGrid/Themes/Generic.xaml`
   into `App.xaml`.

2. **DataGrid columns require `{Binding}`, not `{x:Bind}` (Sample 1).**
   `DataGridBoundColumn.Binding` is typed as a classic `Binding`; the framework does not accept
   `x:Bind` there. This is the one sanctioned exception to the "always `x:Bind`" rule — every
   other binding in the samples uses `x:Bind`.

3. **Compact-density resource dictionary URI (Sample 4).**
   The resource dictionary that resolves at runtime on Windows App SDK 2.3.1 is:
   ```
   ms-appx:///Microsoft.UI.Xaml/DensityStyles/Compact.xaml
   ```
   Toggle density by adding/removing a `ResourceDictionary { Source = <that URI> }` on the root
   element's `Resources.MergedDictionaries` — it must be merged **after** `XamlControlsResources`.

## Open TODOs / SME follow-ups

- **Build toolchain:** `net10.0` requires **MSBuild 18+**. Visual Studio 2022's bundled MSBuild
  (17.14) cannot build these projects, so each sample pins the SDK via `global.json` (10.0.302)
  and builds through the `dotnet` CLI. Ensure the team's standard build host / CI uses a Visual
  Studio version with MSBuild 18, or standardize on `dotnet build`. The `global.json` files can be
  relaxed once that is confirmed.
- **Sample 3 – schema evolution:** uses `EnsureCreatedAsync()` (does not support schema changes).
  Switch to EF Core **migrations** for a production app.
- **Sample 3 – persistence layer:** no edit/delete-task UI yet (spec covered add + complete only).
- **Sample 2 – `Save()` is a stub** (shows an InfoBar and resets the form); wire it to a real
  service layer for an actual LOB app.
- **Sample 3 – supply chain:** the transitive `SQLitePCLRaw.lib.e_sqlite3 2.1.11` carried
  CVE-2025-6965; pinned `SQLitePCLRaw.bundle_e_sqlite3 2.1.12` to stay clean. Bump when EF Core
  ships a newer transitive default.
