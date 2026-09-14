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

You don't need to name the WinUI 3 primitives — state the goal and the skill applies the right patterns for you (responsive reflow, list virtualization, the shared status vocabulary, validation timing, adaptive layout).

- **Dashboard:** _"Using the Windows LOB XAML skill, build a support dashboard showing Open Tickets, Overdue, Resolved Today, and SLA %."_ → skill lays out a card grid that reflows on narrow windows and styles the Overdue tile with the shared status vocabulary.
- **Filterable table:** _"Using the skill, I need a customers screen where users can search and filter a few thousand records and see how many match."_ → skill adds search, removable filter chips, a result count, Clear all, and keeps the list virtualized.
- **Validated form:** _"Using the skill, create a 'New invoice' form for customer, amount, and due date — Save should only work when the entry is valid."_ → skill picks the right inputs, validates on blur/submit (not per-keystroke), and gates Save on a dirty-and-valid ViewModel property.
- **Task tracker:** _"Using the skill, build a task list where users can see each task's status at a glance, select several, and complete or reassign them together."_ → skill adds glyph+label status chips, multi-select, and a contextual bulk command bar with confirm-on-delete.
- **Master-detail:** _"Using the skill, make a records screen with a list and a detail view that still works when the window is narrow."_ → skill builds an adaptive two-pane layout that collapses to single-column navigation.

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
