using System;
using System.Collections.Generic;

namespace Contacts.WcfService {
    public class ContactData {
        public string firstName;
        public string lastName;
        public string nickname;
        public string phone;
        public string email;
        public string mailer;
        public string note;
        public DateTime birthday;

        public Contact ToContact() {
            return new Contact {
                FirstName = firstName,
                LastName = lastName,
                Nickname = nickname,
                Phone = phone,
                Email = email,
                Mailer = mailer,
                Note = note,
                Birthday = birthday.ToShortDateString()
            };
        }

        public static ContactData NewFromContact(Contact contact) {
            return new ContactData() {
                firstName = contact.FirstName,
                lastName = contact.LastName,
                nickname = contact.Nickname,
                phone = contact.Phone,
                email = contact.Email,
                mailer = contact.Mailer,
                note = contact.Note,
                birthday = contact.BirthdayRaw
            };
        }

        public static List<ContactData> NewFromContactCollection(IReadOnlyCollection<Contact> contactsReadOnly) {
            var contacts = new List<Contact>(contactsReadOnly);
            var contactDatas = new List<ContactData>();
            foreach (var contact in contacts) {
                contactDatas.Add(NewFromContact(contact));
            }
            return contactDatas;
        }
    }
}
