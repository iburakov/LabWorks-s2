using System;
using System.Collections.Generic;

namespace Contacts {
    public abstract class RemoteContactsStorage : IContactsStorage {
        public bool IsGreetingSuccessful { get; protected set; }

        public void AddContact(Contact newContact, out string message) {

        }

        public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            throw new NotImplementedException();
        }
    }
}
