---
title: WinForms patterns and their WinUI 3 equivalents
description: A mapping table showing common Windows Forms patterns and their WinUI 3 equivalents for developers migrating line-of-business apps.
ms.topic: concept
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# WinForms patterns and their WinUI 3 equivalents

This article maps common Windows Forms (WinForms) coding patterns to their WinUI 3 equivalents. Use it as a reference when porting LOB app logic.

> [!NOTE]
> WinUI 3 uses a fundamentally different architecture (XAML + data binding) from WinForms (imperative code-behind). Many WinForms patterns don't have direct equivalents — they require a shift to MVVM.

---

## Control mapping

| WinForms control | WinUI 3 equivalent | Notes |
|-----------------|-------------------|-------|
| `DataGridView` | Community Toolkit `DataGrid` | No first-party WinUI DataGrid |
| `ListView` | `ItemsView` | `ItemsView` replaces `ListView` for new code |
| `TextBox` | `TextBox` | Same name, different namespace |
| `ComboBox` | `ComboBox` | Similar API surface |
| `DateTimePicker` | `CalendarDatePicker` or `DatePicker` | Split into date/time controls |
| `NumericUpDown` | `NumberBox` | |
| `ProgressBar` | `ProgressBar` or `ProgressRing` | Ring for indeterminate |
| `TabControl` | `TabView` | |
| `TreeView` | `TreeView` | Similar but different item model |
| `MenuStrip` | `MenuBar` | |
| `ToolStrip` | `CommandBar` | |
| `StatusStrip` | Custom `Grid` row at bottom | No direct equivalent |
| `OpenFileDialog` | `FileOpenPicker` + HWND init | Must call `InitializeWithWindow` |
| `SaveFileDialog` | `FileSavePicker` + HWND init | Must call `InitializeWithWindow` |
| `MessageBox.Show()` | `ContentDialog` | Requires `XamlRoot` |
| `Form` | `Window` | One window per `Window` instance |
| `MDI (MdiParent/MdiChild)` | `TabView` with embedded `Frame` | No direct MDI equivalent |

---

## Pattern mapping

### Event handlers → Data binding + Commands

**WinForms:**
```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    customer.Name = txtName.Text;
    customer.Email = txtEmail.Text;
    _repository.Save(customer);
}
```

**WinUI 3 (MVVM):**
```csharp
// ViewModel
[RelayCommand]
private async Task SaveAsync()
{
    await _repository.SaveAsync(Customer);
}
```
```xml
<!-- XAML -->
<TextBox Text="{x:Bind ViewModel.Customer.Name, Mode=TwoWay}" />
<Button Content="Save" Command="{x:Bind ViewModel.SaveCommand}" />
```

---

### Form.Load → Page.Loaded or OnNavigatedTo

**WinForms:**
```csharp
private void MainForm_Load(object sender, EventArgs e)
{
    LoadData();
}
```

**WinUI 3:**
```csharp
protected override async void OnNavigatedTo(NavigationEventArgs e)
{
    await ViewModel.LoadDataAsync();
}
```

---

### BackgroundWorker → async/await

**WinForms:**
```csharp
backgroundWorker1.DoWork += (s, e) => { /* long operation */ };
backgroundWorker1.RunWorkerAsync();
```

**WinUI 3:**
```csharp
public async Task LoadDataAsync()
{
    IsLoading = true;
    var data = await _service.GetDataAsync();
    DispatcherQueue.TryEnqueue(() => Items = new ObservableCollection<Item>(data));
    IsLoading = false;
}
```

---

### Form validation → ObservableValidator

**WinForms:**
```csharp
if (string.IsNullOrEmpty(txtName.Text))
{
    errorProvider1.SetError(txtName, "Name is required");
    return;
}
```

**WinUI 3:**
```csharp
[ObservableProperty]
[NotifyDataErrorInfo]
[Required(ErrorMessage = "Name is required")]
private string _name = string.Empty;
```

---

### Application.DoEvents() → No equivalent (not needed)

WinUI 3 is inherently async. If you find yourself wanting `DoEvents()`, refactor the long-running work to `async`/`await`.

> [!IMPORTANT]
> There is no `DoEvents()` equivalent in WinUI 3 and attempting to pump messages manually will cause undefined behavior.

---

### Settings / app.config → IConfiguration or local storage

| WinForms | WinUI 3 |
|----------|---------|
| `Properties.Settings.Default` | `ApplicationData.Current.LocalSettings` or `appsettings.json` + `IConfiguration` |
| `app.config` `<connectionStrings>` | Do not store connection strings in the app — use a service layer |

---

## Architecture shift

| WinForms approach | WinUI 3 approach |
|-------------------|------------------|
| Code-behind per form | MVVM with ViewModels |
| Direct control manipulation | Data binding + `x:Bind` |
| Synchronous data access | Async with `Task`/`await` |
| Global state in `static` classes | Dependency injection |
| `ErrorProvider` | `INotifyDataErrorInfo` |
| MDI forms | `TabView` + navigation |

> [!TODO]
> Add a "migration checklist" section: step-by-step process to port a WinForms app (identify forms → create pages → extract ViewModels → replace data access → test).

---

## Related content

- [Migrate from WinForms to WinUI](https://learn.microsoft.com/windows/apps/get-started/migrate-from-winforms)
- [MVVM pattern with CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [x:Bind markup extension](https://learn.microsoft.com/windows/apps/develop/data-binding/xbind-markup-extension)
