using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Models;
using Core.Extensions;
          
namespace Core.Services
{
    public class UserService {
        public Users GetUser(string username) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usr_idnt, usr_uuid, usr_name, usr_email, usr_description, log_enabled, log_tochange, log_core_access, log_admin_lvl, log_access_lvl, log_username, log_password, log_last_access FROM Users INNER JOIN [Login] ON usr_idnt=log_user WHERE log_username='" + username +"'");
            if (dr.Read()) {
                return new Users {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Email = dr[3].ToString(),
                    Notes = dr[4].ToString(),
                    Enabled = Convert.ToBoolean(dr[5]),
                    ToChange = Convert.ToBoolean(dr[6]),
                    CoreAccess = Convert.ToBoolean(dr[7]),
                    AdminLevel = Convert.ToInt64(dr[8]),
                    AccessLevel = dr[9].ToString(),
                    Username = dr[10].ToString(),
                    Password = dr[11].ToString(),
                    LastSeen = Convert.ToDateTime(dr[12])
                };
            }

            return null;
        }

        public Users GetUserByUuid(string uuid) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usr_idnt, usr_uuid, usr_name, usr_email, usr_description, log_enabled, log_tochange, log_core_access, log_admin_lvl, log_access_lvl, log_username, log_password, log_last_access FROM Users INNER JOIN [Login] ON usr_idnt=log_user WHERE usr_uuid COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '" + uuid + "'");
            if (dr.Read()) {
                return new Users {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Email = dr[3].ToString(),
                    Notes = dr[4].ToString(),
                    Enabled = Convert.ToBoolean(dr[5]),
                    ToChange = Convert.ToBoolean(dr[6]),
                    CoreAccess = Convert.ToBoolean(dr[7]),
                    AdminLevel = Convert.ToInt64(dr[8]),
                    AccessLevel = dr[9].ToString(),
                    Username = dr[10].ToString(),
                    Password = dr[11].ToString(),
                    LastSeen = Convert.ToDateTime(dr[12])
                };
            }

            return null;
        }

        public List<Users> GetUsers() {
            List<Users> users = new List<Users>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usr_idnt, usr_uuid, usr_name, usr_email, usr_description, log_enabled, log_tochange, log_core_access, log_admin_lvl, log_access_lvl, log_username, log_last_access FROM Users INNER JOIN [Login] ON usr_idnt=log_user ORDER BY usr_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    users.Add(new Users {
                        Id = Convert.ToInt64(dr[0]),
                        Uuid = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Email = dr[3].ToString(),
                        Notes = dr[4].ToString(),
                        Enabled = Convert.ToBoolean(dr[5]),
                        ToChange = Convert.ToBoolean(dr[6]),
                        CoreAccess = Convert.ToBoolean(dr[7]),
                        AdminLevel = Convert.ToInt64(dr[8]),
                        AccessLevel = dr[9].ToString(),
                        Username = dr[10].ToString(),
                        LastSeen = Convert.ToDateTime(dr[11])
                    });
                }
            }

            return users;
        }

        public List<UsersRoles> GetRoles(Users user) {
            List<UsersRoles> roles = new List<UsersRoles>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usrl_idnt, rl_idnt, rl_name FROM UsersRole INNER JOIN Roles ON usrl_role=rl_idnt WHERE usrl_user=" + user.Id + " ORDER BY rl_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    roles.Add(new UsersRoles { 
                        User = user,
                        Id = Convert.ToInt64(dr[0]),
                        Role = new Roles {
                            Id = Convert.ToInt64(dr[1]),
                            Name = dr[2].ToString()
                        }
                    });
                }
            }

            return roles;
        }


        /*Data Writers*/
        public void UpdateLastAccess(Users user) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_last_access=GETDATE() WHERE log_user=" + user.Id);
        }

        public void UpdatePassword(Users user, string password) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_password='" + password + "', log_tochange=0, log_last_change=GETDATE() WHERE log_user=" + user.Id);
        }

        public void ResetPassword(Users user) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_password='" + new CrytoUtilsExtensions().Encrypt("pass") + "', log_tochange=1 WHERE log_user=" + user.Id);
        }

        public void EnableAccount(Users user, bool opts = true) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_enabled='" + opts + "' WHERE log_user=" + user.Id);
        }
    }
}
