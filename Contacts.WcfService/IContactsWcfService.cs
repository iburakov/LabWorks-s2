using System.Collections.Generic;
using System.ServiceModel;

namespace Contacts.WcfService {
    [ServiceContract]
    public interface IContactsWcfService {
        [OperationContract]
        List<ContactData> GetAllContacts();

        [OperationContract]
        List<ContactData> FindBy(Contact.FieldKind fieldKind, string query);

        [OperationContract]
        string AddContact(ContactData contact);

        [OperationContract]
        bool Greet();
    }
}
