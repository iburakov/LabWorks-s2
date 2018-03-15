using System;
using System.Text.RegularExpressions;

namespace Contacts {
    public class Contact {
        private static Int16 MaxFirstNameLength = 16;
        private static Int16 MaxLastNameLength = 36;
        private static Int16 MinPhoneLength = 3;

        public delegate Boolean FieldValidator(String value, out String errorMessage);

        private static Boolean NameValid(String nameKind, String value, out String errorMessage) {
            if (value.Length == 0) {
                errorMessage = nameKind + " name can't be empty";
                return false;
            }

            if (value.Length > ((nameKind == "First") ? MaxFirstNameLength : MaxLastNameLength)) {
                errorMessage = nameKind + " name is too long";
                return false;
            } else {
                errorMessage = null;
                return true;
            }
        }

        public static Boolean FirstNameValid(String value, out String errorMessage) {
            return NameValid("First", value, out errorMessage);
        }

        public static Boolean LastNameValid(String value, out String errorMessage) {
            return NameValid("Last", value, out errorMessage);
        }

        private String firstName;
        public String FirstName {
            get => firstName;
            set {
                if (FirstNameValid(value, out String errorMessage)) {
                    firstName = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        private String lastName;
        public String LastName {
            get => lastName;
            set {
                if (LastNameValid(value, out String errorMessage)) {
                    lastName = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public static String NormalizePhone(String phone) {
            return Regex.Replace(phone, @"[\-() ]", ""); ;
        }

        public static Boolean PhoneValid(String value, out String errorMessage) {
            if (value.Length < MinPhoneLength) {
                errorMessage = "Phone is too short";
                return false;
            }
            
            Boolean allCharsAreDigits = true;
            foreach (Char c in Regex.Replace(value, @"[\-+() ]", "").ToCharArray()) {
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

        private String phone;
        public String NormalizedPhone{ get => NormalizePhone(phone); }
        public String Phone {
            get => phone;
            set {
                if (PhoneValid(value, out String errorMessage)) {
                    phone = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public static Boolean EmailValid(String value, out String errorMessage) {
            try {
                var parsed = new System.Net.Mail.MailAddress(value);
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

        private String email;
        public String Email {
            get => email;
            set {
                if (EmailValid(value, out String errorMessage)) {
                    email = value;
                } else {
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public Contact(String firstName = null, String lastName = null, String phone = null, String email = null) {
            if (firstName != null) {
                FirstName = firstName;
            }

            if (lastName != null) {
                LastName = lastName;
            }

            if (phone != null) {
                Phone = phone;
            }

            if (email != null) {
                Email = email;
            }
        }

        public override String ToString() {
            return String.Format("{0} {1}, tel: {2}, email: {3}", FirstName, LastName, Phone, Email);
        }
    }
}