using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace DashboardAndDailyTasks.Controls;

public sealed class MoveCursorGrid : ContentControl
{
    public MoveCursorGrid()
    {
        IsTabStop = true;
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeAll);
    }
}
