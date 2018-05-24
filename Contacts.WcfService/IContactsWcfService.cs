using System.Collections.Generic;
using System.ServiceModel;

namespace Contacts.WcfService {
    [ServiceContract]
    public interface IContactsWcfService {
        [OperationContract]
        IReadOnlyCollection<ContactData> GetAllContacts();

        [OperationContract]
        IReadOnlyCollection<ContactData> FindBy(Contact.FieldKind fieldKind, string query);

        [OperationContract]
        string AddContact(ContactData contact);
    }
}
