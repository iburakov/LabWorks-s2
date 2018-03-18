using System;
using System.Collections.Generic;

namespace Contacts {
    public interface IContactsStorage {
        void Add(Contact newContact);
        List<Contact> FindAll(Predicate<Contact> match);
    }
}