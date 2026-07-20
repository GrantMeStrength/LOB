---
title: Design for productivity in WinUI LOB apps
description: Design patterns for WinUI 3 line-of-business apps — compact density, navigation, theming, and accessibility.
ms.topic: concept
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Design for productivity in WinUI LOB apps

LOB apps prioritize information density, keyboard navigation, and long-session comfort over consumer app aesthetics. WinUI 3 supports these needs through Fluent Design primitives with density, theme, and accessibility customization.

---

## Compact density

WinUI 3 supports a compact resource set that reduces control padding:

```xml
<Page.Resources>
    <ResourceDictionary Source="ms-appx:///Microsoft.UI.Xaml/DensityStyles/Compact.xaml" />
</Page.Resources>
```

This reduces default row height in lists and spacing between controls — critical for data-heavy views.

> [!TODO]
> Add before/after screenshots comparing standard and compact density for a typical form.

---

## Mica and theming

Use Mica (or Mica Alt) as the app backdrop for a modern, lightweight look that adapts to the user's desktop wallpaper:

```csharp
// In App.xaml.cs or Window code
var backdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
this.SystemBackdrop = backdrop;
```

For branding, override theme resources:

```xml
<Application.Resources>
    <ResourceDictionary>
        <Color x:Key="SystemAccentColor">#0078D4</Color>
    </ResourceDictionary>
</Application.Resources>
```

---

## Navigation patterns

| Pattern | Best for |
|---------|----------|
| `NavigationView` (left pane) | Apps with 5–10 top-level sections |
| `TabView` | Multi-document or multi-record workflows |
| Breadcrumb + `Frame` | Deep hierarchical navigation |

---

## Accessibility

- Ensure all interactive elements have `AutomationProperties.Name`.
- Use `TabIndex` and `AccessKey` for keyboard workflows.
- Test with Narrator and high-contrast themes.
- Respect `UISettings.TextScaleFactor` — don't hard-code font sizes.

---

## Screenshot

:::image type="content" source="images/04-DesignShowcase.png" alt-text="Screenshot of a WinUI LOB app using compact density, Mica backdrop, and NavigationView.":::

---

## Related content

- [Compact sizing](https://learn.microsoft.com/windows/apps/design/style/xaml-styles#compact-sizing)
- [Mica material](https://learn.microsoft.com/windows/apps/design/style/mica)
- [NavigationView](https://learn.microsoft.com/windows/apps/design/controls/navigationview)
- [Accessibility in WinUI](https://learn.microsoft.com/windows/apps/develop/accessibility)
