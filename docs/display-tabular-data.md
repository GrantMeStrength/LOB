---
title: Display tabular data in a WinUI app
description: How to present collections of records using ItemsView and the Community Toolkit DataGrid control in a WinUI 3 line-of-business app.
ms.topic: how-to
ms.date: 07/20/2026
author: GrantMeStrength
ms.author: jken
---

# Display tabular data in a WinUI app

Most line-of-business apps need to present rows of structured data — customer lists, inventory records, order histories. WinUI 3 provides two main paths:

| Approach | When to use |
|----------|-------------|
| **`ItemsView`** with a custom `DataTemplate` | Card/tile layouts, grouped lists, moderate row counts |
| **Community Toolkit `DataGrid`** | Dense tabular display with sorting, filtering, column resizing |

> [!NOTE]
> WinUI 3 does not include a first-party `DataGrid` control. Use the Community Toolkit package `CommunityToolkit.WinUI.UI.Controls.DataGrid`.

---

## Option 1: ItemsView with a DataTemplate

`ItemsView` is the recommended collection control in WinUI 3 (replacing `ListView`/`GridView` for new code). It supports virtualization, item templates, and layout customization.

```xml
<ItemsView ItemsSource="{x:Bind ViewModel.Customers}">
    <ItemsView.ItemTemplate>
        <DataTemplate x:DataType="models:Customer">
            <Grid Padding="12" ColumnDefinitions="*,*,Auto">
                <TextBlock Text="{x:Bind Name}" />
                <TextBlock Grid.Column="1" Text="{x:Bind Email}" />
                <TextBlock Grid.Column="2" Text="{x:Bind Status}" />
            </Grid>
        </DataTemplate>
    </ItemsView.ItemTemplate>
</ItemsView>
```

> [!TODO]
> Add guidance on `ItemsView` layout strategies (StackLayout vs LinedFlowLayout) with LOB-focused examples.

---

## Option 2: Community Toolkit DataGrid

For dense, Excel-like views with sorting and column resizing, use the Community Toolkit DataGrid:

1. Install the NuGet package:
   ```
   dotnet add package CommunityToolkit.WinUI.UI.Controls.DataGrid --version 7.1.2
   ```

2. Add the namespace:
   ```xml
   xmlns:controls="using:CommunityToolkit.WinUI.UI.Controls"
   ```

3. Bind to your collection:
   ```xml
   <controls:DataGrid
       ItemsSource="{x:Bind ViewModel.Orders}"
       AutoGenerateColumns="True"
       IsReadOnly="True" />
   ```

> [!IMPORTANT]
> The Community Toolkit DataGrid is a community-maintained control. It does not receive the same servicing cadence as first-party WinUI controls. Test thoroughly with your data volumes.

---

## Performance considerations

- Use `ObservableCollection<T>` for live updates; for static snapshots, `List<T>` is sufficient.
- Virtualization is automatic in `ItemsView`. For DataGrid, ensure `MaxHeight` is constrained.
- For very large datasets (10K+ rows), consider server-side pagination.

> [!TODO]
> Add benchmark guidance: ItemsView vs DataGrid render time for 1K / 10K / 100K rows.

---

## Screenshot: Cards layout with ItemsView

:::image type="content" source="images/01-TabularData-cards.png" alt-text="Screenshot of a WinUI app showing customer records in a card layout using ItemsView.":::

## Screenshot: DataGrid with dense rows

:::image type="content" source="images/01-TabularData-datagrid.png" alt-text="Screenshot of a WinUI app showing an order list in a Community Toolkit DataGrid.":::

---

## Related content

- [ItemsView class](/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.itemsview)
- [Community Toolkit DataGrid](https://learn.microsoft.com/dotnet/communitytoolkit/windows/datagrid)
- [Data binding overview](https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-overview)
