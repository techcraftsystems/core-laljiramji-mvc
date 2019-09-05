using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using Core.DataModel;
using Core.Extensions;
using Core.Models;
using Core.Objects;
using Core.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Core.Services {
    public class CoreService {
        private int Actor { get; set; }
        private string Username { get; set; }

        public CoreService() { }
        public CoreService(HttpContext context){
            Actor = int.Parse(context.User.FindFirst(ClaimTypes.Actor).Value);
            Username = context.User.FindFirst(ClaimTypes.UserData).Value;
        }

        public List<SelectListItem> GetIEnumerable(string query) {
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

        public Customers GetCustomer(string station, long id){
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT Custid, Names, Contacts, Telephone, KRA_PIN, AccountBalance, CreditLimit, DateJoined, ISNULL(sr_last,'1990-01-01')x, st_idnt, st_code, CASE st_idnt WHEN 12 THEN 'Shell Uhuru Highway' ELSE st_name END st_names, st_database FROM vCustomers INNER JOIN Stations ON Sts=st_idnt LEFT OUTER JOIN vLastInvoices ON sr_st=Sts AND sr_cust=Custid WHERE st_code='" + station + "' AND Custid=" + id);
            if (dr.Read()) {
                return new Customers {
                    Id = Convert.ToInt64(dr[0]),
                    Name = dr[1].ToString(),
                    Contacts = dr[2].ToString(),
                    Telephone = dr[3].ToString(),
                    KraPin = dr[4].ToString(),

                    Balance = Convert.ToDouble(dr[5]),
                    CreditLimit = Convert.ToDouble(dr[6]),

                    DateJoined = Convert.ToDateTime(dr[7]),
                    LastInvoice = Convert.ToDateTime(dr[8]),
                    Station = new Stations {
                        Id = Convert.ToInt64(dr[9]),
                        Code = dr[10].ToString(),
                        Name = dr[11].ToString(),
                        Prefix = dr[12].ToString()
                    }
                };
            }

            return null;
        }

        public List<Customers> GetCustomers(string stations = "", string codes = ""){
            List<Customers> customers = new List<Customers>();
            string additionalquery = "";

            if (!string.IsNullOrEmpty(stations.Trim())) {
                additionalquery = "WHERE st_name IN ('" + stations + "')";
            }

            if (!string.IsNullOrEmpty(codes.Trim())) {
                additionalquery = "WHERE st_code IN ('" + codes + "')";
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT Custid, Names, Contacts, Telephone, KRA_PIN, AccountBalance, CreditLimit, DateJoined, ISNULL(sr_last,'1990-01-01') sr_last_invs, st_idnt, st_code, CASE st_idnt WHEN 12 THEN 'Shell Uhuru Highway' ELSE st_name END FROM vCustomers INNER JOIN Stations ON Sts=st_idnt LEFT OUTER JOIN vLastInvoices ON Sts=sr_st AND Custid=sr_cust " + additionalquery + " ORDER BY st_name, Names");
            if (dr.HasRows) {
                while (dr.Read()) {
                    customers.Add(new Customers {
                        Id = Convert.ToInt64(dr[0]),
                        Name = dr[1].ToString(),
                        Contacts = dr[2].ToString(),
                        Telephone = dr[3].ToString(),
                        KraPin = dr[4].ToString(),

                        Balance = Convert.ToDouble(dr[5]),
                        CreditLimit = Convert.ToDouble(dr[6]),

                        DateJoined = Convert.ToDateTime(dr[7]),
                        LastInvoice = Convert.ToDateTime(dr[8]),
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[9]),
                            Code = dr[10].ToString(),
                            Name = dr[11].ToString()
                        }
                    });
                }
            }

            return customers;
        }

        public double GetCustomerBalance(Customers Customer) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT AccountBalance FROM vCustomers WHERE Sts=" + Customer.Station.Id + " AND Custid=" + Customer.Id);
            if (dr.Read()) {
                return Convert.ToDouble(dr[0]);
            }

            return 0;
        }

        public Suppliers GetSupplier(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT sp_idnt, sp_uuid, sp_name, ISNULL(NULLIF(sp_pin,''),'—')sp_pin, sp_contacts, sp_city, sp_telephone, sp_balance, sp_fuel, sp_lubes, sp_gas, sp_soda, sp_icon FROM Suppliers WHERE sp_idnt=" + idnt);
            if (dr.Read()) {
                return new Suppliers {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Pin = dr[3].ToString(),
                    Address = dr[4].ToString(),
                    City = dr[5].ToString(),
                    Telephone = dr[6].ToString(),
                    Balance = Convert.ToInt64(dr[7]),
                    Fuel = Convert.ToBoolean(dr[8]),
                    Lube = Convert.ToBoolean(dr[9]),
                    Gas = Convert.ToBoolean(dr[10]),
                    Soda = Convert.ToBoolean(dr[11]),
                    Icon = dr[12].ToString()
                };
            }

            return null;
        }

        public Suppliers GetSupplier(string uuid) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT sp_idnt, sp_uuid, sp_name, ISNULL(NULLIF(sp_pin,''),'—')sp_pin, sp_contacts, sp_city, sp_telephone, sp_balance, sp_fuel, sp_lubes, sp_gas, sp_soda, sp_icon FROM Suppliers WHERE sp_uuid COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '" + uuid + "'");
            if (dr.Read()) {
                return new Suppliers {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Pin = dr[3].ToString(),
                    Address = dr[4].ToString(),
                    City = dr[5].ToString(),
                    Telephone = dr[6].ToString(),
                    Balance = Convert.ToInt64(dr[7]),
                    Fuel = Convert.ToBoolean(dr[8]),
                    Lube = Convert.ToBoolean(dr[9]),
                    Gas = Convert.ToBoolean(dr[10]),
                    Soda = Convert.ToBoolean(dr[11]),
                    Icon = dr[12].ToString()
                };
            }

            return null;
        }

        public double GetSupplierBalance(Suppliers supplier) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT sp_balance FROM Suppliers WHERE sp_idnt=" + supplier.Id);
            if (dr.Read()) {
                return Convert.ToDouble(dr[0]);
            }

            return 0;
        }

        public List<Suppliers> GetSuppliers(string filter = "") {
            List<Suppliers> suppliers = new List<Suppliers>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT sp_idnt, sp_uuid, sp_name, ISNULL(NULLIF(sp_pin,''),'—')sp_pin, sp_contacts, sp_city, sp_telephone, sp_balance, sp_fuel, sp_lubes, sp_gas, sp_soda FROM Suppliers ORDER BY sp_name, sp_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    suppliers.Add(new Suppliers {
                        Id = Convert.ToInt64(dr[0]),
                        Uuid = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Pin = dr[3].ToString(),
                        Address = dr[4].ToString(),
                        City = dr[5].ToString(),
                        Telephone = dr[6].ToString(),
                        Balance = Convert.ToInt64(dr[7]),
                        Fuel = Convert.ToBoolean(dr[8]),
                        Lube = Convert.ToBoolean(dr[9]),
                        Gas = Convert.ToBoolean(dr[10]),
                        Soda = Convert.ToBoolean(dr[11])
                    });
                }
            }

            return suppliers;
        }

        public SuppliersPayment GetSuppliersPayment(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT p.sp_idnt, sp_date, sp_rcpt, sp_chqs, sp_invoice, sp_notes, sp_amount, sp_supp, sp_uuid, sp_name, ISNULL(sp_bank,0)sp_bank, ISNULL(bk_code,'')bk_code, ISNULL(bk_bank,'CASH')bk_bank FROM SuppliersPayments p LEFT OUTER JOIN BankAccounts ON sp_bank=bk_idnt LEFT OUTER JOIN Suppliers sp ON sp_supp=sp.sp_idnt WHERE p.sp_idnt=" + idnt);
            if (dr.Read()) {
                return new SuppliersPayment {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    DateString = Convert.ToDateTime(dr[1]).ToString("d MMMM, yyyy"),
                    Receipt = dr[2].ToString(),
                    Cheque = dr[3].ToString(),
                    Invoices = dr[4].ToString(),
                    Description = dr[5].ToString(),
                    Amount = Convert.ToDouble(dr[6]),
                    Supplier = new Suppliers {
                        Id = Convert.ToInt64(dr[7]),
                        Uuid = dr[8].ToString(),
                        Name = dr[9].ToString(),
                    },
                    Bank = new Bank {
                        Id = Convert.ToInt64(dr[10]),
                        Code = dr[11].ToString(),
                        Name = dr[12].ToString(),
                    }
                };
            }

            return null;
        }

        public SuppliersWithholding GetSuppliersWithholding(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT sw_idnt, sw_date, sw_receipt, sw_cheque, sw_invoice, sw_description, sw_amount, sw_supp, sp_uuid, sp_name, ISNULL(sw_bank,0)sp_bank, ISNULL(bk_code,'')bk_code, ISNULL(bk_bank,'CASH')bk_bank FROM SuppliersWithholding LEFT OUTER JOIN BankAccounts ON sw_bank=bk_idnt LEFT OUTER JOIN Suppliers sp ON sw_supp=sp.sp_idnt WHERE sw_idnt=" + idnt);
            if (dr.Read()) {
                return new SuppliersWithholding {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    DateString = Convert.ToDateTime(dr[1]).ToString("d MMMM, yyyy"),
                    Receipt = dr[2].ToString(),
                    Cheque = dr[3].ToString(),
                    Invoice = dr[4].ToString(),
                    Description = dr[5].ToString(),
                    Amount = Convert.ToDouble(dr[6]),
                    Supplier = new Suppliers {
                        Id = Convert.ToInt64(dr[7]),
                        Uuid = dr[8].ToString(),
                        Name = dr[9].ToString(),
                    },
                    Bank = new Bank {
                        Id = Convert.ToInt64(dr[10]),
                        Code = dr[11].ToString(),
                        Name = dr[12].ToString(),
                    }
                };
            }

            return null;
        }

        public List<SuppliersPayment> GetSuppliersPayments(DateTime start, DateTime stop, Suppliers supplier, string filter = "") {
            List<SuppliersPayment> payments = new List<SuppliersPayment>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "sp_rcpt+'-'+sp_chqs+'-'+sp_invoice+'-'+sp_notes+'-'+CAST(sp_amount AS NVARCHAR)+'-'+sp_name+'-'+ISNULL(bk_bank,'CASH')", "sp_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (supplier != null)
                filterString += " AND sp_supp=" + supplier.Id;

            SqlDataReader dr = conn.SqlServerConnect("SELECT sp_idx, sp_typ, sp_date, sp_rcpt, sp_chqs, sp_invoice, sp_notes, sp_amount, sp_supp, sp_uuid, sp_name, ISNULL(sp_bank,0)sp_bank, ISNULL(bk_code,'')bk_code, ISNULL(bk_bank,'CASH')bk_bank FROM (SELECT sp_idnt sp_idx, 0 sp_typ, sp_supp, sp_date, sp_rcpt, sp_chqs, sp_invoice, sp_notes, sp_amount, sp_bank FROM SuppliersPayments UNION ALL SELECT sw_idnt sp_idx, 1 sw_typ, sw_supp, sw_date, sw_receipt, sw_cheque, sw_invoice, sw_description, sw_amount, sw_bank FROM SuppliersWithholding) As Foo LEFT OUTER JOIN BankAccounts ON sp_bank=bk_idnt LEFT OUTER JOIN Suppliers sp ON sp_supp=sp.sp_idnt " + filterString + " ORDER BY sp_date, sp_chqs, sp_typ, sp_idx");
            if (dr.HasRows) {
                while (dr.Read()) {
                    payments.Add(new SuppliersPayment {
                        Id = Convert.ToInt64(dr[0]),
                        Type = Convert.ToInt64(dr[1]),
                        Date = Convert.ToDateTime(dr[2]),
                        DateString = Convert.ToDateTime(dr[2]).ToString("dd/MM/yyyy"),
                        Receipt = dr[3].ToString(),
                        Cheque = dr[4].ToString(),
                        Invoices = dr[5].ToString(),
                        Description = dr[6].ToString(),
                        Amount = Convert.ToDouble(dr[7]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[8]),
                            Uuid = dr[9].ToString(),
                            Name = dr[10].ToString(),
                        },
                        Bank = new Bank {
                            Id = Convert.ToInt64(dr[11]),
                            Code = dr[12].ToString(),
                            Name = dr[13].ToString(),
                        }
                    });
                }
            }

            return payments;
        }

        public SuppliersCredits GetSuppliersCredit(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT cn_idnt, cn_date, cn_added_on, cn_rcpt, cn_description, cn_amount, cnt_idnt, cnt_init, cnt_type, sp_idnt, sp_uuid, sp_name, ISNULL(st_idnt,0), ISNULL(st_code,''), ISNULL(st_name,'Multiple'), cn_added_by FROM CreditNotes INNER JOIN CreditNotesType ON cn_type=cnt_idnt INNER JOIN Suppliers ON cn_supp=sp_idnt LEFT OUTER JOIN Stations ON cn_station=st_idnt WHERE cn_idnt=" + idnt);
            if (dr.Read()) {
                return new SuppliersCredits {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    DateString = Convert.ToDateTime(dr[1]).ToString("d MMMM, yyyy"),
                    AddedOn = Convert.ToDateTime(dr[2]),
                    Receipt = dr[3].ToString(),
                    Description = dr[4].ToString(),
                    Amount = Convert.ToDouble(dr[5]),
                    Type = new SuppliersCreditsType {
                        Id = Convert.ToInt64(dr[6]),
                        Code = dr[7].ToString(),
                        Name = dr[8].ToString()
                    },
                    Supplier = new Suppliers {
                        Id = Convert.ToInt64(dr[9]),
                        Uuid = dr[10].ToString(),
                        Name = dr[11].ToString()
                    },
                    Station = new Stations {
                        Id = Convert.ToInt64(dr[12]),
                        Code = dr[13].ToString(),
                        Name = dr[14].ToString()
                    },
                    AddedBy = new Users {
                        Id = Convert.ToInt64(dr[15])
                    }
                };
            }

            return null;
        }

        public List<SuppliersCredits> GetSuppliersCredits(DateTime start, DateTime stop, Suppliers supplier, string filter = "") {
            List<SuppliersCredits> credits = new List<SuppliersCredits>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "cn_rcpt+'-'+cn_description+'-'+CAST(cn_amount AS NVARCHAR)+'-'+cnt_type+'-'+sp_name+'-'+ISNULL(st_code,'')+'-'+ISNULL(st_name,'')", "cn_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (supplier != null)
                filterString += " AND cn_supp=" + supplier.Id;

            SqlDataReader dr = conn.SqlServerConnect("SELECT cn_idnt, cn_date, cn_added_on, cn_rcpt, cn_description, cn_amount, cnt_idnt, cnt_init, cnt_type, sp_idnt, sp_uuid, sp_name, ISNULL(st_idnt,0), ISNULL(st_code,''), ISNULL(st_name,'N/A'), cn_added_by FROM CreditNotes INNER JOIN CreditNotesType ON cn_type=cnt_idnt INNER JOIN Suppliers ON cn_supp=sp_idnt LEFT OUTER JOIN Stations ON cn_station=st_idnt " + filterString + " ORDER BY cn_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    credits.Add(new SuppliersCredits {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        DateString = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                        AddedOn = Convert.ToDateTime(dr[2]),
                        Receipt = dr[3].ToString(),
                        Description = dr[4].ToString(),
                        Amount = Convert.ToDouble(dr[5]),
                        Type = new SuppliersCreditsType {
                            Id = Convert.ToInt64(dr[6]),
                            Code = dr[7].ToString(),
                            Name = dr[8].ToString()
                        },
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[9]),
                            Uuid = dr[10].ToString(),
                            Name = dr[11].ToString()
                        },
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[12]),
                            Code = dr[13].ToString(),
                            Name = dr[14].ToString()
                        },
                        AddedBy = new Users {
                            Id = Convert.ToInt64(dr[15])
                        }
                    });
                }
            }

            return credits;
        }

        public List<PaymentSchedule> GetSuppliersPaymentSchedule(DateTime start, DateTime stop, Suppliers supplier, string filter = "", string order = "") {
            List<PaymentSchedule> schedule = new List<PaymentSchedule>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "xp_invoice+CAST(xp_amount AS NVARCHAR)+'-'+sp_pin+'-'+sp_name", "xp_date BETWEEN @start AND @stop");
            if (supplier != null)
                filterString += " AND xp_supplier=" + supplier.Id;

            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT xp_idnt, xp_date, xp_invoice, xp_amount , ISNULL((SELECT SUM(CASE WHEN cn_type=0 THEN cn_amount ELSE 0-cn_amount END) cn_amts FROM CreditNotes WHERE cn_supp=xp_supplier AND cn_date BETWEEN @start AND @stop GROUP BY cn_supp),0) xp_credit , sp_idnt, sp_uuid, sp_name, ISNULL(NULLIF(sp_pin,''),'N/A')sp_pin FROM Expenses INNER JOIN Suppliers ON xp_supplier=sp_idnt " + filterString + " ORDER BY " + order + " sp_name, sp_idnt, xp_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    PaymentSchedule ps = new PaymentSchedule {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        Invoice = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Credits = Convert.ToDouble(dr[4]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[5]),
                            Uuid = dr[6].ToString(),
                            Name = dr[7].ToString(),
                            Pin = dr[8].ToString()
                        }
                    };

                    ps.ExVats = (100.0 / 116.0) * ps.Amount;
                    ps.PercSix = (6.0 / 100.0) * ps.ExVats;
                    ps.PercTen = (10.0 / 100.0) * ps.ExVats;
                    ps.Cheques = ps.Amount - ps.PercSix;

                    schedule.Add(ps);
                }
            }

            return schedule;
        }

        public List<PaymentLedger> GetSuppliersPaymentLedger(DateTime start, DateTime stop, Suppliers supplier, string filter = "", string order = "") {
            List<PaymentLedger> ledger = new List<PaymentLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "xp_invoice+CAST(xp_amount AS NVARCHAR)+'-'+sp_pin+'-'+sp_name", "xp_date BETWEEN @start AND @stop");
            if (supplier != null)
                filterString += " AND xp_supplier=" + supplier.Id;

            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT xp_idnt, xp_date, xp_invoice, xp_amount, ISNULL((SELECT SUM(sp_amount)amts FROM SuppliersPayments WHERE sp_supp=xp_supplier AND sp_date BETWEEN @start AND @stop GROUP BY sp_supp),0)xp_paid, sp_idnt, sp_uuid, sp_name, ISNULL(NULLIF(sp_pin,''),'N/A')sp_pin FROM Expenses INNER JOIN Suppliers ON xp_supplier=sp_idnt " + filterString + " ORDER BY " + order + " sp_name, sp_idnt, xp_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    PaymentLedger ldg = new PaymentLedger {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        Invoice = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Paid = Convert.ToDouble(dr[4]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[5]),
                            Uuid = dr[6].ToString(),
                            Name = dr[7].ToString(),
                            Pin = dr[8].ToString()
                        }
                    };

                    ldg.Balance = ldg.Amount - ldg.Paid;
                    ledger.Add(ldg);
                }
            }

            return ledger;
        }

        public List<Wetstock> GetWetstocks(DateTime start, DateTime stop, Stations station) {
            List<Wetstock> wetstock = new List<Wetstock>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start + "', @stop DATE='" + stop + "'; SELECT ws_date AS pcol_date, ws_open AS OPENING, ws_delv AS DELIVERIES, ws_sales AS SALES, ws_test AS TESTS, ws_open+ws_delv+ws_test-ws_sales AS BOOK, ws_actual AS ACTUALS, ws_water AS WATER, ws_actual-ws_open-ws_delv-ws_test+ws_sales AS VARIANCE, CASE WHEN ws_sales=0 THEN 100 ELSE ((ws_actual-ws_open-ws_delv-ws_test+ws_sales)/ws_sales)*100 END AS VARIANCE_PERC, tnk_idnt, tnk_name, id_, itemName FROM pTanksWetstocks INNER JOIN Tanks ON ws_tank = tnk_idnt INNER JOIN Products ON tnk_fuel = id_ WHERE ws_date BETWEEN @start AND @stop ORDER BY itemName, tnk_name, ws_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Wetstock ws = new Wetstock {
                        Date = Convert.ToDateTime(dr[0]),
                        Opening = Convert.ToDouble(dr[1]),
                        Deliveries = Convert.ToDouble(dr[2]),
                        Sales = Convert.ToDouble(dr[3]),
                        Tests = Convert.ToDouble(dr[4]),
                        Books = Convert.ToDouble(dr[5]),
                        Actuals = Convert.ToDouble(dr[6]),
                        Water = Convert.ToDouble(dr[7]),
                        Variance = Convert.ToDouble(dr[8]),
                        VariancePerc = Convert.ToDouble(dr[9]),
                        Tank = new Tank {
                            Id = Convert.ToInt64(dr[10]),
                            Name = dr[11].ToString(),
                            Fuel = new Fuel {
                                Id = Convert.ToInt64(dr[12]),
                                Name = dr[13].ToString()
                            }
                        }
                    };
                    
                    wetstock.Add(ws);
                }
            }

            return wetstock;
        }

        public List<WetstockModel> GetWetstockModels(DateTime start, DateTime stop, Stations station)
        {
            List<WetstockModel> wsm = new List<WetstockModel>();
            List<Wetstock> wst = new List<Wetstock>();
            int tank = 0;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start + "', @stop DATE='" + stop + "'; SELECT ws_date AS pcol_date, ws_open AS OPENING, ws_delv AS DELIVERIES, ws_sales AS SALES, ws_test AS TESTS, ws_open+ws_delv+ws_test-ws_sales AS BOOK, ws_actual AS ACTUALS, ws_water AS WATER, ws_actual-ws_open-ws_delv-ws_test+ws_sales AS VARIANCE, CASE WHEN ws_sales=0 THEN 100 ELSE ((ws_actual-ws_open-ws_delv-ws_test+ws_sales)/ws_sales)*100 END AS VARIANCE_PERC, tnk_idnt, tnk_name, id_, itemName FROM pTanksWetstocks INNER JOIN Tanks ON ws_tank = tnk_idnt INNER JOIN Products ON tnk_fuel = id_ WHERE ws_date BETWEEN @start AND @stop ORDER BY itemName, tnk_name, ws_date");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!tank.Equals(Convert.ToInt32(dr[10])))
                    {

                    }

                    Wetstock ws = new Wetstock
                    {
                        Date = Convert.ToDateTime(dr[0]),
                        Opening = Convert.ToDouble(dr[1]),
                        Deliveries = Convert.ToDouble(dr[2]),
                        Sales = Convert.ToDouble(dr[3]),
                        Tests = Convert.ToDouble(dr[4]),
                        Books = Convert.ToDouble(dr[5]),
                        Actuals = Convert.ToDouble(dr[6]),
                        Water = Convert.ToDouble(dr[7]),
                        Variance = Convert.ToDouble(dr[8]),
                        VariancePerc = Convert.ToDouble(dr[9]),
                        Tank = new Tank
                        {
                            Id = Convert.ToInt64(dr[10]),
                            Name = dr[11].ToString(),
                            Fuel = new Fuel
                            {
                                Id = Convert.ToInt64(dr[12]),
                                Name = dr[13].ToString()
                            }
                        }
                    };

                    //wetstock.Add(ws);
                }
            }


            return wsm;
        }

        public List<WetstockSummary> GetWetstockSummary(DateTime start, DateTime stop, Stations station) {
            List<WetstockSummary> wetstocks = new List<WetstockSummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "';  SELECT tnk, tnk_name, fuel, itemName, (SELECT TOP(1)td_reading FROM TanksDips WHERE CAST(td_date AS DATE)<@start AND td_tank=tnk ORDER BY td_date DESC) op, SUM(sales)sale, SUM(rtns)rtns, SUM(delv)delv, (SELECT TOP(1)td_reading FROM TanksDips WHERE CAST(td_date AS DATE)<=@stop AND td_tank=tnk ORDER BY td_date DESC) dips FROM (SELECT pcol_tank tnk, pcol_fuel fuel, pcol_electronic_test rtns, pcol_electronic_cl-pcol_electronic_op sales, 0 delv FROM PumpsCollections WHERE pcol_date BETWEEN @start AND @stop UNION ALL SELECT fr_tank, fr_fuel, 0,0, fr_quantity FROM FuelReceipts WHERE fr_date BETWEEN @start AND @stop) As Foo INNER JOIN Tanks ON tnk=tnk_idnt INNER JOIN Products ON id_=fuel GROUP BY tnk, fuel, tnk_name, itemName ORDER BY fuel, tnk_name");
            if (dr.HasRows) {
                while (dr.Read()) {
                    WetstockSummary ws = new WetstockSummary {                     
                        Tank = new Tank {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString()
                        },
                        Fuel = new Fuel {
                            Id = Convert.ToInt64(dr[2]),
                            Name = dr[3].ToString()
                        },
                        Opening = Convert.ToDouble(dr[4]),
                        Sale = Convert.ToDouble(dr[5]),
                        Returns = Convert.ToDouble(dr[6]),
                        Delivery = Convert.ToDouble(dr[7]),
                        Dips = Convert.ToDouble(dr[8]),
                    };

                    ws.Closing = ws.Opening + ws.Returns + ws.Delivery - ws.Sale;
                    wetstocks.Add(ws);
                }
            }

            return wetstocks;
        }

        public List<SelectListItem> GetStationsIEnumerable(bool includeOthers = false) {
            List<SelectListItem> stations = GetIEnumerable("SELECT st_idnt, st_name FROM Stations ORDER BY st_order");
            if (includeOthers) {
                stations.Insert(0, new SelectListItem {
                    Value = "0",
                    Text = "Multiple",
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

        public List<TrucksMonthlySummary> GetTrucksMonthlySummary(int year) {
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
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_date, tf_truck, tr_registration, tf_supplier, sp_name, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated, tf_description FROM TrucksFuel INNER JOIN Trucks ON tr_idnt=tf_truck INNER JOIN Suppliers ON sp_idnt=tf_supplier WHERE tf_idnt=" + idnt);
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
                    VatAmount = Convert.ToDouble(dr[10]),
                    Description = dr[12].ToString()
                };

                xp.DateString = xp.Date.ToString("d MMMM, yyyy");
                xp.Exclussive = (xp.VatAmount / 0.08);
                xp.Zerorated = xp.Amount - xp.VatAmount - xp.Exclussive;
                
                return xp;
            }

            return new TrucksFuelExpense();
        }

        public List<TrucksFuelExpense> GetTrucksFuelExpense(int month, int year) {
            List<TrucksFuelExpense> report = new List<TrucksFuelExpense>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tf_idnt, tf_date, tf_truck, tr_registration, tf_supplier, sp_name, tf_invoice, tf_qnty, tf_price, tf_amount, tf_vatamts, tf_zerorated, tf_description FROM TrucksFuel INNER JOIN Trucks ON tr_idnt=tf_truck INNER JOIN Suppliers ON sp_idnt=tf_supplier WHERE MONTH(tf_date)=" + month + " AND YEAR(tf_date)=" + year + " ORDER BY tf_date, tf_idnt");
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
                        VatAmount = Convert.ToDouble(dr[10]),
                        Description = dr[12].ToString()
                    };

                    xp.DateString = xp.Date.ToString("d MMMM, yyyy");
                    xp.Exclussive = (xp.VatAmount / 0.08);
					xp.Zerorated = xp.Amount - xp.VatAmount - xp.Exclussive;
					
					report.Add(xp);
                }
            }

            return report;
        }

        public StationsExpenses GetStationsExpenses(long idnt) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT xp_idnt, xp_date, xp_invoice, xp_amount, xp_vat_amts, xp_zero_rated, xp_description, xp_user, ec_idnt, ec_category, sp_idnt, sp_name, ISNULL(st_idnt,0)st_idnt, st_code, ISNULL(st_name,'Multiple')st_name FROM Expenses INNER JOIN ExpensesCategory ON xp_category=ec_idnt INNER JOIN Suppliers ON xp_supplier=sp_idnt LEFT OUTER JOIN Stations ON xp_station=st_idnt WHERE xp_idnt=" + idnt);
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

        public List<ExpensesLedger> GetCoreExpenses(DateTime start, DateTime stop, Suppliers supplier, string filter = "") {
            List<ExpensesLedger> expenses = new List<ExpensesLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            string filterString = conn.GetQueryString(filter, "xp_invoice+'-'+ec_category+'-'+xp_description+'-'+CAST(xp_amount AS NVARCHAR)+'-'+ISNULL(sp_name,'Unknown')+'-'+ISNULL(st_code,'')+'-'+ISNULL(st_name,'')", "xp_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (supplier != null)
                filterString += " AND xp_supplier IN (" + supplier.Id + ")";

            SqlDataReader dr = conn.SqlServerConnect("SELECT xp_idnt, xp_type, xp_date, xp_invoice, ec_category, xp_description, xp_amount, xp_vat_amts, xp_zero_rated, ISNULL(sp_idnt,0)spid,ISNULL(sp_uuid,'Y67WzS54')spuuid,ISNULL(sp_name,'Unknown')spname, ISNULL(st_idnt,0)stidnt, ISNULL(st_code,'')stcode, ISNULL(st_name,'')stname, xp_supplier FROM vExpensesLedger LEFT OUTER JOIN Suppliers ON sp_idnt=xp_supplier LEFT OUTER JOIN Stations ON st_idnt=xp_station " + filterString + " ORDER BY xp_date, xp_invoice, xp_idnt, xp_type");
            if (dr.HasRows) {
                while (dr.Read()) { 
                    expenses.Add(new ExpensesLedger {
                        Id = Convert.ToInt64(dr[0]),
                        Type = Convert.ToInt32(dr[1]),
                        Date = Convert.ToDateTime(dr[2]),
                        DateString = Convert.ToDateTime(dr[2]).ToString("dd/MM/yyyy"),
                        Invoice = dr[3].ToString(),
                        Category = dr[4].ToString(),
                        Description = dr[5].ToString(),
                        Amount = Convert.ToDouble(dr[6]),
                        Vat = Convert.ToDouble(dr[7]),
                        Zero = Convert.ToDouble(dr[8]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[9]),
                            Uuid = dr[10].ToString(),
                            Name = dr[11].ToString(),
                        },
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[12]),
                            Code = dr[13].ToString(),
                            Name = dr[14].ToString(),
                        }
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

        public List<Ledger> GetManagementIncomePerStation(DateTime start, DateTime stop) {
            List<Ledger> ledger = new List<Ledger>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT ts_type, tt_name, CASE WHEN ts_item IN (5,6) THEN '' ELSE tt_prefix END tt_prefix, ts_item, ti_name, SUM(CASE WHEN ts_station=1 THEN ts_amount ELSE 0 END) bypass, SUM(CASE WHEN ts_station=2 THEN ts_amount ELSE 0 END) git, SUM(CASE WHEN ts_station=3 THEN ts_amount ELSE 0 END) kaaga, SUM(CASE WHEN ts_station=4 THEN ts_amount ELSE 0 END) kenol, SUM(CASE WHEN ts_station=5 THEN ts_amount ELSE 0 END) kinoru, SUM(CASE WHEN ts_station=6 THEN ts_amount ELSE 0 END) kirunga, SUM(CASE WHEN ts_station=7 THEN ts_amount ELSE 0 END) kobil, SUM(CASE WHEN ts_station=8 THEN ts_amount ELSE 0 END) maua, SUM(CASE WHEN ts_station=9 THEN ts_amount ELSE 0 END) nkubu, SUM(CASE WHEN ts_station=10 THEN ts_amount ELSE 0 END) ojijo, SUM(CASE WHEN ts_station=11 THEN ts_amount ELSE 0 END) oryx, SUM(CASE WHEN ts_station=12 THEN ts_amount ELSE 0 END) uhuru, SUM(CASE WHEN ts_station=13 THEN ts_amount ELSE 0 END) viewpt, SUM(ts_amount) total FROM vManagementStationsIncome INNER JOIN TransactionsType ON ts_type=tt_idnt INNER JOIN TransactionsItems ON ts_item=ti_idnt WHERE ts_date BETWEEN @start AND @stop GROUP BY ts_type, tt_name, ts_item, ti_name, ti_order, tt_prefix ORDER BY ts_type, ti_order");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ledger.Add(new Ledger {
                        Type = new LedgerType {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Prefix = dr[2].ToString()
                        },
                        Item = new LedgerItem {
                            Id = Convert.ToInt64(dr[3]),
                            Name = dr[4].ToString()
                        },
                        Bypass = Convert.ToDouble(dr[5]),
                        Gitimbine = Convert.ToDouble(dr[6]),
                        Kaaga = Convert.ToDouble(dr[7]),
                        Kenol = Convert.ToDouble(dr[8]),
                        Kinoru = Convert.ToDouble(dr[9]),
                        Kirunga = Convert.ToDouble(dr[10]),
                        Kobil = Convert.ToDouble(dr[11]),
                        Maua = Convert.ToDouble(dr[12]),
                        Nkubu = Convert.ToDouble(dr[13]),
                        Ojijo = Convert.ToDouble(dr[14]),
                        Oryx = Convert.ToDouble(dr[15]),
                        Uhuru = Convert.ToDouble(dr[16]),
                        Viewpt = Convert.ToDouble(dr[17]),
                        Total = Convert.ToDouble(dr[18]),
                    });
                }
            }

            return ledger;
        }

        public List<Ledger> GetManagementCostsPerStation(DateTime start, DateTime stop) {
            List<Ledger> ledger = new List<Ledger>();
            Ledger opening = new Ledger { Item = new LedgerItem { Name = "OpeningStock" } };
            Ledger closing = new Ledger { Item = new LedgerItem { Name = "ClosingStock" } };

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT [type], 'Cost of Goods Sold' tt_name, 'Cost' tt_prefix, CASE WHEN item='FUEL' THEN 1 WHEN item='SODA' THEN 2 WHEN item='GAS' THEN 3 WHEN item='LUBES' THEN 4 ELSE 99 END item, ti_name, SUM(CASE WHEN st=1 THEN sl*bp ELSE 0 END) bypass, SUM(CASE WHEN st=2 THEN sl*bp ELSE 0 END) gitimbine, SUM(CASE WHEN st=3 THEN sl*bp ELSE 0 END) kaaga, SUM(CASE WHEN st=4 THEN sl*bp ELSE 0 END) kenol, SUM(CASE WHEN st=5 THEN sl*bp ELSE 0 END) kinoru, SUM(CASE WHEN st=6 THEN sl*bp ELSE 0 END) kirunga, SUM(CASE WHEN st=7 THEN sl*bp ELSE 0 END) kobil, SUM(CASE WHEN st=8 THEN sl*bp ELSE 0 END) maua, SUM(CASE WHEN st=9 THEN sl*bp ELSE 0 END) nkubu, SUM(CASE WHEN st=10 THEN sl*bp ELSE 0 END) ojijo, SUM(CASE WHEN st=11 THEN sl*bp ELSE 0 END) oryx, SUM(CASE WHEN st=12 THEN sl*bp ELSE 0 END) uhuru, SUM(CASE WHEN st=13 THEN sl*bp ELSE 0 END) viewpt, SUM(sl*bp)total, SUM(CASE WHEN st=1 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_bypass, SUM(CASE WHEN st=2 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_git, SUM(CASE WHEN st=3 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_kaaga, SUM(CASE WHEN st=4 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_kenol, SUM(CASE WHEN st=5 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_kinoru, SUM(CASE WHEN st=6 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_kirunga, SUM(CASE WHEN st=7 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_kobil, SUM(CASE WHEN st=8 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_maua, SUM(CASE WHEN st=9 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_nkubu, SUM(CASE WHEN st=10 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_ojijo, SUM(CASE WHEN st=11 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_oryx, SUM(CASE WHEN st=12 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_uhuru, SUM(CASE WHEN st=13 THEN (CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) ELSE 0 END) op_viewpt, SUM(CASE WHEN item='FUEL' THEN cl*bp ELSE (cl+tr+sl-inn)*bp END) op_total, SUM(CASE WHEN st=1 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_bypass, SUM(CASE WHEN st=2 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_git, SUM(CASE WHEN st=3 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_kaaga, SUM(CASE WHEN st=4 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_kenol, SUM(CASE WHEN st=5 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_kinoru, SUM(CASE WHEN st=6 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_kirunga, SUM(CASE WHEN st=7 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_kobil, SUM(CASE WHEN st=8 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_maua, SUM(CASE WHEN st=9 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_nkubu, SUM(CASE WHEN st=10 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_ojijo, SUM(CASE WHEN st=11 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_oryx, SUM(CASE WHEN st=12 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_uhuru, SUM(CASE WHEN st=13 THEN (CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) ELSE 0 END) cl_viewpt, SUM(CASE WHEN item='FUEL' THEN (cl+inn-sl)*bp ELSE cl*bp END) cl_total FROM ( SELECT 2[type],Category item, 1 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_bypass.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_bypass.dbo.ProductsMovement INNER JOIN shell_bypass.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_bypass.dbo.SalesDetails INNER JOIN shell_bypass.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_bypass.dbo.PurchasesDetails INNER JOIN shell_bypass.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_bypass.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_bypass.dbo.PurchasesDetails INNER JOIN shell_bypass.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_bypass.dbo.PurchasesDetails INNER JOIN shell_bypass.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 2 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_gitimbine.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_gitimbine.dbo.ProductsMovement INNER JOIN shell_gitimbine.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_gitimbine.dbo.SalesDetails INNER JOIN shell_gitimbine.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_gitimbine.dbo.PurchasesDetails INNER JOIN shell_gitimbine.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_gitimbine.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_gitimbine.dbo.PurchasesDetails INNER JOIN shell_gitimbine.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_gitimbine.dbo.PurchasesDetails INNER JOIN shell_gitimbine.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 3 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_kaaga.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_kaaga.dbo.ProductsMovement INNER JOIN shell_kaaga.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_kaaga.dbo.SalesDetails INNER JOIN shell_kaaga.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_kaaga.dbo.PurchasesDetails INNER JOIN shell_kaaga.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_kaaga.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_kaaga.dbo.PurchasesDetails INNER JOIN shell_kaaga.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_kaaga.dbo.PurchasesDetails INNER JOIN shell_kaaga.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 4 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_kenol.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_kenol.dbo.ProductsMovement INNER JOIN shell_kenol.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_kenol.dbo.SalesDetails INNER JOIN shell_kenol.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_kenol.dbo.PurchasesDetails INNER JOIN shell_kenol.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_kenol.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_kenol.dbo.PurchasesDetails INNER JOIN shell_kenol.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_kenol.dbo.PurchasesDetails INNER JOIN shell_kenol.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 5 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_kinoru.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_kinoru.dbo.ProductsMovement INNER JOIN shell_kinoru.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_kinoru.dbo.SalesDetails INNER JOIN shell_kinoru.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_kinoru.dbo.PurchasesDetails INNER JOIN shell_kinoru.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_kinoru.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_kinoru.dbo.PurchasesDetails INNER JOIN shell_kinoru.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_kinoru.dbo.PurchasesDetails INNER JOIN shell_kinoru.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 6 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_kirunga.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_kirunga.dbo.ProductsMovement INNER JOIN shell_kirunga.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_kirunga.dbo.SalesDetails INNER JOIN shell_kirunga.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_kirunga.dbo.PurchasesDetails INNER JOIN shell_kirunga.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_kirunga.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_kirunga.dbo.PurchasesDetails INNER JOIN shell_kirunga.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_kirunga.dbo.PurchasesDetails INNER JOIN shell_kirunga.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 7 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_kobil.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_kobil.dbo.ProductsMovement INNER JOIN shell_kobil.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_kobil.dbo.SalesDetails INNER JOIN shell_kobil.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_kobil.dbo.PurchasesDetails INNER JOIN shell_kobil.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_kobil.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_kobil.dbo.PurchasesDetails INNER JOIN shell_kobil.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_kobil.dbo.PurchasesDetails INNER JOIN shell_kobil.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 8 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_maua.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_maua.dbo.ProductsMovement INNER JOIN shell_maua.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_maua.dbo.SalesDetails INNER JOIN shell_maua.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_maua.dbo.PurchasesDetails INNER JOIN shell_maua.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_maua.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_maua.dbo.PurchasesDetails INNER JOIN shell_maua.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_maua.dbo.PurchasesDetails INNER JOIN shell_maua.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 9 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_nkubu.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_nkubu.dbo.ProductsMovement INNER JOIN shell_nkubu.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_nkubu.dbo.SalesDetails INNER JOIN shell_nkubu.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_nkubu.dbo.PurchasesDetails INNER JOIN shell_nkubu.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_nkubu.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_nkubu.dbo.PurchasesDetails INNER JOIN shell_nkubu.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_nkubu.dbo.PurchasesDetails INNER JOIN shell_nkubu.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 10 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_ojijo.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_ojijo.dbo.ProductsMovement INNER JOIN shell_ojijo.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_ojijo.dbo.SalesDetails INNER JOIN shell_ojijo.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_ojijo.dbo.PurchasesDetails INNER JOIN shell_ojijo.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_ojijo.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_ojijo.dbo.PurchasesDetails INNER JOIN shell_ojijo.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_ojijo.dbo.PurchasesDetails INNER JOIN shell_ojijo.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 11 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_oryx.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_oryx.dbo.ProductsMovement INNER JOIN shell_oryx.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_oryx.dbo.SalesDetails INNER JOIN shell_oryx.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_oryx.dbo.PurchasesDetails INNER JOIN shell_oryx.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_oryx.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_oryx.dbo.PurchasesDetails INNER JOIN shell_oryx.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_oryx.dbo.PurchasesDetails INNER JOIN shell_oryx.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 12 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_uhuru.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_uhuru.dbo.ProductsMovement INNER JOIN shell_uhuru.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_uhuru.dbo.SalesDetails INNER JOIN shell_uhuru.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_uhuru.dbo.PurchasesDetails INNER JOIN shell_uhuru.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_uhuru.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_uhuru.dbo.PurchasesDetails INNER JOIN shell_uhuru.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_uhuru.dbo.PurchasesDetails INNER JOIN shell_uhuru.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type],Category item, 13 st, ISNULL(costx,0) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl FROM shell_viewpt.dbo.pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM shell_viewpt.dbo.ProductsMovement INNER JOIN shell_viewpt.dbo.ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty FROM shell_viewpt.dbo.SalesDetails INNER JOIN shell_viewpt.dbo.Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty FROM shell_viewpt.dbo.PurchasesDetails INNER JOIN shell_viewpt.dbo.Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM shell_viewpt.dbo.ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM shell_viewpt.dbo.PurchasesDetails INNER JOIN shell_viewpt.dbo.Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM shell_viewpt.dbo.PurchasesDetails INNER JOIN shell_viewpt.dbo.Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 UNION ALL SELECT 2[type], pcol_item, 1 st, (SELECT TOP(1) price FROM shell_bypass.dbo.PurchasesDetails INNER JOIN shell_bypass.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_bypass.dbo.PurchasesDetails INNER JOIN shell_bypass.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_bypass.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_bypass.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 2 st, (SELECT TOP(1) price FROM shell_gitimbine.dbo.PurchasesDetails INNER JOIN shell_gitimbine.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_gitimbine.dbo.PurchasesDetails INNER JOIN shell_gitimbine.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_gitimbine.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_gitimbine.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 3 st, (SELECT TOP(1) price FROM shell_kaaga.dbo.PurchasesDetails INNER JOIN shell_kaaga.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_kaaga.dbo.PurchasesDetails INNER JOIN shell_kaaga.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_kaaga.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_kaaga.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 4 st, (SELECT TOP(1) price FROM shell_kenol.dbo.PurchasesDetails INNER JOIN shell_kenol.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_kenol.dbo.PurchasesDetails INNER JOIN shell_kenol.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_kenol.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_kenol.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 5 st, (SELECT TOP(1) price FROM shell_kinoru.dbo.PurchasesDetails INNER JOIN shell_kinoru.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_kinoru.dbo.PurchasesDetails INNER JOIN shell_kinoru.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_kinoru.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_kinoru.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 6 st, (SELECT TOP(1) price FROM shell_kirunga.dbo.PurchasesDetails INNER JOIN shell_kirunga.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_kirunga.dbo.PurchasesDetails INNER JOIN shell_kirunga.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_kirunga.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_kirunga.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 7 st, (SELECT TOP(1) price FROM shell_kobil.dbo.PurchasesDetails INNER JOIN shell_kobil.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_kobil.dbo.PurchasesDetails INNER JOIN shell_kobil.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_kobil.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_kobil.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 8 st, (SELECT TOP(1) price FROM shell_maua.dbo.PurchasesDetails INNER JOIN shell_maua.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_maua.dbo.PurchasesDetails INNER JOIN shell_maua.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_maua.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_maua.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 9 st, (SELECT TOP(1) price FROM shell_nkubu.dbo.PurchasesDetails INNER JOIN shell_nkubu.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_nkubu.dbo.PurchasesDetails INNER JOIN shell_nkubu.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_nkubu.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_nkubu.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 10 st, (SELECT TOP(1) price FROM shell_ojijo.dbo.PurchasesDetails INNER JOIN shell_ojijo.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_ojijo.dbo.PurchasesDetails INNER JOIN shell_ojijo.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_ojijo.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_ojijo.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 11 st, (SELECT TOP(1) price FROM shell_oryx.dbo.PurchasesDetails INNER JOIN shell_oryx.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_oryx.dbo.PurchasesDetails INNER JOIN shell_oryx.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_oryx.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_oryx.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 12 st, (SELECT TOP(1) price FROM shell_uhuru.dbo.PurchasesDetails INNER JOIN shell_uhuru.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_uhuru.dbo.PurchasesDetails INNER JOIN shell_uhuru.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_uhuru.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_uhuru.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel UNION ALL SELECT 2[type], pcol_item, 13 st, (SELECT TOP(1) price FROM shell_viewpt.dbo.PurchasesDetails INNER JOIN shell_viewpt.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, (SELECT SUM(qty) FROM shell_viewpt.dbo.PurchasesDetails INNER JOIN shell_viewpt.dbo.Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_ between pcol_from And pcol_to) pcol_inn, pcol_sales, 0 trf, pcol_open FROM (SELECT pcol_fuel, 'FUEL' pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM shell_viewpt.dbo.PumpsCollections WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM shell_viewpt.dbo.TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel ) As Foo INNER JOIN core_system.dbo.TransactionsItems ON ti_idnt=(CASE WHEN item='FUEL' THEN 1 WHEN item='SODA' THEN 2 WHEN item='GAS' THEN 3 WHEN item='LUBES' THEN 4 END) GROUP BY [type], item, ti_name, ti_order ORDER BY ti_order");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ledger.Add(new Ledger {
                        Type = new LedgerType {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Prefix = dr[2].ToString()
                        },
                        Item = new LedgerItem {
                            Id = Convert.ToInt64(dr[3]),
                            Name = dr[4].ToString()
                        },
                        Bypass = Convert.ToDouble(dr[5]),
                        Gitimbine = Convert.ToDouble(dr[6]),
                        Kaaga = Convert.ToDouble(dr[7]),
                        Kenol = Convert.ToDouble(dr[8]),
                        Kinoru = Convert.ToDouble(dr[9]),
                        Kirunga = Convert.ToDouble(dr[10]),
                        Kobil = Convert.ToDouble(dr[11]),
                        Maua = Convert.ToDouble(dr[12]),
                        Nkubu = Convert.ToDouble(dr[13]),
                        Ojijo = Convert.ToDouble(dr[14]),
                        Oryx = Convert.ToDouble(dr[15]),
                        Uhuru = Convert.ToDouble(dr[16]),
                        Viewpt = Convert.ToDouble(dr[17]),
                        Total = Convert.ToDouble(dr[18]),
                    });

                    opening.Bypass += Convert.ToDouble(dr[19]);
                    opening.Gitimbine += Convert.ToDouble(dr[20]);
                    opening.Kaaga += Convert.ToDouble(dr[21]);
                    opening.Kenol += Convert.ToDouble(dr[22]);
                    opening.Kinoru += Convert.ToDouble(dr[23]);
                    opening.Kirunga += Convert.ToDouble(dr[24]);
                    opening.Kobil += Convert.ToDouble(dr[25]);
                    opening.Maua += Convert.ToDouble(dr[26]);
                    opening.Nkubu += Convert.ToDouble(dr[27]);
                    opening.Ojijo += Convert.ToDouble(dr[28]);
                    opening.Oryx += Convert.ToDouble(dr[29]);
                    opening.Uhuru += Convert.ToDouble(dr[30]);
                    opening.Viewpt += Convert.ToDouble(dr[31]);
                    opening.Total = Convert.ToDouble(dr[32]);

                    closing.Bypass -= Convert.ToDouble(dr[33]);
                    closing.Gitimbine -= Convert.ToDouble(dr[34]);
                    closing.Kaaga -= Convert.ToDouble(dr[35]);
                    closing.Kenol -= Convert.ToDouble(dr[36]);
                    closing.Kinoru -= Convert.ToDouble(dr[37]);
                    closing.Kirunga -= Convert.ToDouble(dr[38]);
                    closing.Kobil -= Convert.ToDouble(dr[39]);
                    closing.Maua -= Convert.ToDouble(dr[40]);
                    closing.Nkubu -= Convert.ToDouble(dr[41]);
                    closing.Ojijo -= Convert.ToDouble(dr[42]);
                    closing.Oryx-= Convert.ToDouble(dr[43]);
                    closing.Uhuru -= Convert.ToDouble(dr[44]);
                    closing.Viewpt -= Convert.ToDouble(dr[45]);
                    closing.Total -= Convert.ToDouble(dr[46]);
                }
            }

            ledger.Add(opening);
            ledger.Add(closing);

            return ledger;
        }


        //::Data Writters
        public Suppliers SaveSuppliers(Suppliers supp) {
            SqlServerConnection conn = new SqlServerConnection();
            supp.Id = conn.SqlServerUpdate("DECLARE @idnt INT='" + supp.Id + "', @name NVARCHAR(250)='" + supp.Name + "', @pin NVARCHAR(250)='" + supp.Pin + "', @adds NVARCHAR(250)='" + supp.Address + "', @city NVARCHAR(250)='" + supp.City + "', @tels NVARCHAR(250)='" + supp.Telephone + "', @email NVARCHAR(250)='" + supp.Email + "'; IF NOT EXISTS (SELECT sp_idnt FROM Suppliers where sp_idnt=@idnt) BEGIN INSERT INTO Suppliers (sp_name, sp_pin, sp_contacts, sp_city, sp_telephone, sp_email) output INSERTED.sp_idnt VALUES (@name, @pin, @adds, @city, @tels, @email) END ELSE BEGIN UPDATE Suppliers SET sp_name=@name, sp_pin=@pin, sp_contacts=@adds, sp_city=@city, sp_telephone=@tels, sp_email=@email output INSERTED.sp_idnt WHERE sp_idnt=@idnt END");
                     
            return supp;
        }

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

        public StationsExpenses SaveStationsExpenses(StationsExpenses expense) {
            SqlServerConnection conn = new SqlServerConnection();
            expense.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + expense.Id + ", @date DATE='" + expense.Date + "', @catg INT=" + expense.Category.Id + ", @supp INT=" + expense.Supplier.Id + ", @stns INT=" + expense.Station.Id + ",  @invs NVARCHAR(MAX)='" + expense.Invoice + "', @amts FLOAT=" + expense.Amount + ", @vats FLOAT=" + expense.VatAmount + ", @zero FLOAT=" + expense.Zerorated + ", @user NVARCHAR(50)='" + Username + "', @desc NVARCHAR(MAX)='" + expense.Description + "'; IF NOT EXISTS (SELECT xp_idnt FROM Expenses WHERE xp_idnt=@idnt) BEGIN INSERT INTO Expenses (xp_date, xp_invoice, xp_category, xp_station, xp_supplier, xp_amount, xp_vat_amts, xp_zero_rated, xp_description, xp_user) output INSERTED.xp_idnt VALUES (@date, @invs, @catg, @stns, @supp, @amts, @vats, @zero, @desc, @user) END ELSE BEGIN UPDATE Expenses SET xp_date=@date, xp_invoice=@invs, xp_category=@catg, xp_station=@stns, xp_supplier=@supp, xp_amount=@amts, xp_vat_amts=@vats, xp_zero_rated=@zero, xp_description=@desc output INSERTED.xp_idnt WHERE xp_idnt=@idnt END");

            return expense;
        }

        public Delivery SaveDelivery(Delivery delivery) {
            SqlServerConnection conn = new SqlServerConnection();
            delivery.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + delivery.Id + ", @date DATE='" + delivery.Date + "', @stns INT=" + delivery.Station.Id + ", @bank INT=" + delivery.Bank.Id + ",  @type INT=" + delivery.Type.Id + ", @rcpt NVARCHAR(MAX)='" + delivery.Receipt + "', @note NVARCHAR(MAX)='" + delivery.Description + "', @amts FLOAT=" + delivery.Amount + ", @user INT=" + Actor + "; IF NOT EXISTS (SELECT dlv_idnt FROM Delivery WHERE dlv_idnt=@idnt) BEGIN INSERT INTO Delivery (dlv_date, dlv_rcpt, dlv_station, dlv_type, dlv_bank, dlv_amount, dlv_added_by, dlv_notes) output INSERTED.dlv_idnt VALUES (@date, @rcpt, @stns, @type, @bank, @amts, @user, @note) END ELSE BEGIN UPDATE Delivery SET dlv_date=@date, dlv_rcpt=@rcpt, dlv_station=@stns, dlv_type=@type, dlv_bank=@bank, dlv_amount=@amts, dlv_notes=@note output INSERTED.dlv_idnt WHERE dlv_idnt=@idnt END");

            PettyCashObject PettyCashObject = JsonConvert.DeserializeObject<PettyCashObject>(delivery.JSonExpense);

            conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM DeliveryPettyCash WHERE pc_idnt NOT IN " + PettyCashObject.Idnts + " AND pc_delv=" + delivery.Id);

            foreach (var pc in PettyCashObject.PettyCash) {
                pc.Delivery = delivery;
                pc.AddedBy = delivery.AddedBy;
                pc.Save();
            }

            return delivery;
        }

        public PettyCash SavePettyCash(PettyCash pc) {
            SqlServerConnection conn = new SqlServerConnection();
            pc.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + pc.Id + ", @delv INT=" + pc.Delivery.Id + ", @rcpt NVARCHAR(25)='" + pc.Receipt + "', @vouc NVARCHAR(25)='" + pc.Voucher + "', @supp NVARCHAR(250)='" + pc.Supplier + "', @acnt NVARCHAR(MAX)='" + pc.Account + "', @desc NVARCHAR(MAX)='" + pc.Description + "', @amts FLOAT=" + pc.Amount + ", @user INT=" + pc.AddedBy.Id + "; IF NOT EXISTS (SELECT pc_idnt FROM DeliveryPettyCash WHERE pc_idnt=@idnt) BEGIN INSERT INTO DeliveryPettyCash (pc_delv, pc_voucher, pc_receipt, pc_supplier, pc_account, pc_description, pc_amount, pc_added_by) output INSERTED.pc_idnt VALUES (@delv, @vouc, @rcpt, @supp, @acnt, @desc, @amts, @user) END ELSE BEGIN UPDATE DeliveryPettyCash SET pc_delv=@delv, pc_voucher=@vouc, pc_receipt=@rcpt, pc_supplier=@supp, pc_account=@acnt, pc_description=@desc, pc_amount=@amts output INSERTED.pc_idnt WHERE pc_idnt=@idnt END");

            return pc;
        }

        public CustomersPayments SaveCustomersPayments(CustomersPayments pt) {
            SqlServerConnection conn = new SqlServerConnection();
            pt.Id = conn.SqlServerUpdate("USE " + pt.Customer.Station.Prefix.Replace(".dbo.", "") + "; DECLARE @idnt INT=" + pt.Id + ", @cust INT=" + pt.Customer.Id + ", @date DATE='" + pt.PostDate + "', @rcpt INT=" + pt.Receipt + ", @chqs INT=" + pt.Cheque + ", @note NVARCHAR(MAX)='" + pt.Notes + "', @amts FLOAT=" + pt.Amount + ", @user NVARCHAR(50)='" + Username + "'; IF NOT EXISTS (SELECT im_idnt FROM InvoicePayment WHERE im_idnt=@idnt) BEGIN INSERT INTO InvoicePayment (im_date, im_cust, im_paid, im_change, im_tendered, im_user) VALUES(@date, @cust, @amts, @rcpt, @chqs, @user) SELECT @idnt=SCOPE_IDENTITY() END ELSE BEGIN UPDATE InvoicePayment SET im_date=@date, im_cust=@cust, im_paid=@amts, im_change=@rcpt, im_tendered=@chqs WHERE im_idnt=@idnt END; SELECT @idnt");

            this.UpdateMoneyTransactions(pt.Customer.Station, pt.Id, pt.PostDate, 3, (int.Parse(pt.Cheque) == 0 ? 0 : 1), 1, pt.Cheque, pt.Amount, pt.Notes);

            return pt;
        }

        public SuppliersPayment SaveSuppliersPayment(SuppliersPayment pt) {
            SqlServerConnection conn = new SqlServerConnection();
            pt.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + pt.Id + ", @supp INT=" + pt.Supplier.Id + ", @bank INT=" + pt.Bank.Id + ", @date DATE='" + pt.Date + "', @rcpt NVARCHAR(50)='" + pt.Receipt + "', @chqs NVARCHAR(50)='" + pt.Cheque + "', @invs NVARCHAR(250)='" + pt.Invoices + "', @note NVARCHAR(MAX)='" + pt.Description + "', @amts FLOAT=" + pt.Amount + ", @user INT=" + Actor + "; IF NOT EXISTS (SELECT sp_idnt FROM SuppliersPayments WHERE sp_idnt=@idnt) BEGIN INSERT INTO SuppliersPayments (sp_supp, sp_bank, sp_date, sp_rcpt, sp_chqs, sp_invoice, sp_notes, sp_amount, sp_user) output INSERTED.sp_idnt VALUES (@supp, @bank, @date, @rcpt, @chqs, @invs, @note, @amts, @user) END ELSE BEGIN UPDATE SuppliersPayments SET sp_bank=@bank, sp_date=@date, sp_rcpt=@rcpt, sp_chqs=@chqs, sp_invoice=@invs, sp_notes=@note, sp_amount=@amts output INSERTED.sp_idnt WHERE sp_idnt=@idnt END");

            return pt;
        }

        public SuppliersCredits SaveCreditNote(SuppliersCredits note) {
            SqlServerConnection conn = new SqlServerConnection();
            note.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + note.Id + ", @supp INT=" + note.Supplier.Id + ", @type INT=" + note.Type.Id + ", @station INT=" + note.Station.Id + ", @date DATE='" + note.Date + "', @rcpt NVARCHAR(50)='" + note.Receipt + "', @note NVARCHAR(MAX)='" + note.Description + "', @amts FLOAT=" + note.Amount + ", @user INT=" + Actor + "; IF NOT EXISTS (SELECT cn_idnt FROM CreditNotes WHERE cn_idnt=@idnt) BEGIN INSERT INTO CreditNotes (cn_date, cn_supp, cn_type, cn_station, cn_rcpt, cn_amount, cn_added_by, cn_description) output INSERTED.cn_idnt VALUES (@date, @supp, @type, @station, @rcpt, @amts, @user, @note) END ELSE BEGIN UPDATE CreditNotes SET cn_date=@date, cn_supp=@supp, cn_type=@type, cn_station=@station, cn_rcpt=@rcpt, cn_amount=@amts, cn_description=@note output INSERTED.cn_idnt WHERE cn_idnt=@idnt END");

            return note;
        }

        public CustomersWithholding SaveCustomersWithholding(CustomersWithholding wht) {
            SqlServerConnection conn = new SqlServerConnection();
            wht.Id = conn.SqlServerUpdate("USE " + wht.Customer.Station.Prefix.Replace(".dbo.", "") + "; DECLARE @idnt INT=" + wht.Id + ", @cust INT=" + wht.Customer.Id + ", @date DATE='" + wht.Date + "', @type INT=" + wht.Type.Id + ", @rcpt NVARCHAR(50)='" + wht.Receipt + "', @invs NVARCHAR(250)='" + wht.Invoice + "', @note NVARCHAR(MAX)='" + wht.Description + "', @amts FLOAT=" + wht.Amount + ", @user NVARCHAR(50)='" + Username + "'; IF NOT EXISTS (SELECT cw_idnt FROM CustomersWithholding WHERE cw_idnt=@idnt) BEGIN INSERT INTO CustomersWithholding (cw_cust, cw_type, cw_date, cw_receipt, cw_invoice, cw_amount, cw_added_by, cw_description) VALUES (@cust, @type, @date, @rcpt, @invs, @amts, @user, @note) SELECT @idnt=SCOPE_IDENTITY() END ELSE BEGIN UPDATE CustomersWithholding SET cw_cust=@cust, cw_type=@type, cw_date=@date, cw_receipt=@rcpt, cw_invoice=@invs, cw_amount=@amts, cw_description=@note WHERE cw_idnt=@idnt END; SELECT @idnt");

            return wht;
        }

        public SuppliersWithholding SaveSuppliersWithholding(SuppliersWithholding wht) {
            SqlServerConnection conn = new SqlServerConnection();
            wht.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + wht.Id + ", @supp INT=" + wht.Supplier.Id + ", @bank INT=" + wht.Bank.Id + ", @date DATE='" + wht.Date + "', @type INT=" + wht.Type.Id + ", @rcpt NVARCHAR(50)='" + wht.Receipt + "', @chqs NVARCHAR(50)='" + wht.Cheque + "', @invs NVARCHAR(250)='" + wht.Invoice + "', @note NVARCHAR(MAX)='" + wht.Description + "', @amts FLOAT=" + wht.Amount + ", @user INT=" + Actor + "; IF NOT EXISTS (SELECT sw_idnt FROM SuppliersWithholding WHERE sw_idnt=@idnt) BEGIN INSERT INTO SuppliersWithholding (sw_supp, sw_type, sw_bank, sw_date, sw_receipt, sw_invoice, sw_cheque, sw_amount, sw_added_by, sw_description) output INSERTED.sw_idnt VALUES (@supp, @type, @bank, @date, @rcpt, @invs, @chqs, @amts, @user, @note) END ELSE BEGIN UPDATE SuppliersWithholding SET sw_supp=@supp, sw_typw=@type, sw_bank=@bank, sw_date=@date, sw_receipt=@rcpt, sw_invoice=@invs, sw_cheque=@chqs, sw_amount=@amts, sw_description=@note output INSERTED.sw_idnt WHERE sw_idnt=@idnt END");

            return wht;
        }

        //Updates
        public void UpdateCustomerBalance(Customers customer = null, Stations station = null) {
            string query = "";

            if (customer == null && station == null)
                return;
            if (customer != null) {
                station = customer.Station;
                query = "WHERE Custid = " + customer.Id;
            }

            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("USE " + station.Prefix.Replace(".dbo.", "") + ";UPDATE Customers SET AccountBalance=ISNULL(BALS,0) FROM Customers LEFT OUTER JOIN pCustomersBalances_1 ON Custid=CUST " + query);

        }

        public void UpdateSupplierBalance(Suppliers supplier = null) {
            string query = "";
            if (supplier != null)
                query = "WHERE sp_idnt=" + supplier.Id;
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Suppliers SET sp_balance=ISNULL(sp_bal,0) FROM Suppliers LEFT OUTER JOIN (SELECT sp_supp, SUM(sp_amts)sp_bal FROM vSuppliersBalances GROUP BY sp_supp) As Bals ON sp_supp=sp_idnt " + query);
        }

        public long UpdateMoneyTransactions(Stations station, long idnt, DateTime date, int source, int action, int account, string dtls, double amount, string notes) {
            SqlServerConnection conn = new SqlServerConnection();
            return conn.SqlServerUpdate("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @output INT=0, @idnt INT=" + idnt + ", @date DATE='" + date + "', @source INT=" + source + ", @action INT=" + action + ", @account INT=" + account + ", @dtls NVARCHAR(MAX)='" + dtls + "', @note NVARCHAR(MAX)='" + notes + "', @amts FLOAT=" + amount + "; IF NOT EXISTS (SELECT TOP(1)mm_idnt FROM MoneyTransactions WHERE mm_source=@source AND mm_transaction=@idnt) BEGIN INSERT INTO MoneyTransactions (mm_date, mm_source, mm_action, mm_account, mm_transaction, mm_transdetails, mm_amount, mm_notes) VALUES (@date, @source, @action, @account, @idnt, @dtls, @amts, @note) SELECT @output=SCOPE_IDENTITY() END ELSE BEGIN UPDATE MoneyTransactions SET mm_date=@date, mm_action=@action, mm_account=@account, mm_transdetails=@dtls, mm_amount=@amts, mm_notes=@note WHERE mm_source=@source AND mm_transaction=@idnt SELECT @output=mm_idnt FROM MoneyTransactions WHERE mm_source=@source AND mm_transaction=@idnt END SELECT @output");
        }


        //Deletes
        public void DeleteDelivery(Delivery delivery) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM Delivery WHERE dlv_idnt=" + delivery.Id);
            
            conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM DeliveryPettyCash WHERE pc_delv=" + delivery.Id);
        }

        public void DeleteCustomersPayment(CustomersPayments payment) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("USE " + payment.Station.Prefix.Replace(".dbo.", "") + "; DELETE FROM InvoicePayment WHERE im_idnt=" + payment.Id);

            this.DeleteMoneyTransactions(payment.Station, payment.Id, 3);
        }

        public void DeleteSuppliersPayment(SuppliersPayment payment) {
            new SqlServerConnection().SqlServerUpdate("DELETE FROM SuppliersPayments WHERE sp_idnt=" + payment.Id);
        }
 
        public void DeleteCustomersWithholding(CustomersWithholding wht) {
            new SqlServerConnection().SqlServerUpdate("USE " + wht.Customer.Station.Prefix.Replace(".dbo.", "") + "; DELETE FROM CustomersWithholding WHERE cw_idnt=" + wht.Id);
        }

        public void DeleteSuppliersWithholding(SuppliersWithholding wht) {
            new SqlServerConnection().SqlServerUpdate("DELETE FROM SuppliersWithholding WHERE sw_idnt=" + wht.Id);
        }

        public void DeleteSuppliersCredit(SuppliersCredits credit) {
            new SqlServerConnection().SqlServerUpdate("DELETE FROM CreditNotes WHERE cn_idnt=" + credit.Id);
        }

        public void DeleteSuppliersInvoice(StationsExpenses invoice) {
            new SqlServerConnection().SqlServerUpdate("DELETE FROM Expenses WHERE xp_idnt=" + invoice.Id);
        }

        public void DeleteTrucksFuelExpense(TrucksFuelExpense expense) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM TrucksFuel WHERE tf_idnt=" + expense.Id);
        }

        public void DeleteMoneyTransactions(Stations station, long idnt, int source) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("USE " + station.Prefix.Replace(".dbo.", "") + "; DELETE FROM MoneyTransactions WHERE mm_source=" + source + " AND mm_transaction=" + idnt);
        }
    }
}
