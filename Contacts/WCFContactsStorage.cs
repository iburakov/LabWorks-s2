using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class WcfContactsStorage : RemoteContactsStorage, IDisposable {
        private ContactsWcfServiceClient client;

        public WcfContactsStorage() {
            client = new ContactsWcfServiceClient();

            Console.WriteLine($"Connecting to WCF remote storage at {client.Endpoint.ListenUri}");
            IsGreetingSuccessful = WaitForTaskResult(client.GreetAsync()); ;
        }

        public override void AddContact(Contact newContact, out string message) {
            try {
                message = WaitForTaskResult(client.AddContactAsync(newContact.ToContactData()));
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

        public override IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            
            List<ContactData> contactDatas = WaitForTaskResult(client.FindByAsync((WcfServiceReference.ContactFieldKind)fieldKind, query)); ;
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public override IReadOnlyCollection<Contact> GetAllContacts() {
            List<ContactData> contactDatas = WaitForTaskResult(client.GetAllContactsAsync());
            return Contact.NewFromContactDataCollection(contactDatas);
        }

    }
}
