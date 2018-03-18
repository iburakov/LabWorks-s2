using System;
using Contacts.CommandLine;

namespace Contacts {

    // TODO: The Single Responsibility Principle and SOLID principles
    // TODO: Check out some design pattens

    public sealed class Program {
        private static Menu NewMainMenu(Menu searchMenu, IContactsStorage storage) {
            var mainMenu = new Menu("Menu:");
            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.FindAll(contact => true));
            }));

            mainMenu.AddItem(new MenuItem("Search", searchMenu.Deploy));

            mainMenu.AddItem(new MenuItem("New contact", () => storage.Add(IO.GetContact())));

            return mainMenu;
        }

        private static Menu NewSearchMenu(IContactsStorage storage) {
            var searchMenu = new Menu("Search by:");

            string searchResultsHeader = "Search results";

            searchMenu.AddItem(new MenuItem("First name", () => {
                string substring = IO.GetString("First name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => contact.FirstName.Contains(substring))
                );
            }));

            searchMenu.AddItem(new MenuItem("Last name", () => {
                string substring = IO.GetString("Last name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => contact.LastName.Contains(substring))
                );
            }));
           
            searchMenu.AddItem(new MenuItem("Full name", () => {
                string substring = IO.GetString("Full name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => (contact.FirstName + " " + contact.LastName)
                                               .Contains(substring))
                );
            }));

            searchMenu.AddItem(new MenuItem("Phone", () => {
                string substring = Contact.NormalizePhone(IO.GetString("Phone substring: "));

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => {
                        return contact.NormalizedPhone.Contains(substring);
                    })
                );
            }));

            searchMenu.AddItem(new MenuItem("Email", () => {
                string substring = IO.GetString("Email substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => contact.Email.Contains(substring))
                );
            }));

            return searchMenu;
        }

        public static void Main(string[] args) {
            var localStorage = new LocalStorage();

            Menu searchMenu = NewSearchMenu(localStorage);
            Menu mainMenu = NewMainMenu(searchMenu, localStorage);

            bool needsExit = false;
            mainMenu.AddItem(new MenuItem("Exit", () => needsExit = true));

            Console.WriteLine("Enter the number of action and press [Enter]. Then follow instructions.");
            while (!needsExit) {
                try {
                    mainMenu.Deploy();
                }
                catch (UserRefusedException) {
                    continue;
                }
            }


        }

    }

}
