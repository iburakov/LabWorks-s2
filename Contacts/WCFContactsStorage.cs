using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class WcfContactsStorage : IContactsStorage {
        public void AddContact(Contact newContact, out string message) {
            var client = new ContactsWcfServiceClient();
            try {
                message = client.AddContact(newContact.ToContactData());
                client.Close();
            }
            catch (Exception e) when (
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                client.Abort();
                message = $"An exception occurred: {e.Message}";
            }
        }

        public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            var client = new ContactsWcfServiceClient();
            try {
                var contactDatas = client.FindBy((WcfServiceReference.ContactFieldKind)fieldKind, query) as ReadOnlyCollection<ContactData>;
                var result = Contact.NewFromContactDataCollection(contactDatas);
                client.Close();
                return result;
            }
            catch (Exception e) when (
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                client.Abort();
                throw;
            }
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            var client = new ContactsWcfServiceClient();
            try {
                var contactDatas = client.GetAllContacts() as ReadOnlyCollection<ContactData>;
                var result = Contact.NewFromContactDataCollection(contactDatas);
                client.Close();
                return result;
            }
            catch (Exception e) when (
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                client.Abort();
                throw;
            }
        }

    }
}
