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

You don't need to name the WinUI 3 primitives — state the goal and the skill applies the right patterns for you (responsive reflow, list virtualization, the shared status vocabulary, type-specific validation, visual hierarchy, discoverable affordances, theme-from-system, adaptive layout).

- **Dashboard:** _"Using the Windows LOB XAML skill, build a support dashboard showing Open Tickets, Overdue, Resolved Today, and SLA %."_ → skill lays out a card grid that reflows on narrow windows, styles the Overdue tile with the shared status vocabulary, and gives the most important metric visual prominence so the page leads the eye from summary toward detail (not a wall of identical cards).
- **Filterable table:** _"Using the skill, I need a customers screen where users can search and filter a few thousand records and see how many match."_ → skill adds search, removable filter chips, a result count, Clear all, keeps the list virtualized, and makes sortable columns look sortable — a clear sort affordance, visually distinct from filters and command buttons.
- **Validated form:** _"Using the skill, create a 'New invoice' form for customer, amount, and due date — Save should only work when the entry is valid."_ → skill picks inputs that match each field's data and validates by type (e.g. email, phone) on blur/submit (not per-keystroke), keeps optional fields optional while still validating any value entered, gates Save on a dirty-and-valid ViewModel, and reflows/scrolls as the window narrows instead of splitting a simple form into tabs.
- **Task tracker:** _"Using the skill, build a task list where users can see each task's status at a glance, reorder tasks, sort them the way they actually work, and select several to complete or reassign together."_ → skill adds glyph+label status chips, multi-select, a contextual bulk command bar with confirm-on-delete, a visible grab handle so drag-to-reorder is discoverable, and sorting that separates done from not-done (not just by due date).
- **Master-detail:** _"Using the skill, make a records screen with a list and a detail view that still works when the window is narrow."_ → skill builds an adaptive two-pane layout with sensible minimum widths that collapses to single-column navigation, so it doesn't break at the width the app first opens.
- **Theme:** _"Using the skill, let users switch between light and dark, defaulting to their Windows theme."_ → skill initializes from the current system theme and places the override in Settings (not a prominent top-level toggle), verifying the UI in both Light and Dark.
- **Multi-area app:** _"Using the skill, combine customer management and task tracking into one app rather than two separate pages."_ → skill brings them into a single NavigationView shell with global navigation across the related areas, instead of bolting navigation onto a single-purpose page.

### Reviewing existing XAML

- **Design-system review:** _"Using the Windows LOB XAML skill, review MainPage.xaml and report theming, High Contrast, status-consistency, discoverability of affordances (reorder handles, sortable headers), and accessibility issues with severity and line numbers."_
- **PR review:** _"Using the skill, review the XAML changes in this diff and flag any WPF/UWP idioms, color-only status, or missing empty/loading/error states."_
- **Accessibility pass:** _"Using the skill, check this page for missing AutomationProperties.Name on icon-only controls, any status conveyed by color alone, and poor color contrast in every interactive state (selection, hover, filtered) — not just the default."_

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
