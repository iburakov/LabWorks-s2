using Contacts.CommandLine;
using System;
using System.Net;

namespace Contacts.Server {
    public sealed class Program {
        public static void Main(string[] args) {
            Uri listenerUri = null;

            if (args.Length > 0) {
                if (!IO.TryParseUri(args[0], out listenerUri)) {
                    Console.WriteLine("Couldn't parse the URL given in command-line arguments.");
                    listenerUri = null;
                }
            }

            if (listenerUri is null) {
                listenerUri = IO.ReadUri("Enter URL to listen (http://host[:port]): ");
            }

            try {
                var server = new ContactsServer(listenerUri, new LocalContactsStorage());
                server.Run();
            }
            catch (HttpListenerException e) {
                Console.WriteLine($"Can't start server: {e.Message}");
                return;
            }

        }
    }
}
