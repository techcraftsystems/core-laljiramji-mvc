using System;
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

        public List<SelectListItem> GetStationsIEnumerable(bool includeOthers = false) {
            List<SelectListItem> stations = GetIEnumerable("SELECT st_idnt, st_name FROM Stations ORDER BY st_order");
            if (includeOthers) {
                stations.Insert(0, new SelectListItem {
                    Value = "0",
                    Text = "Mixed",
                });
            }

            return stations;
        }

        public List<SelectListItem> GetTrucksIEnumerable() {
            return GetIEnumerable("SELECT tr_idnt, tr_registration FROM Trucks ORDER BY tr_registration");
        }

        public List<SelectListItem> GetSuppliersIEnumerable() {
            return GetIEnumerable("SELECT sp_idnt, sp_name FROM Suppliers ORDER BY sp_idnt");
        }

        public List<SelectListItem> GetExpenseCategoriesIEnumerable() {
            return GetIEnumerable("SELECT ec_idnt, ec_category FROM ExpensesCategory ORDER BY ec_category");
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
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_source, tf_date, tr_registration, sp_name, tf_dev, tf_invoice, ISNULL(tf_station, xs_source), tf_qnty, tf_price, tf_amount FROM vExpensesCore INNER JOIN ExpensesSource ON tf_source=xs_idnt " + conn.GetQueryString(filter, "tr_registration+'-'+sp_name+'-'+tf_dev+'-'+tf_invoice+'-'+ISNULL(tf_station,xs_source)+'-'+CAST(tf_qnty AS NVARCHAR)+'-'+CAST(tf_price AS NVARCHAR)+'-'+CAST(tf_amount  AS NVARCHAR)", "tf_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + " ORDER BY tf_date, tf_source, tf_idnt");
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

        public TrucksFuelExpense GetTrucksFuelExpense(int idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_date, tf_truck, tr_registration, tf_supplier, sp_name, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated FROM TrucksFuel INNER JOIN Trucks ON tr_idnt=tf_truck INNER JOIN Suppliers ON sp_idnt=tf_supplier WHERE tf_idnt=" + idnt);
            if (dr.Read())
            {
                TrucksFuelExpense xp = new TrucksFuelExpense
                {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    Truck = new Trucks(Convert.ToInt64(dr[2]), dr[3].ToString()),
                    Supplier = new Suppliers(Convert.ToInt64(dr[4]), dr[5].ToString()),
                    Invoice = dr[6].ToString(),
                    Quantity = Convert.ToDouble(dr[7]),
                    Price = Convert.ToDouble(dr[8]),
                    Amount = Convert.ToDouble(dr[9]),
                    VatAmount = Convert.ToDouble(dr[10])
                };
                
                xp.Exclussive = (xp.VatAmount / 0.08);
                xp.Zerorated = xp.Amount - xp.VatAmount - xp.Exclussive;
                
                return xp;
            }

            return new TrucksFuelExpense();
        }

        public List<TrucksFuelExpense> GetTrucksFuelExpense(int month, int year)
        {
            List<TrucksFuelExpense> report = new List<TrucksFuelExpense>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_date, tf_truck, tr_registration, tf_supplier, sp_name, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated FROM TrucksFuel INNER JOIN Trucks ON tr_idnt=tf_truck INNER JOIN Suppliers ON sp_idnt=tf_supplier WHERE MONTH(tf_date)=" + month + " AND YEAR(tf_date)=" + year + " ORDER BY tf_date, tf_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    TrucksFuelExpense xp = new TrucksFuelExpense {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        Truck = new Trucks(Convert.ToInt64(dr[2]), dr[3].ToString()),
                        Supplier = new Suppliers(Convert.ToInt64(dr[4]), dr[5].ToString()),
                        Invoice = dr[6].ToString(),
                        Quantity = Convert.ToDouble(dr[7]),
                        Price = Convert.ToDouble(dr[8]),
                        Amount = Convert.ToDouble(dr[9]),
                        VatAmount = Convert.ToDouble(dr[10])
                    };
					
					xp.Exclussive = (xp.VatAmount / 0.08);
					xp.Zerorated = xp.Amount - xp.VatAmount - xp.Exclussive;
					
					report.Add(xp);
                }
            }

            return report;
        }

        public StationsExpenses GetStationsExpenses(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT xp_idnt, xp_date, xp_invoice, xp_amount, xp_vat_amts, xp_zero_rated, xp_description, xp_user, ec_idnt, ec_category, sp_idnt, sp_name, ISNULL(st_idnt,0)st_idnt, st_code, ISNULL(st_name,'MIXED')st_name FROM Expenses INNER JOIN ExpensesCategory ON xp_category=ec_idnt INNER JOIN Suppliers ON xp_supplier=sp_idnt LEFT OUTER JOIN Stations ON xp_station=st_idnt WHERE xp_idnt=" + idnt);
            if (dr.Read()) {
                return new StationsExpenses {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    DateString = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                    Invoice = dr[2].ToString(),
                    Amount = Convert.ToDouble(dr[3]),
                    VatAmount = Convert.ToDouble(dr[4]),
                    Zerorated = Convert.ToDouble(dr[5]),
                    Description = dr[6].ToString().ToUpper(),
                    User = new Users(dr[7].ToString()),
                    Category = new ExpensesCategory(Convert.ToInt64(dr[8]), dr[9].ToString()),
                    Supplier = new Suppliers(Convert.ToInt64(dr[10]), dr[11].ToString().ToUpper()),
                    Station = new Stations(Convert.ToInt64(dr[12]), dr[13].ToString(), dr[14].ToString().ToUpper()),
                };
            }

            return null;
        }

        public List<StationsExpenses> GetStationsExpenses(DateTime start, DateTime stop, string stations = "", string filter = "") {
            List<StationsExpenses> expenses = new List<StationsExpenses>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "xp_invoice+'-'+CAST(xp_amount AS NVARCHAR)+'-'+xp_description+'-'+ec_category+'-'+sp_name+'-'+ISNULL(st_name,'MIXED')", "xp_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (!string.IsNullOrEmpty(stations))
                filterString += " AND xp_station IN (" + stations + ")";

            SqlDataReader dr = conn.SqlServerConnect("SELECT xp_idnt, xp_date, xp_invoice, xp_amount, xp_vat_amts, xp_zero_rated, xp_description, xp_user, ec_idnt, ec_category, sp_idnt, sp_name, ISNULL(st_idnt,0)st_idnt, st_code, ISNULL(st_name,'MIXED')st_name FROM Expenses INNER JOIN ExpensesCategory ON xp_category=ec_idnt INNER JOIN Suppliers ON xp_supplier=sp_idnt LEFT OUTER JOIN Stations ON xp_station=st_idnt " + filterString + " ORDER BY xp_date, xp_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    expenses.Add(new StationsExpenses {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        DateString = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                        Invoice = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        VatAmount = Convert.ToDouble(dr[4]),
                        Zerorated = Convert.ToDouble(dr[5]),
                        Description = dr[6].ToString().ToUpper(),
                        User = new Users(dr[7].ToString()),
                        Category = new ExpensesCategory(Convert.ToInt64(dr[8]), dr[9].ToString()),
                        Supplier = new Suppliers(Convert.ToInt64(dr[10]), dr[11].ToString().ToUpper()),
                        Station = new Stations(Convert.ToInt64(dr[12]), dr[13].ToString(), dr[14].ToString().ToUpper()),
                    });
                }
            }

            return expenses;
        }

        public List<FuelPriceChange> GetLastPriceChange() {
            List<FuelPriceChange> change = new List<FuelPriceChange>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT TOP(4) vat_date, vat_rate, vat_zero, vat_trucks, fl_idnt, UPPER(fl_name) fl_item FROM VatReports INNER JOIN Fuel ON vat_item=fl_idnt ORDER BY vat_date DESC, vat_item");
            if (dr.HasRows) {
                while (dr.Read()) {
                    change.Add(new FuelPriceChange {
                        Date = Convert.ToDateTime(dr[0]),
                        Taxx = Convert.ToDouble(dr[1]),
                        Zero = Convert.ToDouble(dr[2]),
                        Trucks = Convert.ToDouble(dr[3]),
                        Fuel = new Fuel { 
                            Id = Convert.ToInt64(dr[4]),
                            Name = dr[5].ToString() 
                        }
                    });
                }
            }

            return change;
        }


        //::Data Writters
        public FuelPriceChange SavePriceChange(FuelPriceChange change) {
            SqlServerConnection conn = new SqlServerConnection();
            change.Id = conn.SqlServerUpdate("DECLARE @date DATE='" + change.Date + "', @item INT='" + change.Fuel.Id + "', @trck FLOAT=" + change.Trucks + ", @rate FLOAT=" + change.Taxx + ", @zero FLOAT=" + change.Zero + "; IF NOT EXISTS (SELECT vat_idnt FROM VatReports WHERE vat_date=@date AND vat_item=@item) BEGIN INSERT INTO VatReports (vat_date, vat_item, vat_rate, vat_zero, vat_trucks) output INSERTED.vat_idnt VALUES (@date, @item, @rate, @zero, @trck) END ELSE BEGIN UPDATE VatReports SET vat_rate=@rate, vat_zero=@zero, vat_trucks=@trck output INSERTED.vat_idnt WHERE vat_date=@date AND vat_item=@item END");

            return change;
        }

        public TrucksFuelExpense SaveTrucksFuelExpense(TrucksFuelExpense expense) {
            SqlServerConnection conn = new SqlServerConnection();
            expense.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + expense.Id+ ", @date DATE='" + expense.Date + "', @trck INT=" + expense.Truck.Id + ", @supp INT=" + expense.Supplier.Id + ", @invs NVARCHAR(MAX)='" + expense.Invoice + "', @qnty FLOAT=" + expense.Quantity + ", @prce FLOAT=" + expense.Price + ", @amts FLOAT=" + expense.Amount + ", @vats FLOAT=" + expense.VatAmount + ", @zero FLOAT=" + expense.Zerorated + ", @user NVARCHAR(50)='" + Username + "', @desc NVARCHAR(MAX)='" + expense.Description + "'; IF NOT EXISTS (SELECT tf_idnt FROM TrucksFuel WHERE tf_idnt=@idnt) BEGIN INSERT INTO TrucksFuel (tf_date, tf_truck, tf_supplier, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated, tf_user, tf_description) output INSERTED.tf_idnt VALUES (@date, @trck, @supp, @invs, @qnty, @prce, @amts, @vats, @zero, @user, @desc) END ELSE BEGIN UPDATE TrucksFuel SET tf_date=@date, tf_truck=@trck, tf_supplier=@supp, tf_invoice=@invs, tf_qnty=@qnty, tf_price=@prce, tf_amount=@amts, tf_vatamts=@vats, tf_zerorated=@zero, tf_description=@desc output INSERTED.tf_idnt WHERE tf_idnt=@idnt END");

            return expense;
        }

        public StationsExpenses SaveStationsExpenses(StationsExpenses expense)
        {
            SqlServerConnection conn = new SqlServerConnection();
            expense.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + expense.Id + ", @date DATE='" + expense.Date + "', @catg INT=" + expense.Category.Id + ", @supp INT=" + expense.Supplier.Id + ", @stns INT=" + expense.Station.Id + ",  @invs NVARCHAR(MAX)='" + expense.Invoice + "', @amts FLOAT=" + expense.Amount + ", @vats FLOAT=" + expense.VatAmount + ", @zero FLOAT=" + expense.Zerorated + ", @user NVARCHAR(50)='" + Username + "', @desc NVARCHAR(MAX)='" + expense.Description + "'; IF NOT EXISTS (SELECT xp_idnt FROM Expenses WHERE xp_idnt=@idnt) BEGIN INSERT INTO Expenses (xp_date, xp_invoice, xp_category, xp_station, xp_supplier, xp_amount, xp_vat_amts, xp_zero_rated, xp_description, xp_user) output INSERTED.xp_idnt VALUES (@date, @invs, @catg, @stns, @supp, @amts, @vats, @zero, @desc, @user) END ELSE BEGIN UPDATE Expenses SET xp_date=@date, xp_invoice=@invs, xp_category=@catg, xp_station=@stns, xp_supplier=@supp, xp_amount=@amts, xp_vat_amts=@vats, xp_zero_rated=@zero, xp_description=@desc output INSERTED.xp_idnt WHERE xp_idnt=@idnt END");

            return expense;
        }
    }
}
