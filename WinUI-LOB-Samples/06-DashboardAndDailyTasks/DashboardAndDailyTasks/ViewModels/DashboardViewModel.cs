using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DashboardAndDailyTasks.Models;

namespace DashboardAndDailyTasks.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    public ObservableCollection<SummaryCard> Cards { get; } = [];

    public DashboardViewModel() => Refresh();

    public void Refresh()
    {
        int openTickets = App.State.OpenTickets;
        int remainingTasks = App.State.TasksTotal - App.State.TasksDone;
        Customer[] upcomingRenewals = App.State.Customers
            .Where(customer =>
                customer.RenewalDate >= DateTimeOffset.Now.Date &&
                customer.RenewalDate <= DateTimeOffset.Now.Date.AddDays(30))
            .ToArray();
        int activeCustomers = App.State.Customers.Count(customer => customer.Status == "Active");
        int renewalValue = upcomingRenewals.Sum(customer => customer.AccountValueAmount);
        Cards.Clear();
        Cards.Add(new("Active customers", activeCustomers.ToString(), "Shared customer directory", "\uE716", destination: "Customers", customerFilter: CustomerFilter.Active));
        Cards.Add(new("Open tickets", openTickets.ToString(), "Across the support queue", "\uE8BD", true, "Tickets"));
        Cards.Add(new("Tasks remaining", remainingTasks.ToString(), $"{App.State.TasksDone} completed today", "\uE73E", true, "Tasks"));
        Cards.Add(new(
            "Upcoming renewals",
            upcomingRenewals.Length.ToString(),
            $"${renewalValue / 1000:N0}K renewing in 30 days",
            "\uE787",
            true,
            "Customers",
            CustomerFilter.UpcomingRenewals));
        Cards.Add(new("Average response", "1.8 hours", "32 minutes faster", "\uE823"));
    }
}
