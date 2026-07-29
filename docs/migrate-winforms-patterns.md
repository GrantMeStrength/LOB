---
title: WinForms patterns and their WinUI 3 equivalents
description: A pattern mapping guide for developers migrating Windows Forms apps to WinUI 3, with control, data binding, and lifecycle equivalents.
ms.topic: concept-article
ms.date: 07/29/2026
author: GrantMeStrength
ms.author: jken
---

# WinForms patterns and their WinUI 3 equivalents

> [!NOTE]
> This article is a **first-draft stub** for SME review. Sections marked `> [!TODO]` require technical validation before publication. This article is a companion to [WPF patterns and their WinUI 3 equivalents](../../windows-app-sdk/migrate-to-windows-app-sdk/wpf-patterns-winui3.md).

Windows Forms (WinForms) developers moving to WinUI 3 will find some familiar concepts — event-driven programming and .NET — alongside important differences in UI composition, data binding, and application lifecycle.

> [!NOTE]
> You don't need to rewrite a working WinForms or WPF app. Existing apps remain fully supported, and continuing to maintain them is a valid choice. This guidance is for two situations: **re-skilling** WinForms developers who are building *new* projects in WinUI, and **adding WinUI to an existing app** through interop where a specific new capability adds value.

For LOB apps, WinUI brings capabilities that are harder to reach from WinForms:

- **Fluent Design and modern theming** — built-in light/dark themes, system accent color, and Mica/Acrylic materials.
- **`WebView2`** — an inbox control that hosts the Microsoft Edge (Chromium) engine for rich web content and interop.
- **Modern DPI and rendering** — WinUI 3 renders on the modern Windows composition stack.
- **Windows App SDK APIs** — access to current Windows platform features through a single SDK.
- **On-device AI** — integrate models such as Phi Silica on Copilot+ PCs. See [Add AI capabilities to a line-of-business WinUI app](ai-for-lob-apps.md).

> [!TODO] SME validation: confirm the specific, current-release advantages to claim here (for example, exact DPI/rendering behavior and Windows App SDK feature set) so this list stays accurate and doesn't overstate parity or gaps.

This article maps common WinForms patterns to their WinUI 3 equivalents so you can apply familiar concepts when building new WinUI screens. Use it alongside the [Migration decision guide](../../windows-app-sdk/migrate-to-windows-app-sdk/migration-decision-guide.md) to understand the scope of any migration.

> [!TODO] SME: confirm the current recommended approach for hosting WinUI 3 UI inside an existing WinForms or WPF app (the XAML interop story) and link the authoritative guidance. Do not assert a specific API here until validated.

## Overview

WinForms uses a code-first, designer-driven model where controls are defined in `.designer.cs` files and positioned using absolute or docked layout. WinUI 3 uses XAML markup for UI definition, with a layout system based on panels (`StackPanel`, `Grid`, `RelativePanel`, etc.) that adapt to available space.

The most significant differences for WinForms developers are:

- **No drag-and-drop XAML designer.** WinUI 3 does not have an interactive visual designer comparable to the WinForms designer. UI is authored in XAML markup directly, often with a live preview.
- **Data binding replaces code-behind data wiring.** WinUI 3 uses compiled bindings (`x:Bind`) or classic bindings (`Binding`) rather than manually assigning values in event handlers.
- **MVVM is the dominant pattern.** Business logic in WinUI 3 apps is typically separated into ViewModels, following the Model-View-ViewModel (MVVM) pattern.
- **Application lifecycle differs.** WinUI 3 desktop apps use `App.xaml.cs` and `Window` rather than `Application.Run` and `Form`.

> [!TODO] SME validation: confirm accuracy of the above summary. In particular, confirm the current state of XAML designer support in Visual Studio for WinUI 3 projects (Hot Reload and live preview status as of the current stable release).

## Control equivalents for new WinUI screens and bridging

Use this table when building new WinUI screens or bridging an existing app: it maps familiar WinForms controls and patterns to their WinUI 3 equivalents.

> [!TODO] Complete the table below. Each row should be reviewed by an SME familiar with both WinForms and WinUI 3. Add rows for additional patterns as identified during SME review.

| WinForms pattern | WinUI 3 equivalent | Notes |
|---|---|---|
| `Form` | `Window` | WinUI 3 apps can have multiple `Window` instances. `Window` does not inherit from a control base class. |
| `Panel` (absolute layout) | `Canvas` | `Canvas` supports absolute positioning via `Canvas.Left` / `Canvas.Top` attached properties. Prefer adaptive layouts (`Grid`, `StackPanel`) for LOB apps. |
| `FlowLayoutPanel` | `StackPanel` or `WrapPanel` | `StackPanel` arranges items horizontally or vertically. `WrapPanel` wraps items to the next line. |
| `TableLayoutPanel` | `Grid` | `Grid` uses row and column definitions; `*` sizing is proportional. |
| `DataGridView` | No first-party equivalent yet | First-party WinUI DataGrid support is in progress and not yet available. Use `ListView` or `ItemsView` for now. See [Display tabular data](display-tabular-data.md). |
| `ListBox` | `ListView` or `ItemsView` | `ItemsView` is the recommended modern equivalent for new code. |
| `ComboBox` | `ComboBox` | Available in WinUI 3 with similar API shape. |
| `TextBox` | `TextBox` | Available in WinUI 3. `PlaceholderText` replaces WinForms placeholder patterns. |
| `Label` | `TextBlock` | `TextBlock` is read-only text. Use `TextBox` with `IsReadOnly="True"` for selectable read-only text. |
| `Button` | `Button` | Available in WinUI 3. `Command` property supports MVVM command binding. |
| `CheckBox` | `CheckBox` | Available in WinUI 3. Supports three-state via `IsThreeState`. |
| `RadioButton` | `RadioButton` | Available in WinUI 3. Group with `GroupName`. |
| `MenuStrip` | `MenuBar` | `MenuBar` is a top-level menu bar; `MenuFlyout` is for context menus. |
| `StatusStrip` | No direct equivalent | TODO: SME to recommend approach (custom footer layout, or InfoBar for status messages). |
| `TabControl` | `TabView` | `TabView` supports closeable tabs and is the recommended equivalent. |
| `ToolTip` | `ToolTip` | Available in WinUI 3 via the `ToolTipService`. |
| `BindingSource` | `ObservableCollection<T>` + `x:Bind` | WinUI 3 uses compiled bindings (`x:Bind`) against ViewModel properties. No `BindingSource` intermediary. |
| `ErrorProvider` | `INotifyDataErrorInfo` | Implement on the ViewModel or model class. See [Build a validated form](build-validated-form.md). |
| `BackgroundWorker` | `Task` + `DispatcherQueue` | Use `async`/`await` for background work; marshal back to the UI thread via `DispatcherQueue.TryEnqueue`. |
| `Application.Run` | `App.xaml.cs` + `Application.Start` | WinUI 3 app entry point is in `App.xaml.cs`. The `App` class derives from `Application`. |
| `Form.Load` | `Window.Activated` or ViewModel constructor | Load data in the ViewModel constructor or respond to the `Activated` event on the `Window`. |
| `MessageBox.Show` | `ContentDialog` | `ContentDialog` requires a `XamlRoot`. See the HWND and XamlRoot initialization guidance. |
| `OpenFileDialog` / `SaveFileDialog` | `FileOpenPicker` / `FileSavePicker` | Require HWND initialization via `WinRT.Interop.InitializeWithWindow.Initialize` in WinUI 3 desktop apps. |
| `NotifyIcon` | No first-party equivalent | WinUI 3 and the Windows App SDK do not include a system tray icon control. Use the Win32 `Shell_NotifyIcon` API via P/Invoke, or a community NuGet package that wraps it. |
| `ProgressBar` | `ProgressBar` or `ProgressRing` | `ProgressBar` shows determinate (percentage) or indeterminate progress as a horizontal bar. `ProgressRing` shows indeterminate progress as a spinning ring — use for blocking operations where percentage is unknown. Both are built-in WinUI 3 controls. |
| `DateTimePicker` | `DatePicker` + `TimePicker` | WinUI 3 separates date and time selection into two distinct controls. For a combined date-and-time entry, place a `DatePicker` and a `TimePicker` side-by-side in a `StackPanel`. There is no single combined `DateTimePicker` control in WinUI 3. |
| `NumericUpDown` | `NumberBox` | `NumberBox` is the WinUI 3 equivalent: it accepts numeric input, supports spin buttons (increment/decrement), input validation, and display formatting. Available in WinUI 3 as an inbox control. |
| `SplitContainer` | `Grid` + community `GridSplitter` | WinUI 3 does not include a draggable splitter control. The recommended approach is a `Grid` with proportional columns or rows, combined with a `GridSplitter` from the Windows Community Toolkit for WinUI 3. |
| `WebBrowser` | `WebView2` | `WebView2` hosts the Microsoft Edge (Chromium) rendering engine inside a WinUI 3 app. It replaces the legacy IE-based `WebBrowser` control and supports modern web standards, JavaScript interop, and navigation events. Available as an inbox WinUI 3 control. |

> [!TODO] SME: for the `NotifyIcon` row, confirm whether a community WinUI 3 tray icon package is recommended for LOB use (for example, H.NotifyIcon.WinUI or similar); validate package name and maintenance status before linking.

> [!TODO] SME: for the `SplitContainer` row, confirm the current NuGet package name and version for the Community Toolkit `GridSplitter` compatible with WinUI 3 / Windows App SDK stable channel.

> [!TODO] Add rows for any additional WinForms controls commonly used in LOB apps that are not yet covered above. Each new row requires SME validation.

## Application model differences

### Entry point and startup

> [!TODO] Describe the differences between `static void Main` / `Application.Run(new MainForm())` in WinForms and the WinUI 3 startup sequence (`Program.cs` → `App.xaml.cs` → `MainWindow`). Validate the current generated project structure with the stable Windows App SDK template.

### Window management

In WinForms, you typically have one `Form` as the main window and may open additional `Form` instances as dialogs or secondary windows. In WinUI 3, each window is a `Window` instance, and dialogs are `ContentDialog` controls attached to the `XamlRoot` of an existing window.

- [Multiple windows](../../develop/ui/multiple-windows.md) — manage secondary windows in a WinUI 3 app
- [Dialogs and flyouts](../../develop/ui/controls/dialogs-and-flyouts/dialogs.md) — use `ContentDialog` for modal dialogs

> [!TODO] Validate the `XamlRoot` requirement for `ContentDialog` with SME.

### Closing and cleanup

> [!TODO] Describe how to handle application shutdown in WinUI 3 (the equivalent of `Form.FormClosing`, `Application.ApplicationExit`, etc.). Reference the app lifecycle documentation.

## Data binding differences

WinForms typically wires data to the UI in code-behind — setting `TextBox.Text = record.Name` in event handlers, or using a `BindingSource` component. WinUI 3 uses declarative bindings in XAML.

| Scenario | WinForms | WinUI 3 |
|---|---|---|
| Display a property value | `textBox1.Text = item.Name;` | `<TextBlock Text="{x:Bind ViewModel.Name}" />` |
| Two-way editing | `BindingSource` + `DataBindings.Add` | `<TextBox Text="{x:Bind ViewModel.Name, Mode=TwoWay}" />` |
| Collection display | `DataGridView.DataSource = list;` | `<ItemsView ItemsSource="{x:Bind ViewModel.Items}" />` |
| Change notification | `INotifyPropertyChanged` or `BindingSource.ResetBindings` | `INotifyPropertyChanged` on the ViewModel |

See [Data binding overview](../../develop/data-binding/data-binding-overview.md) and [Data binding in depth](../../develop/data-binding/data-binding-in-depth.md).

## MVVM in WinUI 3

WinForms apps are often written with all logic in the form's code-behind. WinUI 3 apps work best when you separate UI from business logic using the MVVM pattern.

> [!TODO] Add a brief "before and after" comparison showing a WinForms code-behind pattern (loading a list of records and displaying them) alongside the equivalent WinUI 3 MVVM pattern (ViewModel with `ObservableCollection<T>` + `x:Bind`). Validate code examples with SME before publishing.

See [Data binding and MVVM](../../develop/data-binding/data-binding-and-mvvm.md).

## Get the sample

> [!TODO] There is no dedicated WinForms-migration sample folder in the [LOB samples repo](https://github.com/GrantMeStrength/LOB) yet. Confirm with the sample owner whether a migration sample folder should be created, and link it here once it exists.

## Related content

- [WPF patterns and their WinUI 3 equivalents](../../windows-app-sdk/migrate-to-windows-app-sdk/wpf-patterns-winui3.md)
- [Migration decision guide](../../windows-app-sdk/migrate-to-windows-app-sdk/migration-decision-guide.md)
- [Migration strategy overview](../../windows-app-sdk/migrate-to-windows-app-sdk/overall-migration-strategy.md)
- [Display tabular data in a WinUI app](display-tabular-data.md)
- [Data binding overview](../../develop/data-binding/data-binding-overview.md)
- [Build a data-entry form with validation](build-validated-form.md)
