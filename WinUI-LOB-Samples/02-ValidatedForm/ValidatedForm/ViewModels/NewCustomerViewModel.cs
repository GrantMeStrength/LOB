using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ValidatedForm.ViewModels;

/// <summary>
/// ViewModel for the "New Customer" data-entry form. Derives from
/// <see cref="ObservableValidator"/> so that DataAnnotations attributes drive
/// per-field validation. Validation runs on every keystroke because the field
/// setters call <c>ValidateProperty</c> and the XAML inputs use
/// <c>UpdateSourceTrigger=PropertyChanged</c>.
/// </summary>
public partial class NewCustomerViewModel : ObservableValidator
{
    private readonly RelayCommand _saveCommand;

    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string? _region;
    private string _statusMessage = string.Empty;
    private bool _isStatusOpen;

    public NewCustomerViewModel()
    {
        _saveCommand = new RelayCommand(Save, () => IsSaveEnabled);

        // Refresh derived error text, Save state, and the command whenever any
        // field's validation errors change.
        ErrorsChanged += OnErrorsChanged;

        // Validate immediately so required-field errors are visible on an empty
        // form and the Save button starts disabled.
        ValidateAllProperties();
    }

    /// <summary>The four selectable regions shown in the ComboBox.</summary>
    public IReadOnlyList<string> Regions { get; } = new[] { "North", "South", "East", "West" };

    [Required(ErrorMessage = "Name is required.")]
    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            ValidateProperty(value, nameof(Name));
            RefreshSaveState();
        }
    }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ValidateProperty(value, nameof(Email));
            RefreshSaveState();
        }
    }

    /// <summary>Phone is optional and therefore not validated.</summary>
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    [Required(ErrorMessage = "Region is required.")]
    public string? Region
    {
        get => _region;
        set
        {
            SetProperty(ref _region, value);
            ValidateProperty(value, nameof(Region));
            RefreshSaveState();
        }
    }

    /// <summary>Command bound to the Save button; disabled while the form is invalid.</summary>
    public IRelayCommand SaveCommand => _saveCommand;

    /// <summary>
    /// True only when there are no validation errors and every required field
    /// has a value. Drives both the Save button state and the command's
    /// CanExecute.
    /// </summary>
    public bool IsSaveEnabled =>
        !HasErrors
        && !string.IsNullOrWhiteSpace(Name)
        && !string.IsNullOrWhiteSpace(Email)
        && !string.IsNullOrWhiteSpace(Region);

    public string NameError => GetFirstError(nameof(Name));

    public string EmailError => GetFirstError(nameof(Email));

    public string RegionError => GetFirstError(nameof(Region));

    public bool HasNameError => !string.IsNullOrEmpty(NameError);

    public bool HasEmailError => !string.IsNullOrEmpty(EmailError);

    public bool HasRegionError => !string.IsNullOrEmpty(RegionError);

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool IsStatusOpen
    {
        get => _isStatusOpen;
        set => SetProperty(ref _isStatusOpen, value);
    }

    private void Save()
    {
        // In a real LOB app this would persist the customer. Here we surface a
        // success message and reset the form.
        StatusMessage = $"Customer \u2018{Name}\u2019 saved.";
        IsStatusOpen = true;

        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Region = null;
    }

    private void RefreshSaveState()
    {
        OnPropertyChanged(nameof(IsSaveEnabled));
        _saveCommand.NotifyCanExecuteChanged();
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Name):
                OnPropertyChanged(nameof(NameError));
                OnPropertyChanged(nameof(HasNameError));
                break;
            case nameof(Email):
                OnPropertyChanged(nameof(EmailError));
                OnPropertyChanged(nameof(HasEmailError));
                break;
            case nameof(Region):
                OnPropertyChanged(nameof(RegionError));
                OnPropertyChanged(nameof(HasRegionError));
                break;
        }

        RefreshSaveState();
    }

    private string GetFirstError(string propertyName) =>
        GetErrors(propertyName)
            .OfType<ValidationResult>()
            .FirstOrDefault()?.ErrorMessage
        ?? string.Empty;
}
