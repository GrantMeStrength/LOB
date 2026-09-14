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
