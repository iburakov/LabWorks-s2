using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Contacts.CommandLine;

namespace Contacts {

    public sealed class ContactsProgram {
        private static Menu NewMainMenu(Menu searchMenu, IContactsStorage storage) {
            var mainMenu = new Menu("Menu:");

            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.GetAllContacts());
            }));

            mainMenu.AddItem(new MenuItem("Search", searchMenu.Invoke));

            mainMenu.AddItem(new MenuItem("New contact", () => {
                var newContact = IO.ReadContact();
                storage.AddContact(newContact, out string message);
                Console.WriteLine(message);
            }));

            mainMenu.AddItem(new MenuItem("Load contacts from VCard", () => {
                IO.LoadContactsFromVCard(IO.ReadString("Filename: "), storage);
            }));

            mainMenu.AddItem(new MenuItem("Save contacts to VCard", () => {
                string filename = IO.ReadString("Filename: ");
                if (File.Exists(filename)) {
                    Console.WriteLine($"File \"{filename}\" already exists. Rewrite?");
                    if (!IO.ReadBoolean(yesByDefault: false)) return;
                }
                IO.SaveContactsToVCard(filename, storage);
            }));

            return mainMenu;
        }


        private const string searchResultsHeader = "Search results";

        private static void SearchByField(Contact.FieldKind fieldKind, IContactsStorage storage) {
            string query;
            if (fieldKind != Contact.FieldKind.Birthday) {
                query = IO.ReadString(Contact.GetFieldKindName(fieldKind) + " substring: ");
            } else {
                query = IO.ReadBirthday();
            }

            IO.PrintContactList(
                searchResultsHeader,
                storage.FindByField(fieldKind, query)
            );
        }

        private static Menu NewSearchMenu(IContactsStorage storage) {
            var searchMenu = new Menu("Search by:");

            foreach (var fieldKind in (Contact.FieldKind[])typeof(Contact.FieldKind).GetEnumValues()) {
                searchMenu.AddItem(new MenuItem(Contact.GetFieldKindName(fieldKind), () => {
                    SearchByField(fieldKind, storage);
                }));
            }

            searchMenu.AddItem(new MenuItem("Back", () => { return; }));

            return searchMenu;
        }




        public static void Main(string[] args) {
            IContactsStorage storage;
            if (args.Length == 0) {
                storage = new LocalContactsStorage();
            } else if (args[0] == "--wcf" || args[0] == "/wcf") {
                if (args.Length > 1) {
                    Console.WriteLine("Note: WCF client configuration is done with App.config file. Ignoring arguments.");
                }
                try {
                    storage = new WcfContactsStorage();
                }
                catch (AggregateException notFlattenedAe) {
                    AggregateException ae = notFlattenedAe.Flatten();
                    Console.WriteLine($"Could not connect to server!\n\n" +
                        $"Technical details: {ae.InnerExceptions[ae.InnerExceptions.Count - 1].Message}");
                    return;
                }
            } else {
                try {
                    storage = new WebApiContactsStorage(args[0]);
                }
                catch (Exception e) when (
                    e is ArgumentException ||
                    e is UriFormatException
                ){
                    Console.WriteLine(e.Message);
                    return;
                }
            }


            Menu searchMenu = NewSearchMenu(storage);
            Menu mainMenu = NewMainMenu(searchMenu, storage);

            bool needsExit = false;
            mainMenu.AddItem(new MenuItem("Exit", () => {
                Console.WriteLine("Are you sure?");
                needsExit = IO.ReadBoolean(yesByDefault: false);
            }));


            if (storage is RemoteContactsStorage remoteStorage) {
                if (remoteStorage.IsGreetingSuccessful) {
                    Console.WriteLine($"Connected.");
                } else {
                    Console.WriteLine($"Bad server. Terminating.");
                    return;
                }
            } else {
                Console.WriteLine("Working with local contact storage.");
            }
            Console.WriteLine("Enter the number of action and press [Enter]. Then follow instructions.");

            while (!needsExit) {
                try {
                    mainMenu.Invoke();
                }
                catch (UserRefusedException) {
                    continue;
                }
                catch (AggregateException notFlattenedAe) {
                    AggregateException ae = notFlattenedAe.Flatten();
                    Console.WriteLine($"An exception occurred: {ae.InnerExceptions[ae.InnerExceptions.Count - 1].Message}");
                }
                catch (HttpRequestException e) {
                    Console.WriteLine($"An exception occurred: {e.Message}");
                }

            }

            if (storage is WcfContactsStorage wcfContactsStorage) {
                wcfContactsStorage.Dispose();
            }

        }

    }

}
