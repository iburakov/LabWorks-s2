using System;
using Contacts.CommandLine;

namespace Contacts {

    // TODO: Setting/searching/displaying new fields thorugh CLI
    /* List of new fields:
     * * Birthday
     * * Mailer
     * * Nickname
     * * Note
    */

    // TODO: Saving to VCard through CLI
    // TODO: Reading from VCard through CLI

    public sealed class Program {
        private static Menu NewMainMenu(Menu searchMenu, IContactsStorage storage) {
            var mainMenu = new Menu("Menu:");
            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.GetAllContacts());
            }));

            mainMenu.AddItem(new MenuItem("Search", searchMenu.Invoke));

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
                    storage.FindByFirstName(substring)
                );
            }));

            searchMenu.AddItem(new MenuItem("Last name", () => {
                string substring = IO.GetString("Last name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByLastName(substring)
                );
            }));
           
            searchMenu.AddItem(new MenuItem("Full name", () => {
                string substring = IO.GetString("Full name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByFullName(substring)
                );
            }));

            searchMenu.AddItem(new MenuItem("Phone", () => {
                string substring = Contact.NormalizePhone(IO.GetString("Phone substring: "));

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByPhone(substring)
                );
            }));

            searchMenu.AddItem(new MenuItem("Email", () => {
                string substring = IO.GetString("Email substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindByEmail(substring)
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
