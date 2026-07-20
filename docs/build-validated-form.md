---
title: Build a data-entry form with validation
description: How to create data-entry forms with inline validation in a WinUI 3 app using ObservableValidator from the Community Toolkit.
ms.topic: how-to
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Build a data-entry form with validation

Line-of-business apps commonly require forms that validate user input before saving. WinUI 3 does not have a built-in validation framework like WPF's `Validation.ErrorTemplate`. Instead, use the MVVM Community Toolkit's `ObservableValidator` base class to implement `INotifyDataErrorInfo`.

---

## Approach

1. Derive your ViewModel from `ObservableValidator`.
2. Annotate properties with `System.ComponentModel.DataAnnotations` attributes.
3. Bind controls using `x:Bind` with `Mode=TwoWay`.
4. Display errors in the UI using `TextBlock` elements bound to a helper that reads `GetErrors()`.

---

## Step 1: Define the ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

public partial class CustomerFormViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    private string _email = string.Empty;

    public void Submit()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        // Save logic here
    }
}
```

---

## Step 2: Build the XAML form

```xml
<StackPanel Spacing="12" Padding="24">
    <TextBox Header="Name"
             Text="{x:Bind ViewModel.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    <TextBlock Text="{x:Bind ViewModel.NameErrors}"
               Foreground="Red" FontSize="12" />

    <TextBox Header="Email"
             Text="{x:Bind ViewModel.Email, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    <TextBlock Text="{x:Bind ViewModel.EmailErrors}"
               Foreground="Red" FontSize="12" />

    <Button Content="Submit" Command="{x:Bind ViewModel.SubmitCommand}" />
</StackPanel>
```

> [!TODO]
> Add guidance on creating a reusable `ValidationTextBlock` control that subscribes to `ErrorsChanged` per-property. The current snippet uses a simplified string binding.

---

## Screenshot

:::image type="content" source="images/02-ValidatedForm.png" alt-text="Screenshot of a WinUI form showing inline validation errors beneath input fields.":::

---

## Key differences from WPF

| WPF | WinUI 3 |
|-----|---------|
| `Validation.ErrorTemplate` attached property | No equivalent — use manual error display |
| `BindingGroup` for group validation | Not available — validate in ViewModel |
| `IDataErrorInfo` (legacy) | `INotifyDataErrorInfo` (supported) |

---

## Related content

- [ObservableValidator](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/observablevalidator)
- [Data annotations in .NET](https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations)
- [CommunityToolkit.Mvvm NuGet](https://www.nuget.org/packages/CommunityToolkit.Mvvm)
