using System.Collections.Generic;
using System.Threading.Tasks;
using LocalAiTriage.Models;

namespace LocalAiTriage.Services;

/// <summary>
/// Provides the support tickets shown in the app. Loading is asynchronous so a
/// real implementation can query a database or web service without blocking the
/// UI thread.
/// </summary>
public interface ITicketDataService
{
    /// <summary>Loads the current set of support tickets off the UI thread.</summary>
    Task<IReadOnlyList<SupportTicket>> GetTicketsAsync();
}
