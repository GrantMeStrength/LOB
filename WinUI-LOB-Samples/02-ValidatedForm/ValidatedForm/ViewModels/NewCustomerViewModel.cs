using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ValidatedForm.ViewModels;

/// <summary>
/// ViewModel for the "New Customer" data-entry form.
/// </summary>
public partial class NewCustomerViewModel : ObservableObject, INotifyDataErrorInfo
{
    private readonly RelayCommand _saveCommand;
    private readonly Dictionary<string, List<string>> _errors = new(StringComparer.Ordinal);

    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string? _region;
    private string _statusMessage = string.Empty;
    private bool _isStatusOpen;

    public NewCustomerViewModel()
    {
        _saveCommand = new RelayCommand(Save, () => IsSaveEnabled);
        ValidateAll();
    }

    /// <summary>The four selectable regions shown in the ComboBox.</summary>
    public IReadOnlyList<string> Regions { get; } = new[] { "North", "South", "East", "West" };

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                ValidateName();
                RefreshSaveState();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                ValidateEmail();
                RefreshSaveState();
            }
        }
    }

    /// <summary>Phone is optional and therefore not validated.</summary>
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string? Region
    {
        get => _region;
        set
        {
            if (SetProperty(ref _region, value))
            {
                ValidateRegion();
                RefreshSaveState();
            }
        }
    }

    /// <summary>Command bound to the Save button; disabled while the form is invalid.</summary>
    public IRelayCommand SaveCommand => _saveCommand;

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

    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return _errors.Values.SelectMany(static e => e).ToArray();
        }

        return _errors.TryGetValue(propertyName, out List<string>? errors)
            ? errors
            : Array.Empty<string>();
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

    // WinUI has no first-party input-validation framework today (a known gap vs WPF). This is a minimal manual INotifyDataErrorInfo example using BCL only.
    private void ValidateAll()
    {
        ValidateName();
        ValidateEmail();
        ValidateRegion();
        RefreshSaveState();
    }

    private void ValidateName()
    {
        SetErrors(nameof(Name), string.IsNullOrWhiteSpace(Name) ? new List<string> { "Name is required." } : []);
        OnPropertyChanged(nameof(NameError));
        OnPropertyChanged(nameof(HasNameError));
    }

    private void ValidateEmail()
    {
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(Email))
        {
            errors.Add("Email is required.");
        }
        else if (!Email.Contains('@') || Email.StartsWith('@') || Email.EndsWith('@'))
        {
            errors.Add("Enter a valid email address.");
        }

        SetErrors(nameof(Email), errors);
        OnPropertyChanged(nameof(EmailError));
        OnPropertyChanged(nameof(HasEmailError));
    }

    private void ValidateRegion()
    {
        SetErrors(nameof(Region), string.IsNullOrWhiteSpace(Region) ? new List<string> { "Region is required." } : []);
        OnPropertyChanged(nameof(RegionError));
        OnPropertyChanged(nameof(HasRegionError));
    }

    private void SetErrors(string propertyName, List<string> errors)
    {
        bool changed;
        if (errors.Count == 0)
        {
            changed = _errors.Remove(propertyName);
        }
        else if (_errors.TryGetValue(propertyName, out List<string>? existing) && existing.SequenceEqual(errors))
        {
            changed = false;
        }
        else
        {
            _errors[propertyName] = errors;
            changed = true;
        }

        if (changed)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    private void RefreshSaveState()
    {
        OnPropertyChanged(nameof(IsSaveEnabled));
        _saveCommand.NotifyCanExecuteChanged();
    }

    private string GetFirstError(string propertyName) =>
        GetErrors(propertyName).OfType<string>().FirstOrDefault() ?? string.Empty;
}
