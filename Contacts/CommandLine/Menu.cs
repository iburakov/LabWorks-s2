using System;
using System.Collections.Generic;

namespace Contacts.CommandLine {

    internal class MenuItem {
        public string Name { get; }
        public Action Handler { get; }

        public MenuItem(string name, Action handler) {
            Name = name;
            Handler = handler;
        }
    }

    internal class Menu {
        private string _title;
        private List<MenuItem> menuItems = new List<MenuItem>();

        public Menu(string title = "") {
            _title = title;
        }

        public void AddItem(MenuItem newItem) => menuItems.Add(newItem);

        public void Display() {
            Console.WriteLine(_title);
            for (int i = 0; i < menuItems.Count; ++i) {
                Console.WriteLine("\t#{0}: {1}", i + 1, menuItems[i].Name);
            }
        }

        public void Invoke() {
            Display();

            bool gotIndex = false;
            short selectedIndex = -1;
            while (!gotIndex) {
                try {
                    selectedIndex = IO.ReadInt16("Select an option: ", askAgain: false);

                    if (selectedIndex <= 0 || selectedIndex > menuItems.Count) {
                        Console.WriteLine("Your choice is out of range.");
                    } else {
                        gotIndex = true;
                    }
                }
                catch (ArgumentException) {
                    continue;
                }
            }

            menuItems[selectedIndex - 1].Handler();
        }
    }

}
