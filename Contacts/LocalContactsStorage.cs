using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Contacts {

    public class LocalContactsStorage : IContactsStorage {
        private List<Contact> contacts = new List<Contact>();

        public void Add(Contact newContact) => contacts.Add(newContact);

        public List<Contact> FindByEmail(string substring) {
            return contacts.FindAll(contact => contact.Email.Contains(substring));
        }

        public List<Contact> FindByFirstName(string substring) {
            return contacts.FindAll(contact => contact.FirstName.Contains(substring));
        }

        public List<Contact> FindByLastName(string substring) {
            return contacts.FindAll(contact => contact.LastName.Contains(substring));
        }

        public List<Contact> FindByFullName(string substring) {
            return contacts.FindAll(contact => (contact.FirstName + " " + contact.LastName)
                                                .Contains(substring));
        }

        public List<Contact> FindByPhone(string substring) {
            return contacts.FindAll(contact => { return contact.NormalizedPhone.Contains(substring); });
        }

        public List<Contact> GetAllContacts() {
            return new List<Contact>(contacts);
        }

        public void SaveToFile(string filename) {
            File.WriteAllText(filename, String.Join("\n\n", contacts.ConvertAll(contact => contact.ToVCard())));
        }
    }

}