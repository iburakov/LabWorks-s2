﻿using MixERP.Net.VCards;
using MixERP.Net.VCards.Models;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static Contact ReadContact() {
            return new Contact(
                firstName:  ReadFirstName(),
                lastName:   ReadLastName(),
                nickname:   ReadNickname(),
                phone:      ReadPhone(),
                email:      ReadEmail(),
                mailer:     ReadMailer(),
                note:       ReadNote(),
                birthday:   ReadBirthday()
            );
        }

        private static string ReadField(string fieldKind, Contact.FieldValidator<string> validator) {
            bool gotString = false;
            while (!gotString) {
                Console.Write("Enter {0}: ", fieldKind);
                string newField = Console.ReadLine();

                if (!validator.Invoke(newField, out string errorMessage)) {
                    Console.WriteLine($"{errorMessage}. Try again?");
                    if (ReadBoolean(yesByDefault: true)) {
                        continue;
                    } else {
                        break;
                    }
                }

                return newField;
            }

            throw new UserRefusedException();
        }

        public static string ReadFirstName() {
            return ReadField("first name", Contact.IsFirstNameValid);
        }

        public static string ReadLastName() {
            return ReadField("last name", Contact.IsLastNameValid);
        }

        public static string ReadNickname() {
            return ReadField("nickname", Contact.IsNicknameValid);
        }

        public static string ReadPhone() {
            return ReadField("phone", Contact.IsPhoneValid);
        }

        public static string ReadEmail() {
            return ReadField("email", Contact.IsEmailValid);
        }

        public static string ReadMailer() {
            return ReadField("mailer", Contact.IsMailerValid);
        }

        public static string ReadNote() {
            return ReadField("note", Contact.IsNoteValid);
        }

        public static string ReadBirthday() {
            return DateTime.Parse(ReadField("birthday", Contact.IsBirthdayValid)).ToShortDateString();
        }

        public static bool ReadBoolean(bool yesByDefault) {
            string options = (yesByDefault) ? "[Y/n]" : "[y/N]";
            Console.WriteLine(options);

            bool? parseResult = null;
            var yesOptions = new List<string> { "y", "ye", "yes" };
            var noOptions = new List<string> { "n", "no" };

            while (parseResult is null) {
                string input = Console.ReadLine().Trim(' ', '\n', '\t').ToLower();

                if (input == String.Empty) {
                    return yesByDefault;
                }

                if (yesOptions.Contains(input)) {
                    parseResult = true;
                } else if (noOptions.Contains(input)) {
                    parseResult = false;
                } else {
                    Console.WriteLine($"Try again {options}:");
                }
            };

            return (bool)parseResult;
            
        }

        public static string ReadString(string header = "String: ") {
            string answer = null;

            do {
                Console.Write(header);
                answer = Console.ReadLine();

                if (answer == String.Empty) {
                    Console.WriteLine("Empty strings are not allowed here. Try again?");
                    if (ReadBoolean(yesByDefault: true) == false) {
                        throw new UserRefusedException();
                    }
                }

            } while (answer is null);

            return answer;
        }

        public static short ReadInt16(string header = "Number: ", bool askAgain = true) {
            bool gotNumber = false;

            while (!gotNumber) {
                Console.Write(header);
                string newNumberStr = Console.ReadLine();
                
                if (!Int16.TryParse(newNumberStr, out short newNumber)) {
                    if (askAgain) {
                        Console.WriteLine("Incorrect number. Try again?");
                        if (ReadBoolean(yesByDefault: true)) {
                            continue;
                        } else {
                            break;
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

        public static void LoadContactsFromVCard(string filename, IContactsStorage storage) {
            if (!File.Exists(filename)) {
                Console.WriteLine($"File \"{filename}\" doesn't exist!");
                return;
            }

            // github.com/mixerp/MixERP.Net.VCards is licensed under the Apache License 2.0 - private use is allowed
            IEnumerable<VCard> vcards = Deserializer.Deserialize(filename);

            int addedCounter = 0, totalCounter = 0;
            foreach (var vcard in vcards) {
                try {
                    ++totalCounter;

                    var telephones = new List<Telephone>(vcard.Telephones);
                    var emails = new List<Email>(vcard.Emails);

                    storage.Add(new Contact(
                        firstName: vcard.FirstName,
                        lastName: vcard.LastName,
                        nickname: vcard.NickName,
                        phone: telephones[0].Number,
                        email: emails[0].EmailAddress,
                        mailer: vcard.Mailer,
                        note: vcard.Note,
                        birthday: vcard.BirthDay?.ToShortDateString()
                    ));

                    ++addedCounter;
                } catch (Exception e) when (
                    e is NullReferenceException ||
                    e is ArgumentException
                ) {
                    Console.WriteLine($"Error! Corrupted contact ({e.GetType()}: {e.Message})");
                }
                
            }

            string s = (totalCounter == 1) ? "" : "s";
            string were = (addedCounter == 1) ? "was" : "were";
            Console.WriteLine($"{addedCounter} out of {totalCounter} contact{s} {were} loaded.");
        }

        public static void SaveContactsToVCard(string filename, IContactsStorage storage) {
            try {
                File.WriteAllText(filename, String.Join("\n\n", storage.GetAllContacts().ConvertAll(contact => contact.ToVCard())));
                Console.WriteLine("Done.");
            } catch (Exception e) when (
                e is ArgumentException ||
                e is PathTooLongException ||
                e is DirectoryNotFoundException ||
                e is IOException ||
                e is UnauthorizedAccessException ||
                e is NotSupportedException
            ) {
                Console.WriteLine($"Error! {e.Message}");
            }
            
        }
    }
}
