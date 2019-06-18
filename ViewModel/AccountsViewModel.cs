using System;
using Core.Models;

namespace Core.ViewModel
{
    public class UsersViewModel {
        public Users User { get; set; }
        public string Password { get; set; }
        public string Confirm { get; set; }
        public string Message { get; set; }

        public UsersViewModel() {
            User = new Users();
            Password = "";
            Confirm = "";
            Message = "";
        }
    }
}
