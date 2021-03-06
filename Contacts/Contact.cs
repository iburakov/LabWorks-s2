﻿using MixERP.Net.VCards;
using MixERP.Net.VCards.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Contacts.WcfServiceReference;

namespace Contacts {
    public class Contact {
        private const short MaxFirstNameLength = 16;
        private const short MaxLastNameLength = 36;
        private const short MinPhoneLength = 3;
        private const short MaxNicknameLength = 100;
        private const short MaxMailerLength = 50;
        private const short MaxNoteLength = 500;

        public override bool Equals(object obj) {
            if (obj is Contact another) {
                return
                    FirstName == another.FirstName &&
                    LastName == another.LastName &&
                    Nickname == another.Nickname &&
                    Phone == another.Phone &&
                    Email == another.Email &&
                    Mailer == another.Mailer &&
                    Note == another.Note &&
                    Birthday == another.Birthday;
            } else {
                return false;
            }
        }

        public delegate bool FieldValidator<T>(T value, out string errorMessage);

        private void ValidateAndSetField<T>(ref T fieldToSet, T value, FieldValidator<T> fieldValidator) {
            if (fieldValidator.Invoke(value, out string errorMessage)) {
                fieldToSet = value;
            } else {
                throw new ArgumentException(errorMessage);
            }
        }

        public enum FieldKind { FirstName, LastName, FullName, Nickname, Email, Mailer, Phone, Note, Birthday };

        public static string GetFieldKindName(FieldKind fieldKind) {
            switch (fieldKind) {
                case FieldKind.FirstName: return "First name";
                case FieldKind.LastName: return "Last name";
                case FieldKind.FullName: return "Full name";
                default: return fieldKind.ToString();
            }
        }

        private static bool IsFieldValid(FieldKind fieldKind, string value, out string errorMessage) {
            if ((value is null) || (value.Length == 0)) {
                errorMessage = $"{fieldKind} can't be empty";
                return false;
            }

            short maxFieldLength;
            switch (fieldKind) {
                case FieldKind.FirstName: maxFieldLength = MaxFirstNameLength; break;
                case FieldKind.LastName: maxFieldLength = MaxLastNameLength; break;
                case FieldKind.Nickname: maxFieldLength = MaxNicknameLength; break;
                case FieldKind.Mailer: maxFieldLength = MaxMailerLength; break;
                case FieldKind.Note: maxFieldLength = MaxNoteLength; break;
                default: throw new ArgumentException("IsFieldValid used for a FieldKind that has its own special validator.");
            }

            var isLengthValid = value.Length <= maxFieldLength;

            errorMessage = isLengthValid ? string.Empty : $"{fieldKind} is too long";

            return isLengthValid;
        }

        public static bool IsFirstNameValid(string value, out string errorMessage) {
            return IsFieldValid(FieldKind.FirstName, value, out errorMessage);
        }

        public static bool IsLastNameValid(string value, out string errorMessage) {
            return IsFieldValid(FieldKind.LastName, value, out errorMessage);
        }

        public static bool IsNicknameValid(string value, out string errorMessage) {
            return IsFieldValid(FieldKind.Nickname, value, out errorMessage);
        }

        private string firstName;
        public string FirstName {
            get => firstName;
            set {
                ValidateAndSetField(ref firstName, value, IsFirstNameValid);
            }
        }

        private string lastName;
        public string LastName {
            get => lastName;
            set {
                ValidateAndSetField(ref lastName, value, IsLastNameValid);
            }
        }

        public string FullName { get => $"{FirstName} {LastName}"; }

        private string nickname;
        public string Nickname {
            get => nickname;
            set {
                ValidateAndSetField(ref nickname, value, IsNicknameValid);
            }
        }

        public static string NormalizePhone(string phone) {
            return Regex.Replace(phone, @"[\-()\s]", ""); ;
        }

        public static bool IsPhoneValid(string value, out string errorMessage) {
            if (value.Length < MinPhoneLength) {
                errorMessage = "Phone is too short";
                return false;
            }

            bool allCharsAreDigits = true;
            foreach (char c in Regex.Replace(value, @"[\-+()\s]", "")) {
                if (!Char.IsDigit(c)) {
                    allCharsAreDigits = false;
                    break;
                }
            }

            if (!allCharsAreDigits) {
                errorMessage = "Phone number contains invalid characters";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private string phone;
        public string NormalizedPhone { get => NormalizePhone(phone); }
        public string Phone {
            get => phone;
            set {
                ValidateAndSetField(ref phone, value, IsPhoneValid);
            }
        }

        public static bool IsEmailValid(string value, out string errorMessage) {
            try {
                var parsed = new MailAddress(value);
                if (parsed.Address == value) {
                    errorMessage = null;
                    return true;
                } else {
                    throw new ArgumentException();
                }
            }
            catch {
                errorMessage = "E-mail is not correct";
                return false;
            }
        }

        private string email;
        public string Email {
            get => email;
            set {
                ValidateAndSetField(ref email, value, IsEmailValid);
            }
        }

        public static bool IsMailerValid(string value, out string errorMessage) {
            return IsFieldValid(FieldKind.Mailer, value, out errorMessage);
        }

        public static bool IsNoteValid(string value, out string errorMessage) {
            return IsFieldValid(FieldKind.Note, value, out errorMessage);
        }

        private string mailer;
        public string Mailer {
            get => mailer;
            set {
                ValidateAndSetField(ref mailer, value, IsMailerValid);
            }
        }

        private string note;
        public string Note {
            get => note;
            set {
                ValidateAndSetField(ref note, value, IsNoteValid);
            }
        }

        private DateTime birthday;
        public string Birthday {
            get => birthday.ToShortDateString();
            set {
                if (IsBirthdayValid(value, out string errorMessage)) {
                    birthday = DateTime.Parse(value);
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }
        public DateTime BirthdayRaw { get => birthday; }

        public static bool IsBirthdayValid(string value, out string errorMessage) {
            if (value.Length == 0) {
                errorMessage = "A contact must have a birthday";
                return false;
            }

            try {
                var parsedDate = DateTime.Parse(value);
                if (parsedDate.Year == 1) {
                    errorMessage = "A contact must have a birthday";
                    return false;
                };
                errorMessage = "";
                return true;
            } catch (FormatException e) {
                errorMessage = e.Message;
                return false;
            }
        }

        public override string ToString() {
            return $"{FirstName} {nickname} {LastName}, born {Birthday}, tel: {Phone}, email({Mailer}): {Email} - {Note}";
        }

        public bool IsValid() {
            return 
                firstName != null && IsFirstNameValid(firstName, out string errorMessage) &&
                lastName != null && IsLastNameValid(lastName, out errorMessage) &&
                nickname != null && IsNicknameValid(nickname, out errorMessage) &&
                email != null && IsEmailValid(email, out errorMessage) &&
                phone != null && IsPhoneValid(phone, out errorMessage) &&
                mailer != null && IsMailerValid(mailer, out errorMessage) &&
                note != null && IsNoteValid(note, out errorMessage) &&
                birthday != null && IsBirthdayValid(birthday.ToShortDateString(), out errorMessage);
        }

        public static string ToVCardMany(IReadOnlyCollection<Contact> contacts) {
            var contactsList = new List<Contact>(contacts);
            return String.Join("\n\n", contactsList.ConvertAll(contact => contact.ToVCard()));
        }

        public string ToVCard() {
            return
$@"
BEGIN:VCARD
VERSION:4.0
FN:{LastName + " " + FirstName}
N:{LastName};{FirstName};;;
NICKNAME:{Nickname}
BDAY:{BirthdayRaw.ToString("yyyy-MM-dd")}
TEL:{Phone}
EMAIL:{Email}
MAILER:{Mailer}
NOTE:{Note}
END:VCARD
";
        }

        public static Contact Parse(string vcardStr) {
            VCard vcard = Deserializer.GetVCard(vcardStr);

            var telephones = new List<Telephone>(vcard.Telephones);
            var emails = new List<Email>(vcard.Emails);

            return new Contact {
                FirstName = vcard.FirstName,
                LastName = vcard.LastName,
                Nickname = vcard.NickName,
                Phone = telephones[0].Number,
                Email = emails[0].EmailAddress,
                Mailer = vcard.Mailer,
                Note = vcard.Note,
                Birthday = vcard.BirthDay?.ToShortDateString()
            };
        }

        public static List<Contact> ParseMany(string vcards, out int parsedCounter, out int totalCounter) {
            IEnumerable<VCard> parsedVcards = Deserializer.GetVCards(vcards);

            parsedCounter = 0;
            totalCounter = 0;
            var parsedContacts = new List<Contact>();

            foreach (var vcard in parsedVcards) {
                try {
                    ++totalCounter;

                    var telephones = new List<Telephone>(vcard.Telephones);
                    var emails = new List<Email>(vcard.Emails);

                    parsedContacts.Add(new Contact {
                        FirstName = vcard.FirstName,
                        LastName = vcard.LastName,
                        Nickname = vcard.NickName,
                        Phone = telephones[0].Number,
                        Email = emails[0].EmailAddress,
                        Mailer = vcard.Mailer,
                        Note = vcard.Note,
                        Birthday = vcard.BirthDay?.ToShortDateString()
                    });

                    // flow gets here only if no exceptions occurred 
                    ++parsedCounter;
                }
                catch (Exception e) when (
                  e is NullReferenceException ||
                  e is ArgumentException
                ) {
                    Console.WriteLine($"Error! Corrupted contact ({e.GetType()}: {e.Message})");
                }

            }

            return parsedContacts;
        }

        public ContactData ToContactData() {
            return new ContactData {
                firstName = firstName,
                lastName = lastName,
                nickname = nickname,
                phone = phone,
                email = email,
                mailer = mailer,
                note = note,
                birthday = birthday
            };
        }

        public static Contact NewFromContactData(ContactData contactData) {
            return new Contact {
                FirstName = contactData.firstName,
                LastName = contactData.lastName,
                Nickname = contactData.nickname,
                Phone = contactData.phone,
                Email = contactData.email,
                Mailer = contactData.mailer,
                Note = contactData.note,
                Birthday = contactData.birthday.ToShortDateString()
            };
        }

        public static IReadOnlyCollection<Contact> NewFromContactDataCollection(IReadOnlyCollection<ContactData> contactDatasReadOnly) {
            var contacts = new List<Contact>();
            foreach (var contactData in contactDatasReadOnly) {
                contacts.Add(NewFromContactData(contactData));
            }
            return contacts.AsReadOnly();
        }

    }
}