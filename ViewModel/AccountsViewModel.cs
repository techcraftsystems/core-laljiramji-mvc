using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class UsersViewModel {
        public List<Users> Users { get; set; }
        public Users User { get; set; }
        public string Password { get; set; }
        public string Confirm { get; set; }
        public string Message { get; set; }

        public UsersViewModel() {
            Users = new List<Users>();
            User = new Users();
            Password = "";
            Confirm = "";
            Message = "";
        }
    }
}
