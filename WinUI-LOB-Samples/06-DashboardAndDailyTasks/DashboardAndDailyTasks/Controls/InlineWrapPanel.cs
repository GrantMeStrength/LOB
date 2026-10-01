using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace DashboardAndDailyTasks.Controls;

public sealed class InlineWrapPanel : Panel
{
    public double HorizontalSpacing { get; set; } = 4;
    public double VerticalSpacing { get; set; } = 4;

    protected override Size MeasureOverride(Size availableSize)
    {
        double availableWidth = double.IsInfinity(availableSize.Width)
            ? double.MaxValue
            : availableSize.Width;
        double rowWidth = 0;
        double rowHeight = 0;
        double desiredWidth = 0;
        double desiredHeight = 0;

        foreach (UIElement child in Children)
        {
            child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size childSize = child.DesiredSize;
            double nextWidth = rowWidth == 0
                ? childSize.Width
                : rowWidth + HorizontalSpacing + childSize.Width;

            if (rowWidth > 0 && nextWidth > availableWidth)
            {
                desiredWidth = Math.Max(desiredWidth, rowWidth);
                desiredHeight += rowHeight + VerticalSpacing;
                rowWidth = childSize.Width;
                rowHeight = childSize.Height;
            }
            else
            {
                rowWidth = nextWidth;
                rowHeight = Math.Max(rowHeight, childSize.Height);
            }
        }

        desiredWidth = Math.Max(desiredWidth, rowWidth);
        desiredHeight += rowHeight;
        return new Size(desiredWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double x = 0;
        double y = 0;
        double rowHeight = 0;

        foreach (UIElement child in Children)
        {
            Size childSize = child.DesiredSize;
            if (x > 0 && x + HorizontalSpacing + childSize.Width > finalSize.Width)
            {
                x = 0;
                y += rowHeight + VerticalSpacing;
                rowHeight = 0;
            }

            if (x > 0)
                x += HorizontalSpacing;
            child.Arrange(new Rect(x, y, childSize.Width, childSize.Height));
            x += childSize.Width;
            rowHeight = Math.Max(rowHeight, childSize.Height);
        }

        return finalSize;
    }
}
