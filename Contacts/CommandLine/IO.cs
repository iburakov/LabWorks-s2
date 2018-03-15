using System;
using System.Collections.Generic;

namespace Contacts.CommandLine {

    public static class IO {

        public static void PrintContactList(String header, List<Contact> contacts) {
            if (contacts.Count == 0) {
                Console.WriteLine(header + ": nothing.");
                return;
            }

            Console.WriteLine("{0} ({1}):", header, contacts.Count);
            for (int i = 0; i < contacts.Count; ++i) {
                Console.Write("\t#{0}: ", i + 1);
                PrintContact(contacts[i]);
            }
        }

        public static void PrintContact(Contact contact) {
            Console.WriteLine(contact.ToString());
        }

        public static Contact GetContact() {
            return new Contact(
                firstName:  GetFirstName(),
                lastName:   GetLastName(),
                phone:      GetPhone(),
                email:      GetEmail()
            );
        }

        private static String GetField(String fieldKind, Contact.FieldValidator validate) {
            Boolean gotString = false;
            while (!gotString) {
                Console.Write("Enter {0}: ", fieldKind);
                String newField = Console.ReadLine();

                if (!validate(newField, out String errorMessage)) {
                    Console.WriteLine("{0}. Try again?", errorMessage);
                    if (GetBoolean()) {
                        break;
                    } else {
                        continue;
                    }
                }

                return newField;
            }

            throw new UserRefusedException();
        }

        public static String GetFirstName() {
            return GetField("first name", Contact.FirstNameValid);
        }

        public static String GetLastName() {
            return GetField("last name", Contact.LastNameValid);
        }

        public static String GetPhone() {
            return GetField("phone", Contact.PhoneValid);
        }

        public static String GetEmail() {
            return GetField("email", Contact.EmailValid);
        }

        public static Boolean GetBoolean() {
            Console.WriteLine("[\"no\"/anything else]");
            return Console.ReadLine().Trim(' ', '\n', '\t') == "no";
        }

        public static String GetString(String header = "String: ") {
            Boolean gotString = false;
            while (!gotString) {
                Console.Write(header);
                String newString = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(newString)) {
                    Console.WriteLine("Empty strings are not allowed here. Try again?");
                    if (GetBoolean()) {
                        break;
                    } else {
                        continue;
                    }
                }

                return newString;
            }

            throw new UserRefusedException();
        }

        public static Int16 GetInt16(String header = "Number: ", Boolean askAgain = true) {
            Boolean gotNumber = false;

            while (!gotNumber) {
                Console.Write(header);
                String newNumberStr = Console.ReadLine();
                
                if (!Int16.TryParse(newNumberStr, out Int16 newNumber)) {
                    if (askAgain) {
                        Console.WriteLine("Incorrect number. Try again?");
                        if (GetBoolean()) {
                            break;
                        } else {
                            continue;
                        }
                    } else {
                        Console.WriteLine("Incorrect number.");
                        throw new ArgumentException();
                    }
                    
                }

                return newNumber;
            }

            throw new UserRefusedException();
        }
    }
}
