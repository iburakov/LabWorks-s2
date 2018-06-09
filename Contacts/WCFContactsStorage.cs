using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class WcfContactsStorage : RemoteContactsStorage, IDisposable {
        private ContactsWcfServiceClient client;

        public Uri ConnectedToURI { get => client.Endpoint.ListenUri; }

        public WcfContactsStorage(bool isCommandLineInterface = true, string endpoint = null) {
            if (endpoint is null) {
                client = new ContactsWcfServiceClient();
            } else {
                client = new ContactsWcfServiceClient("WSHttpBinding_IContactsWcfService", endpoint);
            }

            this.isCommandLineInterface = isCommandLineInterface;
            if (isCommandLineInterface) Console.WriteLine($"Connecting to WCF remote storage at {client.Endpoint.ListenUri}");

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

        public async Task<string> AddContactAsync(Contact newContact, bool rethrowException = false) {
            try {
                return await client.AddContactAsync(newContact.ToContactData());
            }
            catch (Exception e) when (
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                if (rethrowException) {
                    throw;
                } else {
                    return $"An exception occurred: {e.Message}";
                }
            }
        }

        public override IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            List<ContactData> contactDatas = WaitForTaskResult(client.FindByAsync((WcfServiceReference.ContactFieldKind)fieldKind, query));
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public async Task<IReadOnlyCollection<Contact>> FindByFieldAsync(Contact.FieldKind fieldKind, string query) {
            List<ContactData> contactDatas = await client.FindByAsync((WcfServiceReference.ContactFieldKind)fieldKind, query);
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public override IReadOnlyCollection<Contact> GetAllContacts() {
            List<ContactData> contactDatas = WaitForTaskResult(client.GetAllContactsAsync());
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public async Task<IReadOnlyCollection<Contact>> GetAllContactsAsync() {
            List<ContactData> contactDatas = await client.GetAllContactsAsync();
            return Contact.NewFromContactDataCollection(contactDatas);
        }

        public void Dispose() {
            if (client.State == CommunicationState.Faulted) {
                client.Abort();
            } else {
                client.Close();
            }
        }
    }
}
