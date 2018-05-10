using System.Collections.ObjectModel;

namespace Contacts {
    public interface IContactsStorage {
        void AddContact(Contact newContact, out string message);
        ReadOnlyCollection<Contact> GetAllContacts();
        ReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query);
    }
}