# WinUI LOB docs — review mirror

These are **draft Learn articles** for building line-of-business (LOB) apps with WinUI, mirrored here so MVPs and SMEs can review the current draft directly on GitHub.

- **Source of truth (when published):** https://learn.microsoft.com/windows/apps/get-started/line-of-business/
- **Tracking PR:** [windows-dev-docs #7160](https://github.com/MicrosoftDocs/windows-dev-docs-pr/pull/7160)
- **Samples:** [`WinUI-LOB-Samples/`](../WinUI-LOB-Samples) (see the sample-to-article map below)

> [!NOTE]
> This is a **temporary review copy** of the in-review Learn articles. It will drift from Learn over time; the Learn articles above remain authoritative. Re-sync or trim this mirror once the PR publishes.

> [!TODO]
> Confirm the final published article slugs once PR #7160 is live, then swap the GitHub links below for the Learn URLs.

## How to review

Each topic links to its draft article and lists the **open questions for SMEs** — the points the draft can't settle without expert confirmation. If you can answer one, comment on the PR or edit the article and remove the matching `> [!TODO]`. Any `> [!TODO]` in an article is an open question, not finished guidance.

## Topics and open SME questions

### Overview — [index.md](index.md)
Wayfinding hub for the LOB doc set.
- Confirm the quick-reference stack list and recommended versions; drop anything not broadly applicable to LOB.
- Link an "Authenticate with Entra ID using MSAL in WinUI" how-to once drafted (confirm the WAM broker setup).
- Link a "WinForms to WinUI" migration guide once authored.
- Add Phi Silica and App Content Search reference links once canonical paths are confirmed.

### Display tabular data — [display-tabular-data.md](display-tabular-data.md)
Showing rows and columns without a first-party DataGrid.
- Update the article when first-party WinUI DataGrid support ships.
- Confirm `ItemsView` is the recommended successor to `ListView` for new apps; is `ListView` in maintenance mode?
- Define the sample scenario (e.g., sortable employee directory) once the control recommendation is validated.
- Add a validated model + `ObservableCollection<T>` ViewModel example.
- Add validated `DataTemplate` + matching header-row XAML using shared column widths.
- Add column-sorting guidance (`CollectionViewSource` or custom sort).
- Add row-selection handling (navigate to detail / open an edit dialog).

### Build a validated form — [build-validated-form.md](build-validated-form.md)
Data-entry form validation (a known WinUI-vs-WPF gap).
- Confirm the recommended pattern: attribute-based `ObservableValidator` vs. manual `INotifyDataErrorInfo`, and the error-display approach.
- Expand the article description once the approach is confirmed.
- Author the full walkthrough against the confirmed `ObservableValidator` pattern.
- Confirm the sample reflects the recommended approach before linking it as canonical.

### Connect to a database — [connect-to-a-database.md](connect-to-a-database.md)
Data access: EF Core, SQLite, SQL Server, secure credentials.
- Confirm the recommended architecture: direct EF Core + SQL Server in trusted domain scenarios, or always a service layer?
- Confirm EF Core works in packaged and unpackaged WinUI desktop apps; note packaging/sandboxing caveats.
- Confirm the minimum EF Core version tested with the current stable Windows App SDK/.NET.
- Add a validated entity + `DbContext` + connection-string example (packaged vs. unpackaged paths).
- Add an EF Core migration example.
- Add an async load example (`ToListAsync` → `ObservableCollection`), incl. whether `DispatcherQueue.TryEnqueue` is required.
- Add a save example that handles `DbUpdateException`.
- Confirm the right level of detail for an offline-caching pattern (or split it to a dedicated how-to).
- Recommend a sync approach (Azure Data Sync, Sync Framework, or custom).
- Link secrets-management guidance.
- Confirm `PasswordVault` behavior/packaging and the pattern for building a connection string from a stored credential.

### Design for LOB — [design-for-lob.md](design-for-lob.md)
Productivity-focused UI: density, navigation, materials, accessibility.
- Add representative LOB screenshots (form, tabular, dashboard, nav layout) in light + dark.
- Confirm recommended named system brushes for common LOB surfaces (card, list item, secondary text); link a design-token reference.
- Recommend a current information-density approach now that the compact density dictionary is deprecated.
- Clarify recommended use of Mica/Acrylic behind data surfaces (and Mica as a window base under opaque content).
- Confirm the recommended owner/child + true-modal window approach; add a verified example.
- Add a verified `AutomationProperties.Name` + tab-order example; link a full accessibility walkthrough.
- Add an `AdaptiveTrigger` two-column→one-column layout example.
- Add guidance on choosing `NavigationView` vs. `TabView`.

### Add AI capabilities — [ai-for-lob-apps.md](ai-for-lob-apps.md)
On-device (Phi Silica) and cloud AI for LOB scenarios.
- Confirm exact `AIFeatureReadyState` member names and per-state handling; confirm the on-device↔cloud runtime-switching pattern.
- Add token-management, retry, and cost-estimation guidance for LOB volumes (100–10K requests/day).

### WinForms patterns in WinUI — [migrate-winforms-patterns.md](migrate-winforms-patterns.md)
Re-skilling WinForms devs for new WinUI projects + interop (not a rewrite push).
- Confirm the specific current-release advantages to claim (DPI/rendering, Windows App SDK features).
- Confirm the recommended way to host WinUI UI inside existing WinForms/WPF apps (XAML interop); link authoritative guidance.
- Confirm the current XAML designer / Hot Reload status for WinUI projects.
- Complete the control-equivalents table (each row SME-reviewed).
- Confirm a recommended WinUI tray-icon package for `NotifyIcon` (name + maintenance status).
- Confirm the Community Toolkit `GridSplitter` package name/version for the stable channel.
- Add any other common LOB WinForms controls to the table.
- Document the WinUI startup sequence vs. WinForms `Application.Run`.
- Validate the `XamlRoot` requirement for `ContentDialog`.
- Document app-shutdown handling (WinForms `FormClosing`/`ApplicationExit` equivalents).
- Add a before/after WinForms→WinUI MVVM example.
- Confirm whether a WinForms-migration sample folder should exist, and link it once created.

## Articles

- [Build line-of-business apps with WinUI — overview](index.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
- [Build a data-entry form with validation in WinUI 3](build-validated-form.md)
- [Connect a WinUI app to a database](connect-to-a-database.md)
- [Design for productivity in WinUI LOB apps](design-for-lob.md)
- [Add AI capabilities to a line-of-business WinUI app](ai-for-lob-apps.md)
- [WinForms patterns and their WinUI 3 equivalents](migrate-winforms-patterns.md)

## Sample-to-article map

| Sample | Article |
|---|---|
| `WinUI-LOB-Samples/01-TabularData/` | [Display tabular data in a WinUI app](display-tabular-data.md) |
| `WinUI-LOB-Samples/02-ValidatedForm/` | [Build a data-entry form with validation in WinUI 3](build-validated-form.md) |
| `WinUI-LOB-Samples/03-DatabaseAccess/` | [Connect a WinUI app to a database](connect-to-a-database.md) |
| `WinUI-LOB-Samples/04-DesignShowcase/` | [Design for productivity in WinUI LOB apps](design-for-lob.md) |
| `WinUI-LOB-Samples/05-LocalAI/` | [Add AI capabilities to a line-of-business WinUI app](ai-for-lob-apps.md) |
