---
title: Build a data-entry form with validation in WinUI 3
description: Build a data-entry form in WinUI 3 with input validation using the ObservableValidator base class from the CommunityToolkit.Mvvm package.
ms.topic: tutorial
ms.date: 07/29/2026
author: GrantMeStrength
ms.author: jken
---

# Build a data-entry form with validation in WinUI 3

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

Data-entry forms are central to line-of-business apps. Users enter information — customer records, work orders, inventory items — and the app must validate that data before saving it. In WinUI 3, the recommended approach is the `ObservableValidator` base class from the MVVM Toolkit (`CommunityToolkit.Mvvm`), which implements `INotifyDataErrorInfo` and drives validation from data-annotation attributes on your ViewModel.

## Overview

WinUI 3 has no built-in form-validation control like the WPF `Validation` class or the WinForms `ErrorProvider`. Instead, validation is implemented in the ViewModel and surfaced through data binding. The `ObservableValidator` base class in the MVVM Toolkit provides this: you annotate ViewModel properties with `System.ComponentModel.DataAnnotations` attributes (such as `[Required]` or `[EmailAddress]`), call `ValidateAllProperties()` or `ValidateProperty()`, and read validation errors through `GetErrors()` and `HasErrors`.

`CommunityToolkit.Mvvm` (the MVVM Toolkit) is a live, Microsoft-maintained NuGet package. It is distinct from the unmaintained Community Toolkit `DataGrid` control and is the current recommended MVVM library for WinUI apps.

> [!TODO] SME review: confirm the recommended validation pattern (attribute-based `ObservableValidator` vs. a manual `INotifyDataErrorInfo` implementation) and the error-display approach before this article is published.

## What you'll build

:::image type="content" source="images/02-ValidatedForm.png" alt-text="The WinUI 3 validated form sample showing a New Customer form with Name, Email, Phone, and Region fields. The Email field shows an inline validation error message. The Save button is disabled.":::

> [!TODO] Expand this description once SME has reviewed the form validation approach and confirmed the error display mechanism.

## Prerequisites

- Windows App SDK (stable channel) installed
- A WinUI 3 project created from the "Blank App, Packaged (WinUI 3 in Desktop)" template or equivalent
- The `CommunityToolkit.Mvvm` NuGet package (namespace `CommunityToolkit.Mvvm.ComponentModel`)

## Add the MVVM Toolkit package

Add the `CommunityToolkit.Mvvm` package to your project:

```console
dotnet add package CommunityToolkit.Mvvm
```

Your validating ViewModel then derives from `ObservableValidator` (in the `CommunityToolkit.Mvvm.ComponentModel` namespace).

## Steps

> [!TODO] The full step-by-step walkthrough (annotating properties, triggering validation on input, binding error messages to the UI, and enabling or disabling the Save button based on `HasErrors`) is not yet written. Author it against the `ObservableValidator` pattern above; do not scaffold around the unmaintained Community Toolkit `DataGrid` control.

## Get the sample

The validated form sample is in the [LOB samples repo](https://github.com/GrantMeStrength/LOB) under the `02-ValidatedForm/` folder.

> [!NOTE]
> The sample repo URL may change if the repo is renamed or moved; this article will be updated if that happens.

> [!TODO] SME review: confirm the sample reflects the recommended `ObservableValidator` validation approach before this article links it as canonical guidance.

## Related content

- [Data binding overview](../../develop/data-binding/data-binding-overview.md)
- [Data binding in depth](../../develop/data-binding/data-binding-in-depth.md)
- [Data binding and MVVM](../../develop/data-binding/data-binding-and-mvvm.md)
- [Connect a WinUI app to a database](connect-to-a-database.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
