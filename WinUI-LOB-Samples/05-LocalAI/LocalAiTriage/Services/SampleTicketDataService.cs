using System.Collections.Generic;
using System.Threading.Tasks;
using LocalAiTriage.Models;

namespace LocalAiTriage.Services;

/// <summary>
/// In-memory sample data source. Stands in for a database or line-of-business
/// web service; the small delay simulates real asynchronous I/O.
/// </summary>
public sealed class SampleTicketDataService : ITicketDataService
{
    public async Task<IReadOnlyList<SupportTicket>> GetTicketsAsync()
    {
        // Simulate an async data-access call (database / REST) off the UI thread.
        await Task.Delay(400).ConfigureAwait(false);

        return new List<SupportTicket>
        {
            new(
                1042,
                "Dana Whitfield",
                "Contoso Ledger Pro",
                "Cannot export month-end report to PDF",
                "Since updating to the latest build on Monday, the month-end financial "
                + "report fails to export to PDF. The progress bar reaches about 80 percent "
                + "and then the app shows 'Export failed (code 9)'. Exporting to Excel still "
                + "works. I need the PDF for an auditor meeting on Thursday, so this is urgent."),
            new(
                1043,
                "Marcus Ito",
                "Contoso Ledger Pro",
                "Double-charged on my March invoice",
                "My company card was billed twice for the March subscription — I see two "
                + "identical charges of $89 on the same day. Please refund the duplicate and "
                + "let me know how to stop it happening again next month."),
            new(
                1044,
                "Priya Nair",
                "Contoso Field Sync",
                "Feature request: offline mode for warehouse scanners",
                "Our warehouse team uses the handheld scanners in areas with no Wi-Fi. It "
                + "would be great if the app could queue scans offline and sync them once the "
                + "device is back on the network. Right now anything scanned offline is lost."),
            new(
                1045,
                "Sam O'Connor",
                "Contoso Field Sync",
                "Locked out after password reset",
                "I reset my password this morning and now every sign-in attempt says "
                + "'account temporarily locked'. I've waited an hour and it still won't let me "
                + "in. I have three technicians who can't start their routes until this is fixed."),
            new(
                1046,
                "Lena Brandt",
                "Contoso Ledger Pro",
                "How do I add a second approver to expense workflows?",
                "We're growing and I'd like expenses over $500 to require two approvers "
                + "instead of one. I looked in Settings but only found a single-approver option. "
                + "Is this possible on our current plan, or do we need to upgrade?"),
        };
    }
}
