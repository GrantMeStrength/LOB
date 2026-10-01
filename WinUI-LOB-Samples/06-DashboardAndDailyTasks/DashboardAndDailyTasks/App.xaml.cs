using Microsoft.UI.Xaml;

namespace DashboardAndDailyTasks;

public partial class App : Application
{
    private Window? _window;
    public static Services.AppState State { get; } = new();

    public App()
    {
        InitializeComponent();
        UnhandledException += (_, args) =>
            File.WriteAllText(
                Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "last-crash.txt"),
                args.Exception.ToString());
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
