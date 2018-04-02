using System;
using System.Collections.Generic;

namespace Contacts {
    public interface IContactsStorage {
        void Add(Contact newContact);

        List<Contact> GetAllContacts();
        List<Contact> FindByFirstName(string substring);
        List<Contact> FindByLastName(string substring);
        List<Contact> FindByFullName(string substring);
        List<Contact> FindByEmail(string substring);
        List<Contact> FindByPhone(string substring);
    }
}