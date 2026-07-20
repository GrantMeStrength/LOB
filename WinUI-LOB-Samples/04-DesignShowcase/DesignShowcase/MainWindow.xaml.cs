using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DesignShowcase.Pages;
using DesignShowcase.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DesignShowcase;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        AppWindow.SetIcon("Assets/AppIcon.ico");

        // The root grid hosts every page in the nav frame, so runtime theme and
        // density changes applied to it cascade across the whole app.
        AppearanceService.Initialize(RootGrid);

        NavFrame.Navigate(typeof(DashboardPage));
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
