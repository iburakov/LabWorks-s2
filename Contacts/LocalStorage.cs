using System;
using System.Collections.Generic;

namespace Contacts {

    public class LocalStorage : IContactsStorage {
        private List<Contact> contacts = new List<Contact>();

        public void Add(Contact newContact) => contacts.Add(newContact);
        public List<Contact> FindAll(Predicate<Contact> match) => contacts.FindAll(match);
    }

}