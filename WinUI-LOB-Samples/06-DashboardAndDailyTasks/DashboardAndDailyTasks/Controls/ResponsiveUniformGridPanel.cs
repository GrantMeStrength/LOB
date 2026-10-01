using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace DashboardAndDailyTasks.Controls;

public sealed class ResponsiveUniformGridPanel : Panel
{
    public double MinItemWidth { get; set; } = 216;
    public double ColumnSpacing { get; set; } = 12;
    public double RowSpacing { get; set; } = 12;
    public int MaximumColumns { get; set; } = 5;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
            return new Size();

        double availableWidth = double.IsInfinity(availableSize.Width)
            ? Children.Count * MinItemWidth + (Children.Count - 1) * ColumnSpacing
            : availableSize.Width;
        int columns = GetColumnCount(availableWidth);
        double itemWidth = GetItemWidth(availableWidth, columns);
        var rowHeights = new double[(Children.Count + columns - 1) / columns];

        for (int i = 0; i < Children.Count; i++)
        {
            UIElement child = Children[i];
            child.Measure(new Size(itemWidth, double.PositiveInfinity));
            int row = i / columns;
            rowHeights[row] = Math.Max(rowHeights[row], child.DesiredSize.Height);
        }

        double desiredHeight = rowHeights.Sum() + Math.Max(0, rowHeights.Length - 1) * RowSpacing;
        return new Size(availableWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return finalSize;

        int columns = GetColumnCount(finalSize.Width);
        double itemWidth = GetItemWidth(finalSize.Width, columns);
        var rowHeights = new double[(Children.Count + columns - 1) / columns];
        for (int i = 0; i < Children.Count; i++)
        {
            int row = i / columns;
            rowHeights[row] = Math.Max(rowHeights[row], Children[i].DesiredSize.Height);
        }

        double y = 0;
        for (int row = 0; row < rowHeights.Length; row++)
        {
            int rowStart = row * columns;
            int rowCount = Math.Min(columns, Children.Count - rowStart);
            double rowItemWidth = rowCount == columns
                ? itemWidth
                : (finalSize.Width - (rowCount - 1) * ColumnSpacing) / rowCount;

            for (int column = 0; column < rowCount; column++)
            {
                UIElement child = Children[rowStart + column];
                double x = column * (rowItemWidth + ColumnSpacing);
                child.Arrange(new Rect(x, y, rowItemWidth, rowHeights[row]));
            }

            y += rowHeights[row] + RowSpacing;
        }

        return finalSize;
    }

    private int GetColumnCount(double width)
    {
        int columnsThatFit = Math.Max(
            1,
            (int)Math.Floor((width + ColumnSpacing) / (MinItemWidth + ColumnSpacing)));
        return Math.Min(Children.Count, Math.Min(MaximumColumns, columnsThatFit));
    }

    private double GetItemWidth(double width, int columns) =>
        Math.Max(0, (width - (columns - 1) * ColumnSpacing) / columns);
}
