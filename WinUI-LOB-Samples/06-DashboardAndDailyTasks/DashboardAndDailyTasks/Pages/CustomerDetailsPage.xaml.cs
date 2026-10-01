using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DashboardAndDailyTasks.Pages;

public sealed partial class CustomerDetailsPage : Page
{
    public CustomerDetailsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not Guid customerId ||
            App.State.Customers.FirstOrDefault(customer => customer.Id == customerId) is not { } customer)
        {
            MainWindow.Instance.Navigate("Customers");
            return;
        }

        CompanyLogo.Text = customer.Company.Length > 0
            ? customer.Company[..1].ToUpperInvariant()
            : "?";
        CompanyText.Text = customer.Company;
        IndustryText.Text = customer.Industry;
        RegionText.Text = customer.Region;
        StatusText.Text = customer.Status;
        ValueText.Text = customer.AccountValue;
        RenewalText.Text = customer.RenewalDateDisplay;
        ActivityText.Text = customer.LastActivity;
        SummaryText.Text = customer.Summary;
        ContactText.Text = customer.Name;
        ContactDetailsText.Text = $"{customer.Email}\n{customer.Phone}";
        OwnerText.Text = customer.Owner;
    }
}
