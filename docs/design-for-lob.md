---
title: Design for productivity in WinUI LOB apps
description: Design WinUI 3 line-of-business apps for productivity with guidance on theming, materials, accessibility, and responsive layouts.
ms.topic: concept
ms.date: 07/29/2026
author: GrantMeStrength
ms.author: jken
---

# Design for productivity in WinUI LOB apps

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication.

> [!TODO] Add screenshots showing several representative LOB app types (data-entry form, tabular data view, dashboard, navigation pane layout) demonstrating WinUI 3 Fluent Design in a business context. Images should show both light and dark themes.

:::image type="content" source="images/04-DesignShowcase.png" alt-text="The WinUI 3 design showcase sample app showing a NavigationView with Mica backdrop and a dashboard pane with summary cards in light theme.":::

WinUI 3 apps look modern on Windows 11 by default — you don't need custom styling to get a Fluent Design appearance. Built-in controls automatically handle light and dark mode, system accent color, accessibility contrast ratios, and touch/keyboard/mouse input.

LOB apps benefit from information density, efficient keyboard navigation, and comfort during long working sessions. WinUI 3 and Fluent Design let you deliver those qualities while still looking modern and polished — an efficient business tool doesn't have to look dated. This article covers the WinUI 3 design features most relevant to productivity-focused apps.

> [!TIP]
> **Quick reference:** When styling a WinUI 3 LOB app:
> - Use system brushes (for example, `ApplicationPageBackgroundThemeBrush`) rather than hardcoded color values
> - Reserve `AcrylicBackdrop` for transient surfaces such as flyouts and menus
> - Always test in both light and dark themes before shipping

## Overview

| Design area | Default behavior | LOB recommendation |
|---|---|---|
| Theme | Follows Windows light/dark setting | No change needed; test both modes |
| Background material | Solid system color | Mica on title bar/nav pane; solid on content areas |
| Accent color | Follows Windows system accent | No change needed; do not hardcode brand colors |
| Accessibility | WCAG-compliant contrast in default themes | Test with high-contrast mode; avoid custom colors that break contrast |
| Layout | Fixed by default | Use `Grid` + `VisualStateManager` for adaptive window widths |

## Theming and dark mode

WinUI 3 apps automatically follow the user's Windows theme — light or dark. You get this for free when you use system brushes and the default control styles.

The most common design bug in LOB apps is hardcoded colors: a hex value that looks fine in light mode becomes invisible in dark mode. Always use named theme resources (for example, `TextFillColorPrimaryBrush`, `CardBackgroundFillColorDefaultBrush`) rather than hardcoded `#RRGGBB` values.

- [Theming](../../develop/ui/theming.md)

> [!TODO] SME: confirm the recommended set of named system brushes for common LOB UI surfaces (card background, list item background, secondary text). Link to the WinUI 3 design token reference if one exists.

## Information density

WinUI 3 controls use comfortable padding by default — appropriate for consumer and touch-first apps. LOB apps that display dense data (transaction grids, inventory tables, scheduling views) often need to fit more information on screen.

> [!IMPORTANT]
> The compact resource dictionary (`Microsoft.UI.Xaml/DensityStyles/Compact.xaml`) is deprecated and isn't recommended. Some controls may still respond to it, but applying it app-wide can break layout and behavior in ways that are hard to predict.

> [!TODO] SME review: recommend a current approach to information density in LOB apps (for example, targeted spacing adjustments on specific controls) now that the compact density dictionary is deprecated.

## Materials: Mica and Acrylic

Mica and Acrylic are translucent background materials. For LOB apps their value is a modern, trustworthy, low-fatigue look: a subtle sense of depth and a clear separation between chrome (navigation, title bar) and content, without the flat, dated appearance of older business tools. Used sparingly, they reinforce visual hierarchy while keeping data areas fully readable.

**Mica** samples the desktop wallpaper and applies a tinted surface based on the wallpaper color. It is best suited for the app's background window layer — typically behind a navigation pane or title bar area.

**Acrylic** is a more vivid translucent effect. In LOB apps it is appropriate for transient surfaces such as flyouts, tooltips, and context menus.

Because it's translucent, **Acrylic** can reduce the readability of text placed directly on top of it, so reserve it for transient surfaces rather than dense, content-heavy data areas.

> [!TODO] SME review: clarify the recommended use of Mica and Acrylic behind LOB data surfaces (grids, forms, dense lists), including whether Mica as a window base layer is appropriate when opaque content sits on top.

- [System backdrops (Mica and Acrylic)](../../develop/ui/system-backdrops.md)
- [In-app Acrylic](../../develop/ui/in-app-acrylic.md)

## Secondary windows (child and modal)

LOB apps often need more than one window — a main records view plus tool windows, property inspectors, or modal prompts. WinUI 3 supports two distinct patterns:

- **Modal-in-window dialogs.** Use `ContentDialog` for a modal interaction that overlays the current window (confirmations, short forms, prompts). A `ContentDialog` requires a `XamlRoot`, which you set from the hosting element or window before calling `ShowAsync()`. This is the standard WinUI pattern for blocking the current view until the user responds.
- **Additional top-level windows.** Create another `Microsoft.UI.Xaml.Window` for a tool window, secondary document, or floating panel, and show it alongside the main window. Each `Window` has its own content tree and lifetime.

> [!TODO] SME review: WinUI 3's support for a *true modal* child window (a separate `Window` that blocks its owner) is limited. Confirm the current recommended approach for owner/child window relationships and modal behavior across separate windows, and add a verified code example.

## Accessibility

Accessibility is critical for LOB apps: enterprise and government customers frequently require conformance with standards such as Section 508 (U.S.) and EN 301 549 (EU). WinUI 3 inbox controls are built on the UI Automation (UIA) accessibility framework and pass WCAG contrast requirements in the default light and dark themes, so you get accessible controls for free — as long as you don't undermine them.

- **Names for interactive elements.** Provide meaningful `AutomationProperties.Name` values on controls that lack a visible text label (for example, icon-only buttons), so Narrator and other assistive technologies can announce them.
- **Keyboard and tab order.** Ensure every interactive control is reachable by keyboard and that focus moves in a logical order. Use `TabIndex` to correct the order where the visual layout and the default order differ, and `IsTabStop` to skip non-interactive elements.
- **Contrast and theming.** Don't override default control styles with custom colors that reduce contrast. Test in Windows High Contrast mode (Settings → Accessibility → Contrast themes) as well as light and dark themes.
- **Focus visuals.** Keep the default focus visuals so keyboard users can see which element has focus; don't remove focus indicators for aesthetic reasons.
- **Test with Narrator.** Walk the app end to end with Narrator to confirm that names, states, and reading order make sense.

> [!TODO] Add a short, verified example demonstrating `AutomationProperties.Name` on an icon-only button and a corrected tab order, and link a full accessibility walkthrough once the sample supports it.

See [Accessibility overview](../../design/accessibility/accessibility-overview.md) and [Accessibility testing](../../design/accessibility/accessibility-testing.md).

## Responsive layout

LOB apps are used on a wide range of monitor sizes and at varying window widths — from a narrow side panel to a maximized ultrawide display. Design your layout to adapt.

The recommended approach is a `Grid` with proportional (`*`) column and row sizing, combined with `VisualStateManager` adaptive triggers that reorganize the layout at specific window widths.

- [Responsive design](../../design/layout/responsive-design.md)

> [!TODO] Add a brief XAML example showing a two-column LOB layout (navigation + content) that collapses to a single column below a width threshold using `AdaptiveTrigger`. Validate with SME.

## Navigation patterns for LOB apps

Most LOB apps use one of two navigation patterns:

| Pattern | Control | Best for |
|---|---|---|
| Left navigation pane | `NavigationView` | Apps with 5–10 top-level sections; familiar to Windows users |
| Tab bar | `TabView` | Apps where users work across multiple open records simultaneously |

> [!TODO] SME: add guidance on choosing between `NavigationView` and `TabView` for LOB scenarios. Link to the NavigationView and TabView control guidance pages.

## Get the sample

The design showcase sample is in the [LOB samples repo](https://github.com/GrantMeStrength/LOB) under the `04-DesignShowcase/` folder.

> [!NOTE]
> The sample repo URL may change if the repo is renamed or moved; this article will be updated if that happens.

## Related content

- [Build line-of-business apps with WinUI](index.md)
- [Theming](../../develop/ui/theming.md)
- [System backdrops (Mica and Acrylic)](../../develop/ui/system-backdrops.md)
- [Responsive design](../../design/layout/responsive-design.md)
- [Accessibility overview](../../design/accessibility/accessibility-overview.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
- [LOB samples repo](https://github.com/GrantMeStrength/LOB) — see `04-DesignShowcase/` for a running example of Mica and light/dark theming
