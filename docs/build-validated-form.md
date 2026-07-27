---
title: Build a data-entry form with validation in WinUI 3
description: Input validation is a current gap in WinUI 3 compared with WPF; this article tracks the state of building validated data-entry forms.
ms.topic: tutorial
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Build a data-entry form with validation in WinUI 3

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

Data-entry forms are central to line-of-business apps. Users enter information — customer records, work orders, inventory items — and the app must validate that data before saving it. Input validation is a known gap in WinUI 3, and this article describes the current state rather than a finished pattern.

## Overview

Input validation is one of the areas where WinUI 3 is less complete than WPF. Unlike WPF (which provides the `Validation` class and `ErrorTemplate`) or WinForms (`ErrorProvider`), WinUI 3 has no built-in form-validation system, and there isn't a great first-party solution today.

> [!IMPORTANT]
> There is currently no recommended first-party validation framework for WinUI 3 data-entry forms. Validation has to be implemented manually in your ViewModel — for example, with the `INotifyDataErrorInfo` interface from the base class library — and wired into the UI by hand. This article is a placeholder pending a validated, supported approach.

> [!TODO] SME review: decide whether to publish form-validation guidance for WinUI 3 given the current gap, or to hold this article until a supported first-party solution exists. Do not recommend Community Toolkit or other third-party validation packages as the primary approach.

## What you'll build

:::image type="content" source="images/02-ValidatedForm.png" alt-text="The WinUI 3 validated form sample showing a New Customer form with Name, Email, Phone, and Region fields. The Email field shows an inline validation error message. The Save button is disabled.":::

> [!TODO] Expand this description once SME has reviewed the form validation approach and confirmed the error display mechanism.

## Prerequisites

- Windows App SDK (stable channel) installed
- A WinUI 3 project created from the "Blank App, Packaged (WinUI 3 in Desktop)" template or equivalent

## Steps

> [!TODO] The step-by-step tutorial is not yet written. Because WinUI 3 has no first-party form-validation solution today, the walkthrough is on hold pending SME guidance on a supported approach. Any future steps must not be scaffolded around Community Toolkit `ObservableValidator` or other third-party validation packages.

## Get the sample

A "New Customer" validated-form sample is planned for the LOB samples repo.

> [!NOTE]
> The sample repo is at [github.com/GrantMeStrength/LOB](https://github.com/GrantMeStrength/LOB). This URL may change if the repo is renamed or moved; this article will be updated when that happens.

> [!TODO] Add the sample once a supported WinUI 3 validation approach is confirmed. See the `02-ValidatedForm/` folder.

## Related content

- [Data binding overview](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-overview)
- [Data binding in depth](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-in-depth)
- [Data binding and MVVM](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-and-mvvm)
- [Connect a WinUI app to a database](connect-to-a-database.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
