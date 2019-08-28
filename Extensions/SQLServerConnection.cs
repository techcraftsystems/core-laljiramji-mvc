﻿using System;
using System.Data.SqlClient;

namespace Core.Extensions
{
    public class SqlServerConnection {
        public string Server { get; set; }
        private string SConn { get; set; }
        private SqlConnection Conn { get; set; }
        protected SqlCommand comm = new SqlCommand();

        public SqlServerConnection() {
            Server = "core_system";
            SConn = "Data Source=192.168.1.11;Initial Catalog=" + Server + ";User ID=ct;Password=ct-2011;Max Pool Size=200;";
            Conn = new SqlConnection(SConn);
        }

        public SqlDataReader SqlServerConnect(string sqlstring) {
            try {
                Conn.Open();
                comm = new SqlCommand(sqlstring, Conn);

                return comm.ExecuteReader();
            }
            catch (Exception){
                return null;
            }
        }

        public Int64 SqlServerUpdate(string SqlString) {
            try {
                SqlCommand command = new SqlCommand(SqlString, Conn);
                command.Connection.Open();

                if (SqlString.ToLower().Contains("output"))
                    return Convert.ToInt64(command.ExecuteScalar());
                else {
                    command.ExecuteNonQuery();
                    return 0;
                }
            }
            catch (Exception) {
                return 0;
            }
            finally {
                if (Conn.State == System.Data.ConnectionState.Open)
                    Conn.Close();
            }
        }

        public string GetQueryString(string filter, string command, string sAdditionalString = "", bool AndJoin = true) {
            string query = "";
            string JOIN = " AND ";

            if (!AndJoin)
                JOIN = " OR ";

            char[] Seps = new [] { '.', ' ', '*', '-', '+', '&', '%', '/', '$', '#' };
            string[] MyInfo = filter.Split(Seps, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= (MyInfo.Length - 1); i++)
            {
                if (JOIN.Trim() == "OR" & !(MyInfo[i].Length > 1))
                    continue;

                if (query.Trim() == "")
                    query = " WHERE (" + command + " LIKE '%" + GetValidSqlString(MyInfo[i]) + "%'";
                else
                    query += JOIN + command + " LIKE '%" + GetValidSqlString(MyInfo[i]) + "%'";
            }

            if (query != "")
                query += ")";

            if (sAdditionalString != "") 
            {
                if (query == "")
                    query = " WHERE " + sAdditionalString;
                else
                    query += " AND " + sAdditionalString;
            }

            return query;
        }

        private string GetValidSqlString(String query){
            return query.Replace("'", "''");
        }

    }
}
