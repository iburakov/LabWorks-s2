using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Contacts {

    public class LocalContactsStorage : IContactsStorage {
        private List<Contact> contacts = new List<Contact>();

        public void AddContact(Contact newContact, out string message) {
            contacts.Add(newContact);
            message = $"Successfully added {newContact.FullName} to contacts!";
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            return new ReadOnlyCollection<Contact>(contacts);
        }

        public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            List<Contact> result;
            switch (fieldKind) {
                case Contact.FieldKind.FullName:
                    result = contacts.FindAll(contact => 
                        $"{contact.FirstName} {contact.LastName}".Contains(query)
                        ||
                        $"{contact.LastName} {contact.FirstName}".Contains(query)
                    );
                break;
                case Contact.FieldKind.Phone: result = contacts.FindAll(contact => contact.NormalizedPhone.Contains(Contact.NormalizePhone(query))); break;
                case Contact.FieldKind.Birthday: result = contacts.FindAll(contact => contact.Birthday == query); break;
                default:
                    result = contacts.FindAll(contact =>
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