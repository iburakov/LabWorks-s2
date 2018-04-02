using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Contacts {
    public class Contact {
        private const short MaxFirstNameLength = 16;
        private const short MaxLastNameLength = 36;
        private const short MinPhoneLength = 3;

        public delegate bool FieldValidator(string value, out string errorMessage);

        private enum ContactNameKind { First, Last };

        private static bool IsNameValid(ContactNameKind nameKind, string value, out string errorMessage) {
            if (value.Length == 0) {
                errorMessage = $"{nameKind} name can't be empty";
                return false;
            }

            if (value.Length > ((nameKind == ContactNameKind.First) ? MaxFirstNameLength : MaxLastNameLength)) {
                errorMessage = $"{nameKind} name is too long";
                return false;
            } else {
                errorMessage = null;
                return true;
            }
        }

        public static bool IsFirstNameValid(string value, out string errorMessage) {
            return IsNameValid(ContactNameKind.First, value, out errorMessage);
        }

        public static bool IsLastNameValid(string value, out string errorMessage) {
            return IsNameValid(ContactNameKind.Last, value, out errorMessage);
        }

        private string firstName;
        public string FirstName {
            get => firstName;
            set {
                if (IsFirstNameValid(value, out string errorMessage)) {
                    firstName = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        private string lastName;
        public string LastName {
            get => lastName;
            set {
                if (IsLastNameValid(value, out string errorMessage)) {
                    lastName = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public static string NormalizePhone(string phone) {
            return Regex.Replace(phone, @"[\-() ]", ""); ;
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
            set {
                if (IsPhoneValid(value, out string errorMessage)) {
                    phone = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
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
                if (IsEmailValid(value, out string errorMessage)) {
                    email = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public Contact(string firstName = null, string lastName = null, string phone = null, string email = null) {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }

        public override string ToString() {
            return $"{FirstName} {LastName}, tel: {Phone}, email: {Email}";
        }
    }
}