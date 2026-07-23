using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAiTriage.Models;
using LocalAiTriage.Services;

namespace LocalAiTriage.ViewModels;

/// <summary>
/// Drives the ticket triage page: loads tickets, tracks the local-AI status,
/// and runs on-device summarization and classification for the selected ticket.
/// </summary>
public sealed partial class TicketsViewModel : ObservableObject
{
    private static readonly IReadOnlyList<string> Categories = new[]
    {
        "Billing", "Technical", "Account", "Feature Request", "Other",
    };

    private readonly ITicketDataService _ticketData;
    private readonly ITextGenerationService _textGeneration;

    public TicketsViewModel(ITicketDataService ticketData, ITextGenerationService textGeneration)
    {
        _ticketData = ticketData;
        _textGeneration = textGeneration;
        AiStatusMessage = "Checking local AI availability\u2026";
    }

    public ObservableCollection<SupportTicket> Tickets { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TriageSelectedCommand))]
    public partial SupportTicket? SelectedTicket { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TriageSelectedCommand))]
    public partial bool IsAiReady { get; set; }

    [ObservableProperty]
    public partial string AiStatusMessage { get; set; }

    /// <summary>Loads tickets and prepares the local model when the page opens.</summary>
    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            IReadOnlyList<SupportTicket> tickets = await _ticketData.GetTicketsAsync();
            Tickets.Clear();
            foreach (SupportTicket ticket in tickets)
            {
                Tickets.Add(ticket);
            }
        }
        finally
        {
            IsLoading = false;
        }

        AiStatus status = await _textGeneration.EnsureReadyAsync();
        IsAiReady = status.IsReady;
        AiStatusMessage = status.Message;
    }

    private bool CanTriage() => IsAiReady && SelectedTicket is { IsProcessing: false };

    /// <summary>Summarizes and categorizes the selected ticket entirely on-device.</summary>
    [RelayCommand(CanExecute = nameof(CanTriage))]
    private async Task TriageSelectedAsync()
    {
        SupportTicket? ticket = SelectedTicket;
        if (ticket is null)
        {
            return;
        }

        ticket.IsProcessing = true;
        TriageSelectedCommand.NotifyCanExecuteChanged();
        try
        {
            ticket.Summary = await _textGeneration.GenerateAsync(BuildSummaryPrompt(ticket));
            ticket.Category = await _textGeneration.GenerateAsync(BuildCategoryPrompt(ticket));
        }
        catch (LocalAiUnavailableException ex)
        {
            // The model is present but on-device generation is gated (see the
            // LAF note in the README). Degrade gracefully: surface the reason in
            // the status banner and disable further triage attempts.
            IsAiReady = false;
            AiStatusMessage = ex.Message;
            ticket.Summary = null;
            ticket.Category = null;
        }
        catch (Exception ex)
        {
            ticket.Summary = $"Triage failed: {ex.Message}";
        }
        finally
        {
            ticket.IsProcessing = false;
            TriageSelectedCommand.NotifyCanExecuteChanged();
        }
    }

    private static string BuildSummaryPrompt(SupportTicket ticket) =>
        "You are a customer support assistant. In two short sentences, summarize the "
        + "following support ticket, focusing on the customer's problem and what they need.\n\n"
        + $"Ticket:\n{ticket.Body}";

    private static string BuildCategoryPrompt(SupportTicket ticket) =>
        "Classify the following support ticket into exactly one of these categories: "
        + string.Join(", ", Categories)
        + ". Reply with only the category name and nothing else.\n\n"
        + $"Ticket:\n{ticket.Body}";
}
