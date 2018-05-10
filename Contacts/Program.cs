using System;
using System.Collections.Generic;
using System.IO;
using Contacts.CommandLine;


namespace Contacts {

    public sealed class Program {
        private static Menu NewMainMenu(Menu searchMenu, IContactsStorage storage) {
            var mainMenu = new Menu("Menu:");

            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.GetAllContacts());
            }));

            mainMenu.AddItem(new MenuItem("Search", searchMenu.Invoke));

            mainMenu.AddItem(new MenuItem("New contact", () => {
                storage.AddContact(IO.ReadContact(), out string message);
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


        private delegate List<Contact> ContactsFinder(string substring);
        private const string searchResultsHeader = "Search results";

        private static void SearchByField(Contact.FieldKind fieldKind, IContactsStorage storage) {
            string query;
            if (fieldKind != Contact.FieldKind.Birthday) {
                query = IO.ReadString(Contact.GetFieldKindName(fieldKind) + " substring:");
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
            
            foreach(var fieldKind in (Contact.FieldKind[])typeof(Contact.FieldKind).GetEnumValues()) {
                searchMenu.AddItem(new MenuItem(Contact.GetFieldKindName(fieldKind), () => {
                    SearchByField(fieldKind, storage);
                }));
            }

            searchMenu.AddItem(new MenuItem("Back", () => { return; }));

            return searchMenu;
        }

        public static void Main(string[] args) {
            if (args.Length == 0) {

            }

            var localStorage = new LocalContactsStorage();

            Menu searchMenu = NewSearchMenu(localStorage);
            Menu mainMenu = NewMainMenu(searchMenu, localStorage);

            bool needsExit = false;
            mainMenu.AddItem(new MenuItem("Exit", () => {
                Console.WriteLine("Are you sure?");
                needsExit = IO.ReadBoolean(yesByDefault: false);
            }));

            Console.WriteLine("Enter the number of action and press [Enter]. Then follow instructions.");
            while (!needsExit) {
                try {
                    mainMenu.Invoke();
                }
                catch (UserRefusedException) {
                    continue;
                }
            }


        }

    }

}
