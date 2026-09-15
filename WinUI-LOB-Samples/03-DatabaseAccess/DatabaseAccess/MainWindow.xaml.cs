using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DatabaseAccess;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your
/// UI and logic to MainPage.xaml / MainPage.xaml.cs instead of here so you
/// can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        ResizeWindow(960, 720);

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));
    }

    private void ResizeWindow(int widthDip, int heightDip)
    {
        // AppWindow uses physical pixels, while XAML layout uses effective pixels.
        // Scale the design size for the current monitor, then keep the window inside
        // the public DisplayArea work area so taskbars and smaller displays remain usable.
        IntPtr hwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        double scale = GetDpiForWindow(hwnd) / 96.0;
        int width = (int)Math.Ceiling(widthDip * scale);
        int height = (int)Math.Ceiling(heightDip * scale);
        RectInt32? workArea = DisplayArea
            .GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)
            ?.WorkArea;
        if (workArea is RectInt32 area)
        {
            int margin = (int)Math.Ceiling(32 * scale);
            width = Math.Min(width, Math.Max(1, area.Width - margin));
            height = Math.Min(height, Math.Max(1, area.Height - margin));
        }

        AppWindow.Resize(new SizeInt32(width, height));
    }
}
