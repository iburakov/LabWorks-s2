using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace Contacts {

    public class LocalContactsStorage : IContactsStorage {
        private ConcurrentBag<Contact> contacts = new ConcurrentBag<Contact>();
        private List<Contact> ContactsList { get => new List<Contact>(contacts); }

        public void AddContact(Contact newContact, out string message) {
            contacts.Add(newContact);
            message = $"Successfully added {newContact.FullName} to contacts!";
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            return new ReadOnlyCollection<Contact>(ContactsList);
        }

        public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            List<Contact> result;
            switch (fieldKind) {
                case Contact.FieldKind.FullName:
                    result = ContactsList.FindAll(contact => 
                        $"{contact.FirstName} {contact.LastName}".Contains(query)
                        ||
                        $"{contact.LastName} {contact.FirstName}".Contains(query)
                    );
                break;
                case Contact.FieldKind.Phone: result = ContactsList.FindAll(contact => contact.NormalizedPhone.Contains(Contact.NormalizePhone(query))); break;
                case Contact.FieldKind.Birthday: result = ContactsList.FindAll(contact => contact.Birthday == query); break;
                default:
                    result = ContactsList.FindAll(contact =>
                        typeof(Contact).GetProperty(fieldKind.ToString())
                        .GetValue(contact).ToString()
                        .Contains(query)
                    );
                break;
            }
            return new ReadOnlyCollection<Contact>(result);
        }
    }

}