using System;
using System.Collections.Generic;
using System.ServiceModel;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class WcfContactsStorage : IRemoteContactsStorage, IDisposable {
        private ContactsWcfServiceClient client;

        public bool IsGreetingSuccessful { get; private set; }

        public WcfContactsStorage() {
            client = new ContactsWcfServiceClient();
            IsGreetingSuccessful = client.Greet();
        }

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
            List<ContactData> contactDatas = client.FindBy((WcfServiceReference.ContactFieldKind)fieldKind, query);
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public IReadOnlyCollection<Contact> GetAllContacts() {
            List<ContactData> contactDatas = client.GetAllContacts();
            return Contact.NewFromContactDataCollection(contactDatas);
        }

    }
}
