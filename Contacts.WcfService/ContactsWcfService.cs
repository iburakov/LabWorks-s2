using System;
using System.Collections.Generic;

namespace Contacts.WcfService {
    public class ContactsWcfService : IContactsWcfService {
        private LocalContactsStorage storage = new LocalContactsStorage();

        public string AddContact(ContactData incomingContactData) {
            storage.AddContact(incomingContactData.ToContact(), out string message);
            return message;
        }

        public IReadOnlyCollection<ContactData> FindBy(Contact.FieldKind fieldKind, string query) {
            return ContactData.NewFromContactCollection(storage.FindByField(fieldKind, query));
        }

        public IReadOnlyCollection<ContactData> GetAllContacts() {
            return ContactData.NewFromContactCollection(storage.GetAllContacts());
        }
    }
}
