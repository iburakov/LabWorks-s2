using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Contacts.WcfService {
    [ServiceContract]
    public interface IContactsWcfService {
        [OperationContract]
        bool greet();

        [OperationContract]
        IReadOnlyCollection<Contact> GetAllContacts();

        [OperationContract]
        IReadOnlyCollection<Contact> FindBy(Contact.FieldKind fieldKind, string query);

        [OperationContract]
        string AddContact(Contact contact);
    }
}
