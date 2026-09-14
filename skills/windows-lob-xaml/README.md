# Windows LOB XAML Skill

A focused Copilot skill for designing, building, and reviewing data-dense
**line-of-business (LOB)** apps in **WinUI 3 / Windows App SDK**.

It concentrates on the surfaces LOB developers actually ship — data grids and
tables, dashboards and metric cards, forms and data entry, task/status
tracking, filtering/sorting/grouping, navigation shells — and on enterprise
compliance: theming, High Contrast, and accessibility. It also ensures WinUI 3
/ Windows App SDK APIs are used instead of WPF or UWP idioms.

> Status: draft, created for the XAML design hackathon.

## What it does

The skill works in four modes:

- **Create** — turn a screenshot, spec, or description into measured,
  design-system-aligned XAML.
- **Guide** — answer questions about data controls, layout, tokens, forms, or binding.
- **Implement** — modify XAML, resources, styles, templates, or converters.
- **Review** — evaluate XAML, a diff, or a PR and return prioritized findings.

The full guidance lives in [`SKILL.md`](./SKILL.md).

## How to use it

### With GitHub Copilot (skill file)

Point Copilot at the skill and describe your task. For example:

- **Review:** _"Using the Windows LOB XAML skill in `skills/windows-lob-xaml/SKILL.md`, review `MainPage.xaml` for theming, High Contrast, status consistency, and Windows Design System compliance."_
- **Create:** _"Using the Windows LOB XAML skill, generate a WinUI 3 dashboard page with metric cards and a filterable customer table."_
- **Guide:** _"Per the Windows LOB XAML skill, what control should I use for an editable table in WinUI 3?"_

### As an installed Copilot skill

If your Copilot client supports skill plugins, install this folder as a skill so
it loads automatically and triggers on LOB keywords (data grid, dashboard, form
validation, task tracking, WinUI 3, etc.). Then invoke it by name or let it
trigger on matching prompts. Restart your Copilot session after installing so the
skill is picked up.

## Real-world examples

Concrete prompts a developer can paste into Copilot with this skill loaded. Each maps to a common LOB task.

### Building UI

- **Dashboard:** _"Using the Windows LOB XAML skill, build a WinUI 3 dashboard page with four KPI cards (Open Tickets, Overdue, Resolved Today, SLA %) in a responsive card grid that reflows on narrow widths. Use the shared status vocabulary for the Overdue card."_
- **Filterable table:** _"Using the skill, create a customers table with an AutoSuggestBox search, removable filter chips for Region and Status, a '42 of 318' result count, and a Clear all button. Read-only ListView with a Grid-based row template."_
- **Validated form:** _"Using the skill, generate a 'New invoice' form with Header labels, a NumberBox for amount, a required Customer ComboBox, blur-based validation, an InfoBar summary on submit, and Save enabled only when dirty and valid."_
- **Task tracker:** _"Using the skill, build a task list grouped by status with glyph+label status chips, multi-select, and a contextual CommandBar (Complete, Assign, Delete) that appears only when items are selected. Confirm Delete in a ContentDialog."_
- **Master-detail:** _"Using the skill, lay out a master-detail records screen that shows a list + detail pane on wide windows and collapses to single-column navigation under 640px."_

### Reviewing existing XAML

- **Design-system review:** _"Using the Windows LOB XAML skill, review MainPage.xaml and report theming, High Contrast, status-consistency, and accessibility issues with severity and line numbers."_
- **PR review:** _"Using the skill, review the XAML changes in this diff and flag any WPF/UWP idioms, color-only status, or missing empty/loading/error states."_
- **Accessibility pass:** _"Using the skill, check this page for missing AutomationProperties.Name on icon-only controls and any status conveyed by color alone."_

### Migrating & fixing

- **UWP → WinUI 3:** _"Using the skill, convert this UWP page to WinUI 3: update Windows.UI.Xaml namespaces to Microsoft.UI.Xaml and replace any Style.Triggers, DynamicResource, and Visibility=Hidden with WinUI 3 equivalents."_
- **WPF DataGrid:** _"Using the skill, this WPF DataGrid needs to move to WinUI 3 — recommend the right control and show an editable-table replacement."_
- **Status consistency fix:** _"Using the skill, extract a shared StatusToBrush/StatusToGlyph converter so status looks identical across the dashboard, table, and task list."_

## When it triggers

LOB app, data grid, GridView/ListView, table, filter/sort/group, dashboard,
metric card, KPI tile, chart, status/badge/chip, task tracking, bulk actions,
form layout, form validation, master-detail, NavigationView, CRUD UI, WinUI 3,
WPF-to-WinUI or UWP-to-WinUI migration, and design-system review.

## Scope

Third-party LOB app developers building WinUI 3 desktop apps. Shell/OS-internal
surfaces (Taskbar, Start, etc.) are intentionally out of scope. For the full
Windows Design System, see the official docs at
[learn.microsoft.com/windows/apps/design](https://learn.microsoft.com/windows/apps/design/).
