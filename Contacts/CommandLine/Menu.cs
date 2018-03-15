using System;
using System.Collections.Generic;

namespace Contacts.CommandLine {

    internal struct MenuItem {
        public string Name { get; }
        public Action Handler { get; }

        public MenuItem(string name, Action handler) {
            Name = name;
            Handler = handler;
        }
    }

    internal class Menu {
        private String title;
        private List<MenuItem> items = new List<MenuItem>();

        public Menu(String title = null) {
            if (title != null) {
                this.title = title;
            }
        }

        public void AddItem(MenuItem newItem) => items.Add(newItem);

        public void Display() {
            Console.WriteLine(title);
            for (Int32 i = 0; i < items.Count; ++i) {
                Console.WriteLine("\t#{0}: {1}", i + 1, items[i].Name);
            }
        }

        public void Deploy() {
            Display();

            Boolean gotIndex = false;
            Int16 selectedIndex = -1;
            while (!gotIndex) {
                try {
                    selectedIndex = IO.GetInt16("Select an option: ", askAgain: false);

                    if (selectedIndex <= 0 || selectedIndex > items.Count) {
                        Console.WriteLine("Your choice is out of range.");
                        throw new ArgumentException();
                    }

                    gotIndex = true;
                }
                catch (ArgumentException) {
                    continue;
                }
            }

            items[selectedIndex - 1].Handler();
        }
    }

}
