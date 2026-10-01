using System.Collections.ObjectModel;
using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;

namespace DashboardAndDailyTasks.Services;

public sealed class AppState
{
    public ObservableCollection<Customer> Customers { get; } = [];
    public ObservableCollection<SupportTicket> Tickets { get; } = [];
    public ObservableCollection<WorkTask> Tasks { get; } = [];
    public event EventHandler? Changed;

    public ElementTheme Theme { get; set; } = ElementTheme.Default;
    public bool ShowRevenueOverview { get; set; } = true;
    public bool ShowKpis { get; set; } = true;
    public bool ShowTicketPerformance { get; set; } = true;
    public bool ShowTodayTasks { get; set; } = true;
    public bool ShowCustomers { get; set; } = true;
    public bool ShowOpenTickets { get; set; } = true;
    public string? SelectedAiModel { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public string DefaultPage { get; set; } = "Dashboard";
    public int OpenTickets => Tickets.Count(t => !t.IsDone);
    public int TasksDone => Tasks.Count(t => !t.IsDeleted && t.IsComplete);
    public int TasksTotal => Tasks.Count(t => !t.IsDeleted);

    public AppState()
    {
        Reset();
    }

    public void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

    public void Reset()
    {
        Customers.Clear();
        SeedCustomers();

        Tickets.Clear();
        Tickets.Add(CreateTicket(1042, "Dana Whitfield", "Contoso Ledger Pro", "Cannot export month-end report to PDF", "Since updating to the latest build, the month-end report fails to export to PDF at about 80 percent. Excel export still works, but the PDF is needed for an auditor meeting.", "Critical", "In progress", "Maya: Reproduced on build 10.4; collecting export logs."));
        Tickets.Add(CreateTicket(1043, "Marcus Ito", "Contoso Ledger Pro", "Double-charged on my March invoice", "The company card was billed twice for the March subscription. Please refund the duplicate and prevent it from happening next month.", "High", "Pending", "Alex: Waiting for the payment processor transaction IDs."));
        Tickets.Add(CreateTicket(1044, "Priya Nair", "Contoso Field Sync", "Feature request: offline mode for warehouse scanners", "Warehouse scanners are used in areas with no Wi-Fi. The customer needs scans queued offline and synchronized when connectivity returns.", "Medium", "Pending"));
        Tickets.Add(CreateTicket(1045, "Sam O'Connor", "Contoso Field Sync", "Locked out after password reset", "After a password reset, every sign-in attempt reports that the account is temporarily locked. Three technicians cannot begin their routes.", "Critical", "In progress", "Priya: Identity team is checking the lockout policy."));
        Tickets.Add(CreateTicket(1046, "Lena Brandt", "Contoso Ledger Pro", "How do I add a second approver to expense workflows?", "The customer wants expenses over $500 to require two approvers and needs to know whether their current plan supports it.", "Medium", "Pending"));
        Tickets.Add(CreateTicket(1038, "Jo Bell", "Contoso Field Sync", "Scanner enrollment completed", "The replacement scanner is enrolled and syncing correctly.", "Low", "Done", "Sam: Confirmed that all queued scans synchronized."));
        Tickets.Add(CreateTicket(1039, "Nolan Price", "Contoso Ledger Pro", "Invoice template logo restored", "The company logo now appears on generated invoices after the template reset.", "Low", "Done", "Dana: Customer confirmed the corrected PDF output."));

        Tasks.Clear();
        var exportTask = new WorkTask
        {
            Title = "Review PDF export logs",
            Priority = "High",
            Due = DateTimeOffset.Now
        };
        exportTask.RelatedLinks.Add(new("Ticket", "1042", "TKT-1042 · Cannot export month-end report"));
        Tasks.Add(exportTask);

        var renewalTask = new WorkTask
        {
            Title = "Prepare renewal check-in",
            Priority = "Medium",
            Due = DateTimeOffset.Now.AddDays(1)
        };
        renewalTask.RelatedLinks.Add(new("Customer", Customers[3].Id.ToString(), Customers[3].Company));
        renewalTask.RelatedLinks.Add(new("Ticket", "1043", "TKT-1043 · Double-charged invoice"));
        Tasks.Add(renewalTask);

        var chargeTask = new WorkTask
        {
            Title = "Send duplicate charge update",
            Priority = "High",
            Due = DateTimeOffset.Now
        };
        chargeTask.RelatedLinks.Add(new("Ticket", "1043", "TKT-1043 · Double-charged invoice"));
        Tasks.Add(chargeTask);
        Tasks.Add(new() { Title = "Publish weekly queue notes", Priority = "Low", Due = DateTimeOffset.Now, IsComplete = true });
        NotifyChanged();
    }

    private static SupportTicket CreateTicket(
        int id,
        string customer,
        string product,
        string subject,
        string description,
        string priority,
        string status,
        string? comment = null)
    {
        var ticket = new SupportTicket
        {
            Id = id,
            Customer = customer,
            Product = product,
            Subject = subject,
            Description = description,
            Priority = priority,
            Status = status
        };
        if (comment is not null)
        {
            int separator = comment.IndexOf(':');
            ticket.Comments.Add(separator > 0
                ? new TicketComment(comment[..separator].Trim(), comment[(separator + 1)..].Trim(), DateTimeOffset.Now.AddMinutes(-(id % 6 + 1) * 47))
                : new TicketComment("Team member", comment, DateTimeOffset.Now.AddMinutes(-(id % 6 + 1) * 47)));
        }
        return ticket;
    }

    private void SeedCustomers()
    {
        string[] names =
        [
            "Ava Bennett", "Liam Chen", "Sofia Rossi", "Noah Patel", "Emma Nguyen",
            "Lucas Müller", "Mia Johansson", "Ethan Kowalski", "Olivia Reyes", "Mateo Silva",
            "Charlotte Dubois", "Hiro Tanaka", "Amara Okafor", "Daniel Kim", "Isabella Ferrari",
            "Omar Haddad", "Freya Larsen", "Diego Morales", "Priya Sharma", "Sean O'Brien"
        ];
        string[] companies =
        [
            "Contoso Ltd", "Fabrikam Inc", "Adventure Works", "Northwind Traders", "Tailspin Toys",
            "Wingtip Toys", "Litware Inc", "Proseware Inc", "Fourth Coffee", "Graphic Design Co"
        ];
        string[] regions = ["North", "South", "East", "West", "Central"];
        string[] statuses = ["Active", "Prospect", "At risk", "Pending"];
        string[] industries =
        [
            "Manufacturing", "Retail", "Financial services", "Healthcare",
            "Technology", "Professional services", "Logistics", "Energy"
        ];
        string[] owners = ["Maya Chen", "Alex Wilber", "Nora Hassan", "Diego Ruiz", "Priya Shah"];
        string[] activities = ["12 minutes ago", "Yesterday", "2 days ago", "Last week", "3 hours ago"];
        var random = new Random(20250720);

        for (int i = 0; i < names.Length; i++)
        {
            Customers.Add(new Customer
            {
                Name = names[i],
                Company = companies[i % companies.Length],
                Email = $"contact{i + 1}@customer.example",
                Phone = $"(555) 010-{i + 10:00}",
                Region = regions[random.Next(regions.Length)],
                Status = statuses[random.Next(statuses.Length)],
                Industry = industries[i % industries.Length],
                Owner = owners[i % owners.Length],
                AccountValueAmount = (48 + random.Next(320)) * 1000,
                RenewalDate = DateTimeOffset.Now.Date.AddDays(i < 3 ? 8 + i * 7 : 45 + random.Next(120)),
                LastActivity = activities[random.Next(activities.Length)],
                Summary = $"Strategic {industries[i % industries.Length].ToLowerInvariant()} account focused on service growth and a successful upcoming renewal."
            });
        }
    }
}
