using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DesignShowcase.Pages;
using DesignShowcase.Services;
using Microsoft.UI.Windowing;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DesignShowcase;

public sealed partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        AppWindow.SetIcon("Assets/AppIcon.ico");
        ResizeWindow(1120, 760);

        // The root grid hosts every page in the nav frame, so runtime theme
        // changes applied to it cascade across the whole app.
        AppearanceService.Initialize(RootGrid);

        NavFrame.Navigate(typeof(DashboardPage));
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

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItem is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "dashboard":
                    NavFrame.Navigate(typeof(DashboardPage));
                    break;
                case "customers":
                    NavFrame.Navigate(typeof(CustomersPage));
                    break;
                default:
                    throw new InvalidOperationException($"Unknown navigation item tag: {item.Tag}");
            }
        }
    }

    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        ElementTheme theme = AppearanceService.ToggleTheme();

        // Segoe Fluent Icons: sun glyph when currently dark (tap to go light),
        // brightness glyph when currently light (tap to go dark).
        ThemeToggleIcon.Glyph = theme == ElementTheme.Dark ? "\uE706" : "\uE793";
    }
}
