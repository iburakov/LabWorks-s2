using System;
using System.Text.RegularExpressions;
using Contacts.CommandLine;

namespace Contacts {

    // TODO: Use string interpolation
    // TODO: The Single Responsibility Principle check and SOLID principles
    // TODO: Check out some design pattens 

    public sealed class Program { 

        private static Menu NewMainMenu(Menu searchMenu, IStorage storage) {
            var mainMenu = new Menu("Menu:");
            mainMenu.AddItem(new MenuItem("View all contacts", () => {
                IO.PrintContactList("All contacts", storage.FindAll(contact => true));
            }));

            mainMenu.AddItem(new MenuItem("Search", () => {
                searchMenu.Deploy();
            }));

            mainMenu.AddItem(new MenuItem("New contact", () => {
                storage.Add(IO.GetContact());
            }));

            return mainMenu;
        }

        private static Menu NewSearchMenu(IStorage storage) {
            var searchMenu = new Menu("Search by:");

            String searchResultsHeader = "Search results";

            searchMenu.AddItem(new MenuItem("First name", () => {
                String substring = IO.GetString("First name substring: ");

                IO.PrintContactList(
                    searchResultsHeader, 
                    storage.FindAll(contact => contact.FirstName.Contains(substring))
                );
            }));

            searchMenu.AddItem(new MenuItem("Last name", () => {
                String substring = IO.GetString("Last name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => contact.LastName.Contains(substring))
                );
            }));

            searchMenu.AddItem(new MenuItem("Full name", () => {
                String substring = IO.GetString("Full name substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => (contact.FirstName + " " + contact.LastName)
                                               .Contains(substring))
                );
            }));

            searchMenu.AddItem(new MenuItem("Phone", () => {
                String substring = Contact.NormalizePhone(IO.GetString("Phone substring: "));

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => {
                        return contact.NormalizedPhone.Contains(substring);
                    })
                );
            }));

            searchMenu.AddItem(new MenuItem("Email", () => {
                String substring = IO.GetString("Email substring: ");

                IO.PrintContactList(
                    searchResultsHeader,
                    storage.FindAll(contact => contact.Email.Contains(substring))
                );
            }));

            return searchMenu;

        }

        public static void Main(string[] args) {
            var localStorage = new LocalStorage();

            var searchMenu = NewSearchMenu(localStorage);
            var mainMenu = NewMainMenu(searchMenu, localStorage);

            Boolean needsExit = false;
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
