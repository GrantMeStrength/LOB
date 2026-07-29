# WinUI LOB docs (review mirror)

The published Learn articles are the source of truth for this sample set:
https://learn.microsoft.com/windows/apps/get-started/line-of-business/

> [!NOTE]
> This folder mirrors the in-review Learn articles (PR #7160) so reviewers can read the current draft directly on GitHub. It is a temporary review copy and will drift from Learn; the Learn articles above remain authoritative. Re-sync or trim this mirror once the PR publishes.

> [!TODO]
> Confirm the final published article slugs once PR #7160 is live, then replace the GitHub links below with the Learn URLs.

## How to review

Each topic below links to its article and lists the **open SME questions** still embedded in that draft as `> [!TODO]` markers. If you can answer any of them, comment on the corresponding line in [PR #7160](https://github.com/MicrosoftDocs/windows-dev-docs-pr/pull/7160) (or here in the mirror). The articles are intentionally honest about what isn't settled yet — every gap is marked rather than filled with a guess.

## Topics and open SME questions

### [Build line-of-business apps with WinUI — overview](index.md)

- Confirm the quick-reference decision list and recommended stack versions; drop anything not broadly applicable to LOB.
- Add an "Authenticate with Entra ID using MSAL in WinUI" how-to link and validate the WAM broker setup steps.
- Link a "WinForms to WinUI" guidance page once authored (the WPF patterns page is the model).
- Add Phi Silica and App Content Search reference links once canonical paths are confirmed.

### [Display tabular data in a WinUI app](display-tabular-data.md)

- Update the article when a first-party WinUI DataGrid ships; until then, keep the ListView/ItemsView guidance.
- Confirm `ItemsView` is the recommended successor to `ListView` for new apps, and whether `ListView` is in maintenance mode.
- Define the sample scenario (e.g., sortable employee directory) once the control recommendation is validated.
- Add an SME-validated model class + `ObservableCollection<T>` exposed from a ViewModel.
- Add complete, SME-validated item `DataTemplate` XAML plus a matching header row using shared column widths.
- Document column sorting (`CollectionViewSource` or custom sort) for `ListView`/`ItemsView`.
- Document row-selection handling (navigate to a detail view / open an edit dialog).

### [Build a data-entry form with validation in WinUI](build-validated-form.md)

- Confirm the validation pattern (attribute-based `ObservableValidator` vs. manual `INotifyDataErrorInfo`) and the error-display approach.
- Expand the article description once the approach is confirmed.
- Author the full walkthrough (annotate properties, trigger validation on input, bind error messages, enable/disable Save via `HasErrors`).
- Confirm the sample reflects the recommended `ObservableValidator` approach before linking it as canonical.

### [Connect a WinUI app to a database](connect-to-a-database.md)

- Confirm the recommended architecture: is direct EF Core + SQL Server acceptable in trusted domain-joined scenarios, or is a service layer always the guidance?
- Confirm EF Core runs in packaged and unpackaged WinUI desktop apps; note packaging/sandboxing considerations for DB file access.
- Confirm the minimum EF Core version tested with the current stable Windows App SDK / .NET, and link EF Core Getting Started.
- Add an SME-validated entity + `DbContext` + connection-string example (packaged vs. unpackaged data paths).
- Add an EF Core add/apply-migration example and link the migrations docs.
- Add an async load example (`ToListAsync` → `ObservableCollection`) with error handling; clarify whether `DispatcherQueue.TryEnqueue` is needed off-thread.
- Add an insert/save example (`SaveChangesAsync`) with `DbUpdateException` handling.
- Add a practical offline caching pattern (local SQLite copy, network detection, queue + sync); SME to set the depth.
- Decide what sync technology to recommend (Azure Data Sync, Microsoft Sync Framework, or custom).
- Add a secrets-management guidance link.
- Confirm `PasswordVault` behavior and packaging requirements, and the pattern for building a connection string from a retrieved credential.

### [Design for productivity in WinUI LOB apps](design-for-lob.md)

- Add screenshots of representative LOB app types (data-entry form, tabular view, dashboard, nav layout) in light and dark themes.
- Confirm the recommended named system brushes for common LOB surfaces (card, list item, secondary text); link a design-token reference if one exists.
- Recommend a current information-density approach now that the compact density dictionary is deprecated.
- Clarify recommended Mica/Acrylic use behind LOB data surfaces, including whether a Mica base layer is appropriate with opaque content on top.
- Confirm the recommended owner/child and true-modal window approach and add a verified example.
- Add a verified `AutomationProperties.Name` + tab-order example and link a full accessibility walkthrough.
- Add an `AdaptiveTrigger` two-column → single-column layout example.
- Add guidance on choosing between `NavigationView` and `TabView` for LOB scenarios.

### [Add AI features to a WinUI LOB app](ai-for-lob-apps.md)

- Confirm the exact `AIFeatureReadyState` member names and per-state handling, and the recommended on-device ↔ cloud runtime switching pattern.
- Add token management, retry-policy, and cost-estimation guidance for LOB volumes (100–10K requests/day).

### [WinForms patterns in WinUI](migrate-winforms-patterns.md)

- Confirm the specific current-release advantages to claim (DPI/rendering behavior, Windows App SDK feature set) without overstating parity.
- Confirm the recommended way to host WinUI UI inside an existing WinForms or WPF app (the XAML interop story) and link authoritative guidance.
- Confirm the summary's accuracy, especially current XAML designer, Hot Reload, and live-preview status for WinUI projects.
- Complete the control-equivalents table; each row needs SME review.
- For `NotifyIcon`: confirm a recommended WinUI tray-icon package (e.g., H.NotifyIcon.WinUI) and its maintenance status.
- For `SplitContainer`: confirm the current Community Toolkit `GridSplitter` package name/version for the stable channel.
- Add rows for any other common LOB WinForms controls not yet covered.
- Document the WinForms `Main`/`Application.Run` vs. WinUI startup sequence; validate against the current project template.
- Validate the `XamlRoot` requirement for `ContentDialog`.
- Document WinUI app-shutdown handling (the equivalent of `FormClosing`/`ApplicationExit`).
- Add a WinForms-vs-WinUI "before/after" (code-behind list load vs. ViewModel + `ObservableCollection<T>` + `x:Bind`).
- Confirm whether a WinForms-migration sample folder should be created in this repo, and link it once it exists.

## Articles

Start with the overview, then read the topic that matches what you're building:

- [Build line-of-business apps with WinUI — overview](index.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
- [Build a data-entry form with validation in WinUI 3](build-validated-form.md)
- [Connect a WinUI app to a database](connect-to-a-database.md)
- [Design for productivity in WinUI LOB apps](design-for-lob.md)
- [Add AI features to a WinUI LOB app](ai-for-lob-apps.md)
- [WinForms patterns in WinUI](migrate-winforms-patterns.md)

## Sample-to-article map

Each article pairs with a runnable sample in [`WinUI-LOB-Samples/`](../WinUI-LOB-Samples):

| Article | Sample |
| --- | --- |
| Display tabular data | `01-TabularData` |
| Build a validated form | `02-ValidatedForm` |
| Connect to a database | `03-DatabaseAccess` |
| Design for productivity | `04-DesignShowcase` |
| Add AI features | `05-LocalAI` |
