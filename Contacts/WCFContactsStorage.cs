using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contacts {
    public class WCFContactsStorage /*: IContactsStorage*/{
    //    private void ProcessUsingWcfClient(Func<ContactsWcfServiceClient, string> action, out string message) {
    //        var client = new ContactsWcfServiceClient();
    //        try {
    //            message = action.Invoke(client);
    //            client.Close();
    //        }
    //        catch (Exception e) when (
    //            e is FaultException ||
    //            e is CommunicationException ||
    //            e is TimeoutException
    //        ) {
    //            client.Abort();
    //            message = $"An exception occurred: {e.Message}";
    //        }
    //    }

    //    public void AddContact(Contact newContact, out string message) {
    //        ProcessUsingWcfClient((client) => {
    //            // TODO: reinvent the special contact class
    //            var castedContact = new WcfService.Contact {
    //                FirstName = newContact.FirstName,
    //                LastName = newContact.LastName,
    //                Mailer = newContact.Mailer,
    //                Nickname = newContact.Nickname,
    //                Note = newContact.Note,
    //                Phone = newContact.Phone,
    //                Email = newContact.Email
    //            };

    //            return client.AddContact(castedContact);
    //        }, out message);
    //    }

    //    public IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
    //        ReadOnlyCollection<Contact> r
    //        ProcessUsingWcfClient((client) => {
    //            client.FindBy((WcfService.Contact.FieldKind)fieldKind, query);
    //            return 

    //        }, out string message);
    //    }

    //    public IReadOnlyCollection<Contact> GetAllContacts() {
    //        throw new NotImplementedException();
    //    }
    }
}
