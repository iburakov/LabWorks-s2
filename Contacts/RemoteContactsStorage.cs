using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Contacts {
    public abstract class RemoteContactsStorage : IContactsStorage {
        public bool IsGreetingSuccessful { get; protected set; }

        protected T WaitForTaskResult<T>(Task<T> task) {
            int checks = 0;
            int delay = 25;
            while (!task.IsCompleted) {
                if (checks++ > 4) {
                    Console.Write(".");
                }
                Thread.Sleep(delay += 25);
            }
            if (checks > 5) {
                Console.WriteLine();
            }

            return task.Result;
        }

        public abstract void AddContact(Contact newContact, out string message);

        public abstract IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query);

        public abstract IReadOnlyCollection<Contact> GetAllContacts();
    }
}
