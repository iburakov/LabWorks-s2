using Contacts.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.Server {
    public sealed class Program {
        public static void Main(string[] args) {
            Uri listenerUri = null;

            if (args.Length > 0) {
                try {
                    listenerUri = new Uri(args[0], UriKind.Absolute);
                } catch (UriFormatException) {
                    Console.WriteLine("Couldn't parse the URL given in command-line arguments.");
                    listenerUri = null;
                }
            }

            if (listenerUri is null) {
                listenerUri = IO.ReadUri("Enter URL to listen (http[s]://host[:port]): ");
            }

            var server = new ContactsServer(listenerUri, new LocalContactsStorage());

            server.Run();
        }
    }
}
