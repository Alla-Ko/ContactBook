using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactBook.Models;

public partial class Contact : INotifyPropertyChanged
{
    private int _contactId;
    private string? _firstName;
    private string? _lastName;
    private string? _email;
    private string? _adresse;
    private int? _phoneCodeId;
    private int? _phoneNumber;
    private PhoneCode _phoneCode;

    // Поля для властивостей
    private bool _canEdit;
    private bool _canDelete;



    // Властивість CanEdit
    public bool CanEdit
    {
        get => _canEdit;
        set
        {
            if (_canEdit != value)
            {
                _canEdit = value;
                OnPropertyChanged(nameof(CanEdit));
            }
        }
    }

    // Властивість CanDelete
    public bool CanDelete
    {
        get => _canDelete;
        set
        {
            if (_canDelete != value)
            {
                _canDelete = value;
                OnPropertyChanged(nameof(CanDelete));
            }
        }
    }

    public int ContactId
    {
        get => _contactId;
        set
        {
            if (_contactId != value)
            {
                _contactId = value;
                OnPropertyChanged();
            }
        }
    }

    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName != value)
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }
    }

    public string? LastName
    {
        get => _lastName;
        set
        {
            if (_lastName != value)
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Adresse
    {
        get => _adresse;
        set
        {
            if (_adresse != value)
            {
                _adresse = value;
                OnPropertyChanged();
            }
        }
    }

    public int? PhoneCodeId
    {
        get => _phoneCodeId;
        set
        {
            if (_phoneCodeId != value)
            {
                _phoneCodeId = value;
                OnPropertyChanged();
            }
        }
    }

    public int? PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (_phoneNumber != value)
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }
    }

    public PhoneCode PhoneCode
    {
        get => _phoneCode;
        set
        {
            if (_phoneCode != value)
            {
                _phoneCode = value;
                OnPropertyChanged();
            }
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public string FullPhone
    {
        get
        {
            var code = PhoneCodeId.HasValue ? GetPhoneCode(PhoneCodeId.Value) : string.Empty;
            var number = PhoneNumber.HasValue ? PhoneNumber.Value.ToString() : string.Empty;
            return $"{code} {number}";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private string GetPhoneCode(int codeId)
    {
        using var context = new ContactsContext();
        var phoneCode = context.PhoneCodes
                .Where(pc => pc.CodeId == codeId)
                .Select(pc => pc.Code)
                .FirstOrDefault();

        return phoneCode.HasValue ? phoneCode.Value.ToString() : string.Empty;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
