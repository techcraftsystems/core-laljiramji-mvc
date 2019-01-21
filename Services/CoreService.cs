﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using Core.DataModel;
using Core.Extensions;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Services
{
    public class CoreService
    {
        private int Actor { get; set; }
        private string Username { get; set; }

        public CoreService() { }
        public CoreService(HttpContext context){
            Actor = int.Parse(context.User.FindFirst(ClaimTypes.Actor).Value);
            Username = context.User.FindFirst(ClaimTypes.UserData).Value;
        }

        public List<SelectListItem> GetIEnumerable(string query)
        {
            List<SelectListItem> enumarables = new List<SelectListItem>();
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect(query);
            if (dr.HasRows) {
                while (dr.Read()) {
                    SelectListItem option = new SelectListItem {
                        Value = dr[0].ToString(),
                        Text = dr[1].ToString()
                    };

                    enumarables.Add(option);
                }
            }

            return enumarables;
        }

        public Customers GetCustomer(String station, Int64 id){
            Customers customer = new Customers();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT Custid, Names, Contacts, Telephone, KRA_PIN, AccountBalance, CreditLimit, DateJoined, ISNULL(sr_last,'1990-01-01')x, st_idnt, st_code, CASE st_idnt WHEN 12 THEN 'Shell Uhuru Highway' ELSE st_name END st_names FROM vCustomers INNER JOIN Stations ON Sts=st_idnt LEFT OUTER JOIN vLastInvoices ON sr_st=Sts AND sr_cust=Custid WHERE st_code='" + station + "' AND Custid=" + id);
            if (dr.Read()) {
                customer.Id = Convert.ToInt64(dr[0]);
                customer.Name = dr[1].ToString();
                customer.Contacts = dr[2].ToString();
                customer.Telephone = dr[3].ToString();
                customer.KraPin = dr[4].ToString();

                customer.Balance = Convert.ToDouble(dr[5]);
                customer.CreditLimit = Convert.ToDouble(dr[6]);

                customer.DateJoined = Convert.ToDateTime(dr[7]);
                customer.LastInvoice = Convert.ToDateTime(dr[8]);

                customer.Station.Id = Convert.ToInt64(dr[9]);
                customer.Station.Code = dr[10].ToString();
                customer.Station.Name = dr[11].ToString();
            }

            return customer;
        }

        public List<Customers> GetCustomers(String stations){
            List<Customers> customers = new List<Customers>();
            String additionalquery = "";

            if (!string.IsNullOrEmpty(stations.Trim()))
            {
                additionalquery = "WHERE st_name IN ('" + stations + "')";
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT Custid, Names, Contacts, Telephone, KRA_PIN, AccountBalance, CreditLimit, DateJoined, ISNULL(sr_last,'1990-01-01') sr_last_invs, st_idnt, st_code, CASE st_idnt WHEN 12 THEN 'Shell Uhuru Highway' ELSE st_name END FROM vCustomers INNER JOIN Stations ON Sts=st_idnt LEFT OUTER JOIN vLastInvoices ON Sts=sr_st AND Custid=sr_cust " + additionalquery + " ORDER BY st_name, Names");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Customers customer = new Customers();
                    customer.Id = Convert.ToInt64(dr[0]);
                    customer.Name = dr[1].ToString();
                    customer.Contacts = dr[2].ToString();
                    customer.Telephone = dr[3].ToString();
                    customer.KraPin = dr[4].ToString();

                    customer.Balance = Convert.ToDouble(dr[5]);
                    customer.CreditLimit = Convert.ToDouble(dr[6]);

                    customer.DateJoined = Convert.ToDateTime(dr[7]);
                    customer.LastInvoice = Convert.ToDateTime(dr[8]);

                    customer.Station.Id = Convert.ToInt64(dr[9]);
                    customer.Station.Code = dr[10].ToString();
                    customer.Station.Name = dr[11].ToString();

                    customers.Add(customer);
                }
            }

            return customers;
        }

        public List<SelectListItem> GetTrucksIEnumerable() {
            return GetIEnumerable("SELECT tr_idnt, tr_registration FROM Trucks ORDER BY tr_registration");
        }

        public List<SelectListItem> GetSuppliersIEnumerable() {
            return GetIEnumerable("SELECT sp_idnt, sp_name FROM Suppliers ORDER BY sp_idnt");
        }

        public double GetLatestPurchasePrice() {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT TOP(1) vat_trucks FROM VatReports WHERE vat_item=1 ORDER BY vat_date DESC");
            if (dr.Read()) {
                return Convert.ToDouble(dr[0]);
            }

            return 0;
        }

        public VatResults GetVatResults(DateTime date, Fuel item) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT vat_idnt, vat_date, vat_item, vat_rate, vat_zero, vat_trucks FROM VatReports WHERE vat_item=" + item.Id + " AND vat_date='" + date.Date + "'");
            if (dr.Read()) {
                return new VatResults {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    Fuel = new Fuel(Convert.ToInt64(dr[2])),
                    Rate = Convert.ToDouble(dr[3]),
                    Zero = Convert.ToDouble(dr[4]),
                    Truck = Convert.ToDouble(dr[5])
                };
            }

            return null;
        }

        public List<ExpensesCore> GetExpensesCore(DateTime start, DateTime stop, string filter = "") {
            List<ExpensesCore> expenses = new List<ExpensesCore>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_source, tf_date, tr_registration, sp_name, tf_dev, tf_invoice, xs_source, tf_qnty, tf_price, tf_amount FROM vExpensesCore INNER JOIN ExpensesSource ON tf_source=xs_idnt " + conn.GetQueryString(filter, "tr_registration+'-'+sp_name+'-'+tf_dev+'-'+tf_invoice+'-'+xs_source+'-'+CAST(tf_qnty AS NVARCHAR)+'-'+CAST(tf_price AS NVARCHAR)+'-'+CAST(tf_amount  AS NVARCHAR)", "tf_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + " ORDER BY tf_date, tf_source, tf_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    expenses.Add(new ExpensesCore {
                        Id = Convert.ToInt64(dr[0]),
                        Source = Convert.ToInt32(dr[1]),
                        Date = Convert.ToDateTime(dr[2]).ToString("dd/MM/yyyy"),
                        Account = dr[3].ToString(),
                        Details = dr[4].ToString().ToUpper(),
                        Delivery = dr[5].ToString(),
                        Invoice = dr[6].ToString(),
                        Description = dr[7].ToString().ToUpper(),
                        Quantity = Convert.ToInt32(dr[8]),
                        Price = Convert.ToInt32(dr[9]),
                        Amount = Convert.ToInt32(dr[10]),
                    });
                }
            }

            return expenses;
        }

        public List<TrucksMonthlySummary> GetTrucksMonthlySummary(int year)
        {
            List<TrucksMonthlySummary> report = new List<TrucksMonthlySummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tr_idnt, tr_registration, ISNULL(xJan,0)xJan, ISNULL(xFeb,0)xFeb, ISNULL(xMar,0)xMar, ISNULL(xApr,0)xApr, ISNULL(xMay,0)xMay, ISNULL(xJun,0)xJun, ISNULL(xJul,0)xJul, ISNULL(xAug,0)xAug, ISNULL(xSep,0)xSep, ISNULL(xOct,0)xOct, ISNULL(xNov,0)xNov, ISNULL(xDec,0)xDec, ISNULL(xTotal,0)xTotal FROM Trucks LEFT OUTER JOIN ( SELECT tf_truck, SUM(CASE MONTH(tf_date)WHEN 1 THEN tf_qnty ELSE 0 END) xJan, SUM(CASE MONTH(tf_date)WHEN 2 THEN tf_qnty ELSE 0 END) xFeb, SUM(CASE MONTH(tf_date)WHEN 3 THEN tf_qnty ELSE 0 END) xMar, SUM(CASE MONTH(tf_date)WHEN 4 THEN tf_qnty ELSE 0 END) xApr, SUM(CASE MONTH(tf_date)WHEN 5 THEN tf_qnty ELSE 0 END) xMay, SUM(CASE MONTH(tf_date)WHEN 6 THEN tf_qnty ELSE 0 END) xJun, SUM(CASE MONTH(tf_date)WHEN 7 THEN tf_qnty ELSE 0 END) xJul, SUM(CASE MONTH(tf_date)WHEN 8 THEN tf_qnty ELSE 0 END) xAug, SUM(CASE MONTH(tf_date)WHEN 9 THEN tf_qnty ELSE 0 END) xSep, SUM(CASE MONTH(tf_date)WHEN 10 THEN tf_qnty ELSE 0 END) xOct, SUM(CASE MONTH(tf_date)WHEN 11 THEN tf_qnty ELSE 0 END) xNov, SUM(CASE MONTH(tf_date)WHEN 12 THEN tf_qnty ELSE 0 END) xDec, SUM(tf_qnty) xTotal FROM TrucksFuel WHERE YEAR(tf_date)=" + year + " GROUP BY tf_truck) As Foo On tf_truck=tr_idnt ORDER BY tr_registration");
            if (dr.HasRows) {
                while (dr.Read()) {
                    report.Add(new TrucksMonthlySummary { 
                        Truck = new Trucks(Convert.ToInt64(dr[0]), dr[1].ToString()),
                        Jan = Convert.ToDouble(dr[2]),
                        Feb = Convert.ToDouble(dr[3]),
                        Mar = Convert.ToDouble(dr[4]),
                        Apr = Convert.ToDouble(dr[5]),
                        May = Convert.ToDouble(dr[6]),
                        Jun = Convert.ToDouble(dr[7]),
                        Jul = Convert.ToDouble(dr[8]),
                        Aug = Convert.ToDouble(dr[9]),
                        Sep = Convert.ToDouble(dr[10]),
                        Oct = Convert.ToDouble(dr[11]),
                        Nov = Convert.ToDouble(dr[12]),
                        Dec = Convert.ToDouble(dr[13]),
                        Total = Convert.ToDouble(dr[14])
                    });
                }
            }

            return report;
        }


        //::Data Writters
        public TrucksFuelExpense SaveTrucksFuelExpense(TrucksFuelExpense expense) {
            SqlServerConnection conn = new SqlServerConnection();
            expense.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + expense.Id+ ", @date DATE='" + expense.Date + "', @trck INT=" + expense.Truck.Id + ", @supp INT=" + expense.Supplier.Id + ", @invs NVARCHAR(MAX)='" + expense.Invoice + "', @qnty FLOAT=" + expense.Quantity + ", @prce FLOAT=" + expense.Price + ", @amts FLOAT=" + expense.Amount + ", @vats FLOAT=" + expense.VatAmount + ", @zero FLOAT=" + expense.Zerorated + ", @user NVARCHAR(50)='" + Username + "', @desc NVARCHAR(MAX)='" + expense.Description + "'; IF NOT EXISTS (SELECT tf_idnt FROM TrucksFuel WHERE tf_idnt=@idnt) BEGIN INSERT INTO TrucksFuel (tf_date, tf_truck, tf_supplier, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated, tf_user, tf_description) output INSERTED.tf_idnt VALUES (@date, @trck, @supp, @invs, @qnty, @prce, @amts, @vats, @zero, @user, @desc) END ELSE BEGIN UPDATE TrucksFuel SET tf_date=@date, tf_truck=@trck, tf_supplier=@supp, tf_invoice=@invs, tf_qnty=@qnty, tf_price=@prce, tf_amount=@amts, tf_vatamts=@vats, tf_zerorated=@zero, tf_description=@desc output INSERTED.tf_idnt WHERE tf_idnt=@idnt END");

            return expense;
        }
    }
}
