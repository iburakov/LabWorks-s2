using System;
using System.Collections.Generic;
using System.IO;
using Contacts.CommandLine;

namespace Contacts {

    public sealed class Program {
        private static Menu NewMainMenu(Menu searchMenu, IContactsStorage storage) {
            var mainMenu = new Menu("Menu:");

            mainMenu.AddItem(new MenuItem("Load contacts from VCard", () => {
                IO.LoadContactsFromVCard(IO.GetString("Filename: "), storage);
            }));

            mainMenu.AddItem(new MenuItem("Save contacts to VCard", () => {
                string filename = IO.GetString("Filename: ");
                if (File.Exists(filename)) {
                    Console.WriteLine($"File \"{filename}\" already exists. Rewrite?");
                    if (!IO.GetBoolean(yesByDefault: false)) return;
                }
                IO.SaveContactsToVCard(filename, storage);
            }));

            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.GetAllContacts());
            }));

            mainMenu.AddItem(new MenuItem("Search", searchMenu.Invoke));

            mainMenu.AddItem(new MenuItem("New contact", () => storage.Add(IO.GetContact())));

            return mainMenu;
        }


        private delegate List<Contact> ContactsFinder(string substring);
        private const string searchResultsHeader = "Search results";

        private static void FieldSearchHandler(string field, ContactsFinder finder) {
            string substring = IO.GetString($"{field} substring: ");

            IO.PrintContactList(
                searchResultsHeader,
                finder.Invoke(substring)
            );
        }

        private static Menu NewSearchMenu(IContactsStorage storage) {
            var searchMenu = new Menu("Search by:");

            searchMenu.AddItem(new MenuItem("First name", () => {
                FieldSearchHandler("First name", storage.FindByFirstName);
            }));

            searchMenu.AddItem(new MenuItem("Last name", () => {
                FieldSearchHandler("Last name", storage.FindByLastName);
            }));

            searchMenu.AddItem(new MenuItem("Nickname", () => {
                FieldSearchHandler("Nickname", storage.FindByNickname);
            }));

            searchMenu.AddItem(new MenuItem("Full name", () => {
                FieldSearchHandler("Full name", storage.FindByFullName);
            }));

            searchMenu.AddItem(new MenuItem("Phone", () => {
                string substring = Contact.NormalizePhone(IO.GetString("Phone substring: "));

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByPhone(substring)
                );
            }));

            searchMenu.AddItem(new MenuItem("Email", () => {
                FieldSearchHandler("Email", storage.FindByEmail);
            }));

            searchMenu.AddItem(new MenuItem("Mailer", () => {
                FieldSearchHandler("Mailer", storage.FindByMailer);
            }));

            searchMenu.AddItem(new MenuItem("Note", () => {
                FieldSearchHandler("Note", storage.FindByNote);
            }));

            searchMenu.AddItem(new MenuItem("Birthday", () => {
                string birthday = IO.GetBirthday();

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByBirthday(birthday)
                );
            }));

            return searchMenu;
        }

        public static void Main(string[] args) {
            var localStorage = new LocalContactsStorage();

            Menu searchMenu = NewSearchMenu(localStorage);
            Menu mainMenu = NewMainMenu(searchMenu, localStorage);

            bool needsExit = false;
            mainMenu.AddItem(new MenuItem("Exit", () => needsExit = true));

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
