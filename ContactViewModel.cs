using ContactBook.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactBook
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        private readonly ContactsContext _context;
        private Contact _selectedContact;
        private Contact _previousSelectedContact;
        private Contact _contactToEdit;
        private Contact _originalContact;
        private Visibility _infoGridVisibility = Visibility.Collapsed;
        private Visibility _saveButtonVisibility = Visibility.Visible;
        private bool _addButtonIsEnabled = true;
        private bool _isEditing;
        private bool _isModified;
        private bool _isUnique;
        private bool _isFieldsValid;
        private string _searchQuery;


        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<PhoneCode> _phoneCodes;

        public ObservableCollection<Contact> Contacts { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand LoadContactsCommand { get; }
        public ICommand AddNewContactCommand { get; }
        public ICommand EditContactCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand DeleteContactCommand { get; }
        public ICommand CancelCommand { get; }

        public ContactViewModel()
        {
            _context = new ContactsContext();
            Contacts = new ObservableCollection<Contact>();
            PhoneCodes = new ObservableCollection<PhoneCode>();
            ContactToEdit = new Contact();
            SelectedContact = new Contact();
            LoadContactsCommand = new RelayCommand(async () => await LoadContacts());
            AddNewContactCommand = new RelayCommand(AddNewContact, CanAddOrEdit);
            EditContactCommand = new RelayCommand(EditContact);
            SaveChangesCommand = new RelayCommand(SaveChanges, CanSaveChanges);
            DeleteContactCommand = new RelayCommand(Delete, CanDelete);
            SearchCommand = new RelayCommand(async () => await ExecuteSearch());
            CancelCommand = new RelayCommand(Cancel);
            ContactToEdit.PropertyChanged += (s, e) => CheckIfModified();
            ContactToEdit.PropertyChanged += (s, e) => CheckIfUnique();
            LoadContacts();
            LoadPhoneCodes();
        }
        public ObservableCollection<PhoneCode> PhoneCodes
        {
            get => _phoneCodes;
            set
            {
                _phoneCodes = value;
                OnPropertyChanged(nameof(PhoneCodes));
            }
        }
        private void LoadPhoneCodes()
        {

            PhoneCodes.Clear();

            var phoneCodes = _context.PhoneCodes.ToList();

            PhoneCodes = new ObservableCollection<PhoneCode>(phoneCodes);

        }


        private void Cancel()
        {

            ContactToEdit = new Contact();
            OriginalContact = new Contact();
            IsEditing = false;

            ValidateFields();
            UpdateSaveButtonVisibility();
            UpdateAddButtonIsEnabled();
            CheckIfModified();
            CheckIfUnique();
        }


        private async Task ExecuteSearch()
        {
            await LoadContacts(SearchQuery);
            ContactToEdit = new Contact();
            IsEditing = false;
            CheckIfModified();
            UpdateAddButtonIsEnabled();
            UpdateSaveButtonVisibility();
        }
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));

            }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact != value)
                {
                    if (_previousSelectedContact != null)
                    {
                        _previousSelectedContact.CanEdit = false;
                        _previousSelectedContact.CanDelete = false;
                    }
                    _selectedContact = value;
                    ContactToEdit = new Contact();
                    OriginalContact = new Contact();
                    IsEditing = false;
                    OnPropertyChanged(nameof(SelectedContact));
                    UpdateInfoGridVisibility();
                    if (_selectedContact != null)
                    {
                        _selectedContact.CanEdit = true;
                        _selectedContact.CanDelete = true;
                    }
                    _previousSelectedContact = _selectedContact;
                }

            }
        }
        public Visibility InfoGridVisibility
        {
            get => _infoGridVisibility;
            set
            {
                if (_infoGridVisibility != value)
                {
                    _infoGridVisibility = value;
                    OnPropertyChanged(nameof(InfoGridVisibility));
                }
            }
        }
        public Visibility SaveButtonVisibility
        {
            get => _saveButtonVisibility;
            private set
            {
                if (_saveButtonVisibility != value)
                {
                    _saveButtonVisibility = value;
                    OnPropertyChanged(nameof(SaveButtonVisibility));
                }
            }
        }
        public bool AddButtonIsEnabled
        {
            get => _addButtonIsEnabled;
            private set
            {
                if (_addButtonIsEnabled != value)
                {
                    _addButtonIsEnabled = value;
                    OnPropertyChanged(nameof(AddButtonIsEnabled));
                }
            }
        }


        private void UpdateInfoGridVisibility()
        {
            if (SelectedContact.ContactId > 0)
            {
                InfoGridVisibility = Visibility.Visible;
            }
            else
            {
                InfoGridVisibility = Visibility.Collapsed;
            }
        }
        private void UpdateSaveButtonVisibility()
        {
            SaveButtonVisibility = (IsEditing && IsModified && IsFieldsValid) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void UpdateAddButtonIsEnabled()
        {
            AddButtonIsEnabled = (!IsEditing && IsUnique && IsFieldsValid);
        }


        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
                UpdateSaveButtonVisibility();
                UpdateAddButtonIsEnabled();

            }
        }
        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
                UpdateSaveButtonVisibility();

            }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set
            {
                _isUnique = value;
                OnPropertyChanged(nameof(IsUnique));

                UpdateAddButtonIsEnabled();
            }
        }

        private void CheckIfUnique()
        {

            if (_contactToEdit == null)
            {
                IsUnique = false;

            }
            else
            {
                var existingContact = _context.Contacts
                             .FirstOrDefault(c => (c.PhoneNumber == ContactToEdit.PhoneNumber || c.Email == ContactToEdit.Email) &&
                             c.ContactId != ContactToEdit.ContactId);

                IsUnique = existingContact == null;
            }
            OnPropertyChanged(nameof(IsUnique));

            UpdateAddButtonIsEnabled();
        }
        private void CheckIfModified()
        {

            if (_contactToEdit == null || _originalContact == null)
            {
                IsModified = false;

            }
            else
            {

                IsModified = !(_contactToEdit.FirstName == _originalContact.FirstName &&
                               _contactToEdit.LastName == _originalContact.LastName &&
                               _contactToEdit.Email == _originalContact.Email &&
                               _contactToEdit.Adresse == _originalContact.Adresse &&
                               _contactToEdit.PhoneCodeId == _originalContact.PhoneCodeId &&
                               _contactToEdit.PhoneNumber == _originalContact.PhoneNumber);

            }
            OnPropertyChanged(nameof(IsModified));
            UpdateSaveButtonVisibility();
            UpdateAddButtonIsEnabled();
        }


        public bool IsFieldsValid
        {
            get { return _isFieldsValid; }
            set
            {
                _isFieldsValid = value;
                OnPropertyChanged(nameof(IsFieldsValid));
                UpdateSaveButtonVisibility();
                UpdateAddButtonIsEnabled();
            }
        }
        private void ValidateFields()
        {
            IsFieldsValid = !string.IsNullOrWhiteSpace(ContactToEdit.FirstName) &&
                            !string.IsNullOrWhiteSpace(ContactToEdit.LastName) &&
                            ContactToEdit.PhoneNumber > 10000 &&
                            IsValidEmail(ContactToEdit.Email);
            UpdateSaveButtonVisibility();
            UpdateAddButtonIsEnabled();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public Contact ContactToEdit
        {
            get => _contactToEdit;
            set
            {
                if (_contactToEdit != null)
                {
                    _contactToEdit.PropertyChanged -= ContactToEdit_PropertyChanged;
                }
                _contactToEdit = value;


                if (_contactToEdit != null)
                {
                    ContactToEdit.PhoneCodeId = _contactToEdit.PhoneCodeId;
                    _contactToEdit.PropertyChanged += ContactToEdit_PropertyChanged;
                }



                OnPropertyChanged(nameof(ContactToEdit));
                CheckIfModified();
                CheckIfUnique();
                ValidateFields();
                UpdateSaveButtonVisibility();
                UpdateAddButtonIsEnabled();

            }
        }
        private void ContactToEdit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIfModified();
            CheckIfUnique();
            ValidateFields();
        }
        public Contact OriginalContact
        {
            get { return _originalContact; }
            set
            {
                _originalContact = value;
                OnPropertyChanged(nameof(OriginalContact));
                CheckIfModified();
                CheckIfUnique();
            }
        }


        private async Task LoadContacts(string filter = null)
        {
            Contacts.Clear();

            var query = _context.Contacts.Include(c => c.PhoneCode).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Trim().ToLower();
                query = query.Where(c =>
                    (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(filter) ||
                    (c.LastName.ToLower() + " " + c.FirstName.ToLower()).Contains(filter) ||
                    c.Email.ToLower().Contains(filter) ||
                    c.Adresse.ToLower().Contains(filter) ||
                    (c.PhoneCodeId.ToString() + c.PhoneNumber.ToString()).Contains(filter)
                );
            }

            var contacts = await query.ToListAsync();
            foreach (var contact in contacts)
            {
                contact.CanDelete = false;
                contact.CanEdit = false;
                Contacts.Add(contact);
            }
        }

        private void AddNewContact()
        {
            if (ContactToEdit == null) return;
            if (IsEditing) return;
            var existingContact = _context.Contacts
                .FirstOrDefault(c => c.PhoneNumber == ContactToEdit.PhoneNumber || c.Email == ContactToEdit.Email);

            _context.Contacts.Add(ContactToEdit);
            _context.SaveChanges();
            LoadContactsCommand.Execute(null);
            ContactToEdit = new Contact();
        }

        private void EditContact()
        {

            IsEditing = true;
            UpdateSaveButtonVisibility();
            if (SelectedContact != null)
            {
                ContactToEdit = new Contact
                {
                    ContactId = SelectedContact.ContactId,
                    FirstName = SelectedContact.FirstName,
                    LastName = SelectedContact.LastName,
                    Email = SelectedContact.Email,
                    Adresse = SelectedContact.Adresse,
                    PhoneCodeId = SelectedContact.PhoneCodeId,
                    PhoneNumber = SelectedContact.PhoneNumber,
                    PhoneCode = SelectedContact.PhoneCode
                };
                OriginalContact = new Contact
                {
                    ContactId = SelectedContact.ContactId,
                    FirstName = SelectedContact.FirstName,
                    LastName = SelectedContact.LastName,
                    Email = SelectedContact.Email,
                    Adresse = SelectedContact.Adresse,
                    PhoneCodeId = SelectedContact.PhoneCodeId,
                    PhoneNumber = SelectedContact.PhoneNumber,
                    PhoneCode = SelectedContact.PhoneCode
                };
                ValidateFields();


            }
            else
            {
                ContactToEdit = new Contact();
                OriginalContact = new Contact();
            }
            UpdateSaveButtonVisibility();
        }
        private bool CanAddOrEdit() => ContactToEdit != null;
        private void SaveChanges()
        {
            if (ContactToEdit == null) return;

            var contact = _context.Contacts
                .FirstOrDefault(c => c.ContactId == ContactToEdit.ContactId);

            if (contact != null)
            {
                contact.FirstName = ContactToEdit.FirstName;
                contact.LastName = ContactToEdit.LastName;
                contact.Email = ContactToEdit.Email;
                contact.Adresse = ContactToEdit.Adresse;
                contact.PhoneCodeId = ContactToEdit.PhoneCodeId;
                contact.PhoneNumber = ContactToEdit.PhoneNumber;

                _context.SaveChanges();
                LoadContactsCommand.Execute(null);
                IsEditing = false;
                UpdateSaveButtonVisibility();
            }
        }
        private bool CanSaveChanges()
        {
            if (ContactToEdit == null) return false;
            var original = _context.Contacts.AsNoTracking().FirstOrDefault(c => c.ContactId == ContactToEdit.ContactId);
            if (original == null) return false;

            return original.FirstName != ContactToEdit.FirstName ||
                   original.LastName != ContactToEdit.LastName ||
                   original.Email != ContactToEdit.Email ||
                   original.Adresse != ContactToEdit.Adresse ||
                   original.PhoneCodeId != ContactToEdit.PhoneCodeId ||
                   original.PhoneNumber != ContactToEdit.PhoneNumber;
        }


        private void Delete()
        {
            if (SelectedContact == null) return;
            if (true)
            {
                _context.Contacts.Remove(SelectedContact);
                _context.SaveChanges();
                LoadContactsCommand.Execute(null);
            }
        }
        private bool CanDelete() => SelectedContact != null;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
