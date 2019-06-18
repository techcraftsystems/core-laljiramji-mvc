using System;
using System.Collections.Generic;
using Core.Services;

namespace Core.Models
{
    public class Users {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public bool ToChange { get; set; }
        public bool CoreAccess { get; set; }
        public long AdminLevel { get; set; }
        public string AccessLevel { get; set; }
        public string Notes { get; set; }
        public string Message { get; set; }
        public DateTime LastSeen { get; set; }

        public Users() {
            Id = 0;
            Uuid = "";
            Name = "";
            Email = "";
            Notes = "";
            Username = "";
            Password = "";
            Enabled = true;
            ToChange = false;
            AdminLevel = 0;
            AccessLevel = "";
            Message = "";
            LastSeen = new DateTime(1990, 1, 1);
        }

        public Users(string username) : this() {
            Username = username;
        }

        public List<UsersRoles> GetRoles() {
            return new UserService().GetRoles(this);
        }

        public void UpdateLastAccess() {
            new UserService().UpdateLastAccess(this);
        }

        public void UpdatePassword(string password) {
            new UserService().UpdatePassword(this, password);
        }

        public void ResetPassword() {
            new UserService().ResetPassword(this);
        }

        public void EnableAccount(bool opts) {
            new UserService().EnableAccount(this, opts);
        }
    }

    public class UsersRoles {
        public long Id { get; set; }
        public Users User { get; set; }
        public Roles Role { get; set; }

        public UsersRoles() {
            Id = 0;
            User = new Users();
            Role = new Roles();
        }
    }
}
