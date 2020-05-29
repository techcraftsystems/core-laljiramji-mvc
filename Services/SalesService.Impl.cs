using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Extensions;
using Core.Models;

namespace Core.Services
{
    public class SalesService : ISalesService
    {
        public List<PettyCash> GetPettyCash(DateTime from, DateTime to, string code)
        {
            List<PettyCash> pc = new List<PettyCash>();
            string q = "WHERE dlv_date BETWEEN '" + from + "' AND '" + to + "'";
            if (!string.IsNullOrEmpty(code) && !code.Equals("all"))
                q += " AND st_code LIKE '" + code + "'";

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pc_idnt, pc_voucher, pc_receipt, pc_account, pc_supplier, pc_description, pc_amount, dlv_idnt, dlv_date, dlv_rcpt, st_idnt, st_code, st_name FROM DeliveryPettyCash INNER JOIN Delivery ON pc_delv=dlv_idnt INNER JOIN Stations ON dlv_station=st_idnt " + q + " ORDER BY dlv_date, pc_idnt DESC");
            if (dr.HasRows) {
                while (dr.Read()) {
                    pc.Add(new PettyCash {
                        Id = Convert.ToInt64(dr[0]),
                        Voucher = dr[1].ToString(),
                        Receipt = dr[2].ToString(),
                        Account = dr[3].ToString(),
                        Supplier = dr[4].ToString(),
                        Description = dr[5].ToString(),
                        Amount = Convert.ToDouble(dr[6]),
                        Delivery = new Delivery {
                            Id = Convert.ToInt64(dr[7]),
                            Date = Convert.ToDateTime(dr[8]),
                            Receipt = dr[9].ToString(),
                            Station = new Stations {
                                Id = Convert.ToInt64(dr[10]),
                                Code = dr[11].ToString(),
                                Name = dr[12].ToString(),
                            }
                        }
                    });
                }
            }

            return pc;
        }
    }
}
