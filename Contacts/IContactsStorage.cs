using System.Collections.Generic;

namespace Contacts {
    public interface IContactsStorage {
        void AddContact(Contact newContact, out string message);
        IReadOnlyCollection<Contact> GetAllContacts();
        IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query);
    }
}