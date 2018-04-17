using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Contacts {
    public class Contact {
        private const short MaxFirstNameLength = 16;
        private const short MaxLastNameLength = 36;
        private const short MinPhoneLength = 3;
        private const short MaxNicknameLength = 100;
        private const short MaxMailerLength = 50;
        private const short MaxNoteLength = 500;

        public delegate bool FieldValidator<T>(T value, out string errorMessage);

        private void ValidateAndSetField<T>(ref T fieldToSet, T value, FieldValidator<T> fieldValidator) {
            if (fieldValidator(value, out string errorMessage)) {
                fieldToSet = value;
            } else {
                throw new ArgumentException(errorMessage);
            }
        }

        private enum FieldKind { FirstName, LastName, Nickname, Mailer, Note };

        private static bool IsFieldValid(FieldKind fieldKind, string value, out string errorMessage) {
            if (value.Length == 0) {
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
                default: maxFieldLength = short.MaxValue; break;
            }

            if (value.Length > maxFieldLength) {
                errorMessage = $"{fieldKind} is too long";
                return false;
            } else {
                errorMessage = null;
                return true;
            }
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
            set { ValidateAndSetField(ref firstName, value, IsFirstNameValid); }
        }

        private string lastName;
        public string LastName {
            get => lastName;
            set { ValidateAndSetField(ref lastName, value, IsLastNameValid); }
        }

        private string nickname;
        public string Nickname {
            get => nickname;
            set { ValidateAndSetField(ref nickname, value, IsNicknameValid); }
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
            foreach (Char c in Regex.Replace(value, @"[\-+()\s]", "")) {
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
            set { ValidateAndSetField(ref phone, value, IsPhoneValid); }
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
            set { ValidateAndSetField(ref email, value, IsEmailValid); }
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
            set { ValidateAndSetField(ref mailer, value, IsMailerValid); }
        }

        private string note;
        public string Note {
            get => mailer;
            set { ValidateAndSetField(ref note, value, IsNoteValid); }
        }

        public DateTime Birthday { get; set; }

        public Contact(string firstName = null, string lastName = null, string phone = null, string email = null) {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }

        public override string ToString() {
            return $"{FirstName} {LastName}, tel: {Phone}, email: {Email}";
        }

        private string GetBirthdayStringForVCard() {
            return " [[ STUB ]] ";
            // throw new NotImplementedException();
            // TODO
        }

        public string ToVCard() {
            return 
                $@"BEGIN:VCARD
                VERSION:4.0
                FN:{LastName + " " + FirstName}
                N:{LastName};{FirstName};;;
                NICKNAME:{Nickname}
                BDAY:{GetBirthdayStringForVCard()}
                TEL:{Phone}
                EMAIL:{Email}
                MAILER:{Mailer}
                NOTE:{Note}
                END:VCARD
                ";
        }
    }
}