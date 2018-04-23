using System;
using System.Collections.Generic;

namespace Contacts.CommandLine {

    public static class IO {

        public static void PrintContactList(string header, List<Contact> contacts) {
            if (contacts.Count == 0) {
                Console.WriteLine(header + ": nothing.");
                return;
            }

            Console.WriteLine($"{header} ({contacts.Count}):");
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
                nickname:   GetNickname(),
                phone:      GetPhone(),
                email:      GetEmail(),
                mailer:     GetMailer(),
                note:       GetNote(),
                birthday:   GetBirthday()
            );
        }

        private static string GetField(string fieldKind, Contact.FieldValidator<string> validator) {
            bool gotString = false;
            while (!gotString) {
                Console.Write("Enter {0}: ", fieldKind);
                string newField = Console.ReadLine();

                if (!validator.Invoke(newField, out string errorMessage)) {
                    Console.WriteLine($"{errorMessage}. Try again?");
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

        public static string GetFirstName() {
            return GetField("first name", Contact.IsFirstNameValid);
        }

        public static string GetLastName() {
            return GetField("last name", Contact.IsLastNameValid);
        }

        private static string GetNickname() {
            return GetField("nickname", Contact.IsNicknameValid);
        }

        public static string  GetPhone() {
            return GetField("phone", Contact.IsPhoneValid);
        }

        public static string GetEmail() {
            return GetField("email", Contact.IsEmailValid);
        }

        private static string GetMailer() {
            return GetField("mailer", Contact.IsMailerValid);
        }

        private static string GetNote() {
            return GetField("note", Contact.IsNoteValid);
        }

        private static string GetBirthday() {
            return GetField("birthday", Contact.IsBirthdayValid);
        }

        public static bool GetBoolean() {
            Console.WriteLine("[\"no\"/anything else]");
            return Console.ReadLine().Trim(' ', '\n', '\t') == "no";
        }

        public static string GetString(string header = "String: ") {
            bool gotString = false;
            while (!gotString) {
                Console.Write(header);
                string newString = Console.ReadLine();

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

        public static short GetInt16(string header = "Number: ", bool askAgain = true) {
            bool gotNumber = false;

            while (!gotNumber) {
                Console.Write(header);
                string newNumberStr = Console.ReadLine();
                
                if (!Int16.TryParse(newNumberStr, out short newNumber)) {
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
