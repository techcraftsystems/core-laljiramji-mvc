using System;
using Core.Models;

namespace Core.ViewModel
{
    public class LoginModel
    {
        public Users User { get; set; }
        public String Message { get; set; }

        public LoginModel()
        {
            User = new Users();
            Message = "";
        }
    }
}
