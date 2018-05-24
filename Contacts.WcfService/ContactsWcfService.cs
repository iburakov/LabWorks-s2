using System;
using System.Collections.Generic;

namespace Contacts.WcfService {
    public class ContactsWcfService : IContactsWcfService {
        private LocalContactsStorage storage = new LocalContactsStorage();

        public string AddContact(Contact contact) {
            storage.AddContact(contact, out string message);
            return message;
        }

        public IReadOnlyCollection<Contact> FindBy(Contact.FieldKind fieldKind, string query) {
            return storage.FindByField(fieldKind, query);
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            return storage.GetAllContacts();
        }

        public bool greet() {
            return true;
        }
    }
}
