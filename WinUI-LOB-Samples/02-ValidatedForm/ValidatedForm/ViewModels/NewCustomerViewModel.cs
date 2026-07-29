using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ValidatedForm.ViewModels;

/// <summary>
/// ViewModel for the "New Customer" data-entry form.
/// </summary>
public partial class NewCustomerViewModel : ObservableValidator
{
    private readonly RelayCommand _saveCommand;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required.")]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Region is required.")]
    private string? _region;

    private string _statusMessage = string.Empty;
    private bool _isStatusOpen;

    public NewCustomerViewModel()
    {
        _saveCommand = new RelayCommand(Save, () => IsSaveEnabled);
        ErrorsChanged += OnErrorsChanged;
        ValidateAllProperties();
    }

    /// <summary>The four selectable regions shown in the ComboBox.</summary>
    public IReadOnlyList<string> Regions { get; } = new[] { "North", "South", "East", "West" };

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
        StatusMessage = $"Customer '{Name}' saved.";
        IsStatusOpen = true;

        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Region = null;
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

        OnPropertyChanged(nameof(IsSaveEnabled));
        _saveCommand.NotifyCanExecuteChanged();
    }

    private string GetFirstError(string propertyName) =>
        GetErrors(propertyName)
            .OfType<ValidationResult>()
            .FirstOrDefault()?.ErrorMessage
        ?? string.Empty;
}
