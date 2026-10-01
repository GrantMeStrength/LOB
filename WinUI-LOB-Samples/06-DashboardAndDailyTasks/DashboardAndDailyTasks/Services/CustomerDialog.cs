using System.Net.Mail;
using DashboardAndDailyTasks.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DashboardAndDailyTasks.Services;

public sealed partial class CustomerDialog : ContentDialog
{
    private bool _wasSaved;

    private CustomerDialog()
    {
        InitializeComponent();
    }

    public static async Task ShowAsync(XamlRoot root)
    {
        var dialog = new CustomerDialog { XamlRoot = root };
        await dialog.ShowAsync();
        if (dialog._wasSaved)
        {
            var confirmation = new TeachingTip
            {
                Title = "Customer added",
                Subtitle = $"{dialog.NameTextBox.Text.Trim()} is ready to view.",
                IsOpen = true,
                PreferredPlacement = TeachingTipPlacementMode.Bottom
            };
            if (root.Content is Panel host)
            {
                host.Children.Add(confirmation);
                confirmation.Closed += (_, _) => host.Children.Remove(confirmation);
            }
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Hide();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        string? message = Validate(
            NameTextBox.Text,
            CompanyTextBox.Text,
            EmailTextBox.Text,
            PhoneTextBox.Text,
            RegionComboBox.SelectedItem as string);
        if (message is not null)
        {
            ValidationInfoBar.Message = message;
            ValidationInfoBar.IsOpen = true;
            return;
        }

        App.State.Customers.Add(new Customer
        {
            Name = NameTextBox.Text.Trim(),
            Company = CompanyTextBox.Text.Trim(),
            Email = EmailTextBox.Text.Trim(),
            Phone = PhoneTextBox.Text.Trim(),
            Region = (string)RegionComboBox.SelectedItem,
            Status = "Prospect",
            Industry = "Professional services",
            Owner = "Alex Wilber",
            AccountValueAmount = 50000,
            LastActivity = "12 minutes ago",
            Summary = "New customer account ready for qualification, onboarding, and relationship planning."
        });
        App.State.NotifyChanged();
        _wasSaved = true;
        Hide();
    }

    private static string? Validate(string name, string company, string email, string phone, string? region)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Enter the customer's name.";
        if (string.IsNullOrWhiteSpace(company))
            return "Enter the customer's company.";
        if (string.IsNullOrWhiteSpace(email))
            return "Enter a valid email address.";

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            return "Enter a valid email address.";
        }
        catch (ArgumentException)
        {
            return "Enter a valid email address.";
        }

        if (phone.Count(char.IsDigit) < 7)
            return "Enter a valid phone number.";
        if (region is null)
            return "Choose a region.";

        return null;
    }
}
