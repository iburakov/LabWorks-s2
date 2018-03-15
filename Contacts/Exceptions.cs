using System;

namespace Contacts {

    public class UserRefusedException : Exception {
        public UserRefusedException() { }
        public UserRefusedException(string message) : base(message) { }
        public UserRefusedException(string message, Exception inner) : base(message, inner) { }
    }

}