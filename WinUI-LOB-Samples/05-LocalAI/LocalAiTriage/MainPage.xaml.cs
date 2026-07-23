using LocalAiTriage.Models;
using LocalAiTriage.Services;
using LocalAiTriage.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LocalAiTriage;

/// <summary>
/// Hosts the support-ticket triage experience. Selection is pushed from the
/// <see cref="ItemsView"/> into the ViewModel because <c>ItemsView.SelectedItem</c>
/// is read-only and cannot be bound two-way.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        ViewModel = new TicketsViewModel(
            new SampleTicketDataService(),
            new PhiSilicaTextGenerationService());

        Loaded += OnLoaded;
    }

    /// <summary>Gets the ViewModel that backs this page.</summary>
    public TicketsViewModel ViewModel { get; }

    /// <summary>x:Bind helper: whether a ticket is currently selected.</summary>
    public bool IsTicketSelected(SupportTicket? ticket) => ticket is not null;

    /// <summary>x:Bind helper: whether no ticket is selected.</summary>
    public bool IsNoTicketSelected(SupportTicket? ticket) => ticket is null;

    /// <summary>x:Bind helper: maps AI readiness to an <see cref="InfoBarSeverity"/>.</summary>
    public InfoBarSeverity StatusSeverity(bool isReady) =>
        isReady ? InfoBarSeverity.Success : InfoBarSeverity.Warning;

    private async void OnLoaded(object sender, RoutedEventArgs e) =>
        await ViewModel.InitializeCommand.ExecuteAsync(null);

    private void OnTicketSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args) =>
        ViewModel.SelectedTicket = sender.SelectedItem as SupportTicket;
}
