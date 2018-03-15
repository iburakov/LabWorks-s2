using System;
using System.Collections.Generic;

namespace Contacts {
    public interface IStorage {
        void Add(Contact newContact);
        List<Contact> FindAll(Predicate<Contact> match);
    }
}