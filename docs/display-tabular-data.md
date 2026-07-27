---
title: Display tabular data in a WinUI app
description: Choose the right WinUI 3 control for tabular data with ListView and ItemsView, including trade-offs and when to use each option.
ms.topic: how-to
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Display tabular data in a WinUI app

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

Most line-of-business apps need to display structured, tabular data — rows and columns of records that users can scan, sort, and select. WinUI 3 doesn't ship a first-party DataGrid control today. First-party support is in progress but not yet available, so use the built-in `ListView` and `ItemsView` controls with a columnar `DataTemplate` for now.

## Overview

:::image type="content" source="../screenshots/01-TabularData-cards.png" alt-text="The WinUI 3 tabular data sample showing a customer list in an ItemsView with card-style DataTemplates, displaying Name, Company, Region, and Status fields.":::

WinUI 3 offers the following built-in options for tabular or list-style data display:

| Control | Source | Best for |
|---|---|---|
| `ListView` | WinUI 3 (inbox) | Single-column or simple multi-column lists; full template control |
| `ItemsView` | WinUI 3 (inbox) | Modern successor to `ListView`; flexible layout via `ItemsLayout` |

> [!IMPORTANT]
> WinUI 3 doesn't include a first-party DataGrid control today. First-party DataGrid support is in progress but not yet available, so use `ListView` or `ItemsView` with a columnar `DataTemplate` for tabular data for now.

> [!TODO] SME review: update this article when first-party WinUI DataGrid support ships. Until then, keep the guidance on `ListView`/`ItemsView`.

## When to use each option

### ListView and ItemsView

`ListView` and `ItemsView` are the built-in WinUI 3 controls for displaying collections of items. They are appropriate when:

- Your data has a natural list or card shape (not strict row-column tabular data).
- You need full control over item templates, including heterogeneous row heights or custom layouts.
- Column alignment is not required (or can be achieved with a custom `DataTemplate` using a `Grid`).

`ItemsView` is the recommended control for new code. It supports flexible layouts via the `ItemsLayout` property and is the direction of active WinUI investment.

> [!TODO] SME validation: confirm that `ItemsView` is the recommended successor to `ListView` for new WinUI 3 apps as of the current stable Windows App SDK release. Confirm whether `ListView` is in maintenance mode or still actively developed.

See [List views and grid views](https://learn.microsoft.com/windows/apps/develop/ui/controls/listview-and-gridview) for usage guidance.

## What you'll build

> [!TODO] Describe the sample app scenario (for example: an employee directory with sortable columns, a product catalog with inline editing, or a transaction log). Define this after the control recommendation is validated by SME.

## Steps

### 1. Choose your control

Use the decision table in the [Overview](#overview) section to select the control that fits your data shape and interaction requirements.

### 2. Prepare your data source

Bind the control to an `ObservableCollection<T>` so the UI updates automatically when items are added, removed, or replaced.

> [!TODO] Add a C# code example defining a simple model class and an `ObservableCollection<T>` exposed from a ViewModel. Validate with SME before publishing.

### 3. Define columns

> [!TODO] Add XAML examples for a `ListView` and an `ItemsView` with a multi-column `DataTemplate` using a `Grid`. Validate against the current stable Windows App SDK release.

### 4. Enable sorting

> [!TODO] Describe how to implement column sorting for `ListView` and `ItemsView`, which requires a `CollectionViewSource` or custom sort logic. Validate with SME.

### 5. Handle selection and navigation

> [!TODO] Describe how to respond to row selection (navigate to a detail view, open an edit dialog, etc.). Reference the [list/details pattern](https://learn.microsoft.com/windows/apps/develop/ui/controls/list-details) for the detail pane approach.

## Get the sample

The complete tabular data sample — an `ItemsView`-based customer record list — is available on GitHub.

> [!NOTE]
> The sample repo is at [github.com/GrantMeStrength/LOB](https://github.com/GrantMeStrength/LOB). This URL may change if the repo is renamed or moved; this article will be updated when that happens.

See the `01-TabularData/` folder in the repo.

## Related content

- [List views and grid views](https://learn.microsoft.com/windows/apps/develop/ui/controls/listview-and-gridview)
- [Data binding overview](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-overview)
- [Data binding and MVVM](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-and-mvvm)
- [List/details pattern](https://learn.microsoft.com/windows/apps/develop/ui/controls/list-details)
- [Build a data-entry form with validation](build-validated-form.md)
