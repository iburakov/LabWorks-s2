using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class WcfContactsStorage : IContactsStorage, IDisposable {
        private ContactsWcfServiceClient client = new ContactsWcfServiceClient();

        public void AddContact(Contact newContact, out string message) {
            try {
                message = client.AddContact(newContact.ToContactData());
            }
            catch (Exception e) when (
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                message = $"An exception occurred: {e.Message}";
            }
        }

        public void Dispose() {
            if (client.State == CommunicationState.Faulted) {
                client.Abort();
            } else {
                client.Close();
            }
        }

        public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            var contactDatas = client.FindBy((WcfServiceReference.ContactFieldKind)fieldKind, query) as ReadOnlyCollection<ContactData>;
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            var contactDatas = client.GetAllContacts() as ReadOnlyCollection<ContactData>;
            return Contact.NewFromContactDataCollection(contactDatas);
        }

    }
}
