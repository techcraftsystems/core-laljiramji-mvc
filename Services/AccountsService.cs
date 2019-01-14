using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Extensions;
using Core.Models;
using Core.ViewModel;

namespace Core.Services
{
    public class AccountsService
    {
        public Bank GetBank(long idnt) {
            Bank bank = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bk_idnt, bk_code, bk_bank, bk_branch, bk_name, bk_number, bk_notes, ISNULL(bs_count,0) bk_count FROM BankAccounts LEFT OUTER JOIN (SELECT bs_bank, COUNT(bs_stidnt) bs_count FROM BankStations GROUP BY bs_bank) As Foo ON bk_idnt=bs_bank WHERE bk_idnt=" + idnt);
            if (dr.Read()) {
                bank = new Bank {
                    Id = Convert.ToInt64(dr[0]),
                    Code = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Branch = dr[3].ToString(),
                    AccountName = dr[4].ToString(),
                    AccountNumber = dr[5].ToString(),
                    Description = dr[6].ToString(),
                    StationCount = Convert.ToInt32(dr[7]),
                };
            }

            return bank;
        }

        public Bank GetBank(string code) {
            Bank bank = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bk_idnt, bk_code, bk_bank, bk_branch, bk_name, bk_number, bk_notes, ISNULL(bs_count,0) bk_count FROM BankAccounts LEFT OUTER JOIN (SELECT bs_bank, COUNT(bs_stidnt) bs_count FROM BankStations GROUP BY bs_bank) As Foo ON bk_idnt=bs_bank WHERE bk_code='" + code + "'");
            if (dr.Read()) {
                bank = new Bank {
                    Id = Convert.ToInt64(dr[0]),
                    Code = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Branch = dr[3].ToString(),
                    AccountName = dr[4].ToString(),
                    AccountNumber = dr[5].ToString(),
                    Description = dr[6].ToString(),
                    StationCount = Convert.ToInt32(dr[7]),
                };
            }

            return bank;
        }

        public List<Bank> GetBanks() {
            List<Bank> banks = new List<Bank>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bk_idnt, bk_code, bk_bank, bk_branch, bk_name, bk_number, bk_notes, ISNULL(bs_count,0) bk_count FROM BankAccounts LEFT OUTER JOIN (SELECT bs_bank, COUNT(bs_stidnt) bs_count FROM BankStations GROUP BY bs_bank) As Foo ON bk_idnt=bs_bank ORDER BY bk_bank");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Bank bank = new Bank {
                        Id = Convert.ToInt64(dr[0]),
                        Code = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Branch = dr[3].ToString(),
                        AccountName = dr[4].ToString(),
                        AccountNumber = dr[5].ToString(),
                        Description = dr[6].ToString(),
                        StationCount = Convert.ToInt32(dr[7]),
                    };

                    banks.Add(bank);
                }
            }

            return banks;
        }

        public List<BankingReconcileModel> GetBankingReconciles(DateTime start, DateTime stop, string accounts, string filter)
        {
            List<BankingReconcileModel> model = new List<BankingReconcileModel>();
            string q = "";
            double cumm = 0;

            if (!string.IsNullOrEmpty(accounts)) {
                q = " AND ar_st IN (SELECT bs_stidnt FROM BankStations WHERE bs_bank IN (" + accounts + "))";
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date1 DATE='" + start.Date + "'; SELECT ISNULL(SUM(CASE brs_action WHEN 1 THEN ar_amount ELSE 0-ar_amount END),0) FROM vBankReconcile INNER JOIN BankReconcileSources ON ar_source=brs_idnt INNER JOIN Stations ON ar_st=st_idnt WHERE ar_date<@date1" + q);
            if (dr.Read() && Convert.ToDouble(dr[0])>0) {
                BankingReconcileModel entry = new BankingReconcileModel {
                    Date = start.ToString("dd-MMM"),
                    Cummulative = Convert.ToDouble(dr[0]),
                    Description = "BALANCE B/F"
                };

                cumm = entry.Cummulative;
                model.Add(entry);
            }

            q = conn.GetQueryString(filter, "ar_cust+'-'+ar_chqs+'-'+ar_invs+'-'+CAST(ar_amount AS NVARCHAR)+'-'+brs_source+'-'+st_synonym", "ar_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (!string.IsNullOrEmpty(accounts)) {
                q += " AND ar_st IN (SELECT bs_stidnt FROM BankStations WHERE bs_bank IN (" + accounts + "))";
            }

            conn = new SqlServerConnection();
            dr = conn.SqlServerConnect("SELECT ar_st, ar_source, brs_action, ar_date, ar_qty, ar_price, ar_cust, ar_chqs, ar_invs, brs_source+' '+st_synonym ar_desc, ar_amount FROM vBankReconcile INNER JOIN BankReconcileSources ON ar_source=brs_idnt INNER JOIN Stations ON ar_st=st_idnt " + q + " ORDER BY ar_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    BankingReconcileModel entry = new BankingReconcileModel {
                        Station = Convert.ToInt32(dr[0]),
                        Source = Convert.ToInt32(dr[1]),
                        Action = Convert.ToInt32(dr[2]),
                        Date = Convert.ToDateTime(dr[3]).ToString("dd-MMM"),

                        Quantity = Convert.ToDouble(dr[4]),
                        Price = Convert.ToDouble(dr[5]),

                        Customer = dr[6].ToString(),
                        Cheque = dr[7].ToString(),
                        Invoice = dr[8].ToString(),
                        Description = dr[9].ToString()
                    };

                    if (entry.Action.Equals(1)) {
                        entry.Revenue = Convert.ToDouble(dr[10]);
                    }
                    else {
                        entry.Expense = Convert.ToDouble(dr[10]);
                    }

                    cumm += entry.Revenue - entry.Expense;
                    entry.Cummulative = cumm;

                    model.Add(entry);
                }
            }

            return model;
        }
    }
}
