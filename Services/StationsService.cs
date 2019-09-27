using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Models;
using Core.Extensions;
using Core.ViewModel;
using Core.ReportModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.DataModel;

namespace Core.Services
{
    public class StationsService
    {
        private readonly CoreService Core = new CoreService();

        public Stations GetStation(string code) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_void, st_code, st_name, st_database, push_date FROM Stations INNER JOIN vLastPush ON push_station=st_idnt WHERE st_code='" + code + "'");
            if (dr.Read()) {
                return new Stations {
                    Id = Convert.ToInt64(dr[0]),
                    Void = Convert.ToBoolean(dr[1]),
                    Code = dr[2].ToString(),
                    Name = dr[3].ToString(),
                    Prefix = dr[4].ToString(),
                    Push = Convert.ToDateTime(dr[5])
                };
            }

            return null;
        }

        public List<Stations> GetPendingPush() {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_code, st_name, push_date FROM vLastPush INNER JOIN Stations ON push_station=st_idnt WHERE st_void=0 AND push_date<CAST(DATEADD(DAY, -1, GETDATE())AS DATE) ORDER BY push_date DESC, st_order, st_name");
            if (dr.HasRows) {
                while (dr.Read()) {
                    stations.Add(new Stations {
                        Code = dr[0].ToString(),
                        Name = dr[1].ToString(),
                        Push = Convert.ToDateTime(dr[2]).AddDays(1)
                    });
                }
            }

            return stations;
        }

        public List<Stations> GetUpdatedPush() {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_code, st_name FROM vLastPush INNER JOIN Stations ON push_station=st_idnt WHERE st_void=0 AND push_date>=CAST(DATEADD(DAY, -1, GETDATE())AS DATE) ORDER BY push_date DESC, st_order, st_name");
            if (dr.HasRows) {
                while (dr.Read()) {
                    stations.Add(new Stations {
                        Code = dr[0].ToString(),
                        Name = dr[1].ToString()
                    });
                }
            }

            return stations;
        }

        public List<Stations> GetStations(){
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_code, st_name, sb_idnt, sb_brand, push_date, pcol_ltrs, pcol_amts FROM vLastPush INNER JOIN Stations ON push_station=st_idnt INNER JOIN StationsBrand ON st_brand=sb_idnt INNER JOIN vDailySales ON push_station=pcol_station AND push_date=pcol_date ORDER BY push_date DESC, st_order");
            if (dr.HasRows) {
                while (dr.Read()) {
                    stations.Add(new Stations {
                        Id = Convert.ToInt64(dr[0]),
                        Code = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Brand = new StationsBrand {
                            Id = Convert.ToInt64(dr[3]),
                            Name = dr[4].ToString()
                        },
                        Push = Convert.ToDateTime(dr[5]),
                        FuelLtrs = Convert.ToDouble(dr[6]),
                        FuelSales = Convert.ToDouble(dr[7])
                    });
                }
            }

            return stations;
        }

        public List<Stations> GetStationsNames() {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_code, st_name, st_database FROM Stations ORDER BY st_name");
            if (dr.HasRows) {
                while (dr.Read()) {
                    stations.Add(new Stations {
                        Id = Convert.ToInt64(dr[0]),
                        Code = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Prefix = dr[3].ToString(),
                    });
                }
            }

            return stations;
        }

        public List<SelectListItem> GetStationIdntsIEnumerable(string query = "") {
            return Core.GetIEnumerable("SELECT st_idnt, st_name FROM Stations " + query + " ORDER BY st_void, st_name");
        }

        public List<SelectListItem> GetStationCodesIEnumerable(string query = "") {
            return Core.GetIEnumerable("SELECT st_code, st_name FROM Stations " + query + " ORDER BY st_void, st_name");
        }

        public List<PumpReadings> GetMetreReadings(Stations st, DateTime date) {
            List<PumpReadings> readings = new List<PumpReadings>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pcol_idnt, pcol_pump, pmp_name, pcol_price, pcol_op, pcol_adjust, pcol_test, pcol_cl FROM vPumps LEFT OUTER JOIN vReadings ON pmp_st=pcol_st AND pmp_idnt=pcol_pump WHERE pmp_st=" + st.Id + " AND pcol_date='" + date.Date + "'");
            if (dr.HasRows) {
                while (dr.Read()) {
                    readings.Add(new PumpReadings {
                        Id = Convert.ToInt64(dr[0]),
                        Pump = new Pump {
                            Id = Convert.ToInt64(dr[1]),
                            Name = dr[2].ToString()
                        },
                        Price = Convert.ToDouble(dr[3]),
                        Opening = Convert.ToDouble(dr[4]),
                        Adjustment = Convert.ToDouble(dr[5]),
                        Tests = Convert.ToDouble(dr[6]),
                        Closing = Convert.ToDouble(dr[7])
                    });
                }
            }

            return readings;
        }

        public List<TankSummary> GetSummaries(Stations st, DateTime date){
            List<TankSummary> summaries = new List<TankSummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date DATE='" + date.Date + "', @stn INT=" + st.Id + "; SELECT tnk_idnt, tnk_name, tnk_capacity, tnk_fuel, ISNULL(pcol_rtns,0) RTNS,  ISNULL(pcol_sales,0) SALES, ISNULL(dd.delv,0) DELVS, ISNULL(td.reads,0) DIPS, ISNULL(op.ttd_read,0) OP FROM vTanks LEFT OUTER JOIN (SELECT ttd_st, tdd_tank, td_reading ttd_read FROM (SELECT td_st ttd_st, td_tank tdd_tank, MAX(td_date) tdd_date FROM vTanksDips WHERE td_date<@date GROUP BY td_st, td_tank) AS z1 INNER JOIN vTanksDips ON ttd_st=td_st AND tdd_tank=td_tank AND td_date=tdd_date) As op ON tnk_st=ttd_st AND tdd_tank=tnk_idnt LEFT OUTER JOIN vTanksSales ON tnk_st=pcol_st AND tnk_idnt=pcol_tank AND pcol_date=@date LEFT OUTER JOIN (SELECT fr_st, fr_tank, SUM(fr_qnty) delv FROM vTanksDelv WHERE fr_date=@date GROUP BY fr_st, fr_tank) As dd ON fr_st=tnk_st AND fr_tank=tnk_idnt LEFT OUTER JOIN (SELECT td_st, td_tank, SUM(td_reading) reads FROM vTanksDips WHERE td_date=@date GROUP BY td_st, td_tank) As td ON td_st=tnk_st AND td_tank=tnk_idnt WHERE tnk_st=@stn ORDER BY tnk_idnt");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    TankSummary summary = new TankSummary();
                    summary.Tank.Id = Convert.ToInt64(dr[0]);
                    summary.Tank.Name = dr[1].ToString();
                    summary.Tank.Capacity = Convert.ToDouble(dr[2]);
                    summary.Fuel.Id = Convert.ToInt64(dr[3]);

                    summary.Returns = Convert.ToDouble(dr[4]);
                    summary.Sales = Convert.ToDouble(dr[5]);
                    summary.Delivery = Convert.ToDouble(dr[6]);
                    summary.Dips = Convert.ToDouble(dr[7]);
                    summary.Opening = Convert.ToDouble(dr[8]);
                    summary.Closing = summary.Opening + summary.Delivery + summary.Returns - summary.Sales;
                    summary.Variance = summary.Closing - summary.Dips;

                    summaries.Add(summary);
                }
            }

            return summaries;
        }

        public List<LegderSummary> GetCustomersSummaries(Stations st, DateTime date){
            List<LegderSummary> summaries = new List<LegderSummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT mm_account, mm_acname, mm_type, mm_action, mm_amount, mm_lube, ISNULL(mm_disc,0) mm_disc FROM vLedgerSummary WHERE mm_st=" + st.Id + " AND mm_date='" + date.Date + "'");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    LegderSummary summary = new LegderSummary();
                    summary.Customer.Id = Convert.ToInt64(dr[0]);
                    summary.Customer.Name = dr[1].ToString();
                    summary.Type = Convert.ToInt64(dr[2]);
                    summary.Action = Convert.ToInt64(dr[3]);
                    summary.FuelSales = Convert.ToDouble(dr[4]);
                    summary.LubeSales = Convert.ToDouble(dr[5]);
                    summary.Discounts = Convert.ToDouble(dr[6]);

                    summaries.Add(summary);
                }
            }

            return summaries;
        }

        public List<ReportCustomerYearly> GetCustomerYearlyReport(Int64 year, Int64 stid, String type){
            List<ReportCustomerYearly> entries = new List<ReportCustomerYearly>();
            String additionalQuery = "";

            if (type.Equals("invoices")){
                additionalQuery = "AND Srcs=1";
            }
            else if (type.Equals("payments")){
                additionalQuery = "AND Srcs=2";
            }


            SqlServerConnection conn = new SqlServerConnection();
            //SqlDataReader dr = conn.SqlServerConnect("DECLARE @yr INT=" + year + ", @st INT=" + stid + "; SELECT Cust, Names, SUM(CASE WHEN Mnths=0 THEN Amts ELSE 0 END) OPN, SUM(CASE WHEN Mnths=1 THEN Amts ELSE 0 END) JAN, SUM(CASE WHEN Mnths=2 THEN Amts ELSE 0 END) FEB, SUM(CASE WHEN Mnths=3 THEN Amts ELSE 0 END) MAR, SUM(CASE WHEN Mnths=4 THEN Amts ELSE 0 END) APR, SUM(CASE WHEN Mnths=5 THEN Amts ELSE 0 END) MAY, SUM(CASE WHEN Mnths=6 THEN Amts ELSE 0 END) JUN, SUM(CASE WHEN Mnths=7 THEN Amts ELSE 0 END) JUL, SUM(CASE WHEN Mnths=8 THEN Amts ELSE 0 END) AUG, SUM(CASE WHEN Mnths=9 THEN Amts ELSE 0 END) SEP, SUM(CASE WHEN Mnths=10 THEN Amts ELSE 0 END) OCT, SUM(CASE WHEN Mnths=11 THEN Amts ELSE 0 END) NOV, SUM(CASE WHEN Mnths=12 THEN Amts ELSE 0 END) DEC, SUM(Amts) TTL FROM (SELECT Stns, Cust, MONTH(Dates) Mnths, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)=@yr " + additionalQuery + " UNION ALL SELECT Stns, Cust, 0, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)<@yr) As Foo INNER JOIN vCustomers ON [Custid]=[Cust] AND [Stns]=[Sts] GROUP BY Stns, Names, Cust ORDER BY Names");
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @yr INT=" + year + ", @st INT=" + stid + "; SELECT [Type], Cust, CASE WHEN [Type]=4 THEN 'LIPA NA MPESA' WHEN [Type]=3 THEN BankName+(CASE WHEN BankName IN ('EQUITY','CFC') THEN ' VISA' ELSE '' END) ELSE Names END Names, SUM(CASE WHEN Mnths=0 THEN Amts ELSE 0 END) OPN, SUM(CASE WHEN Mnths=1 THEN Amts ELSE 0 END) JAN, SUM(CASE WHEN Mnths=2 THEN Amts ELSE 0 END) FEB, SUM(CASE WHEN Mnths=3 THEN Amts ELSE 0 END) MAR, SUM(CASE WHEN Mnths=4 THEN Amts ELSE 0 END) APR, SUM(CASE WHEN Mnths=5 THEN Amts ELSE 0 END) MAY, SUM(CASE WHEN Mnths=6 THEN Amts ELSE 0 END) JUN, SUM(CASE WHEN Mnths=7 THEN Amts ELSE 0 END) JUL, SUM(CASE WHEN Mnths=8 THEN Amts ELSE 0 END) AUG, SUM(CASE WHEN Mnths=9 THEN Amts ELSE 0 END) SEP, SUM(CASE WHEN Mnths=10 THEN Amts ELSE 0 END) OCT, SUM(CASE WHEN Mnths=11 THEN Amts ELSE 0 END) NOV, SUM(CASE WHEN Mnths=12 THEN Amts ELSE 0 END) DEC, SUM(Amts) TTL FROM (SELECT Stns, 0 [Type], Cust, MONTH(Dates) Mnths, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)=@yr " + additionalQuery + " UNION ALL SELECT Stns, 0, Cust, 0, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)<@yr UNION ALL SELECT mm_st, mm_type, mm_account, MONTH(mm_date)mm_mnth, (mm_invs-mm_pymt)mm_amts FROM vAccountsStatements WHERE mm_st=@st AND YEAR(mm_date)=@yr " + additionalQuery + " UNION ALL SELECT mm_st, mm_type, mm_account, 0, (mm_invs-mm_pymt)mm_amts FROM vAccountsStatements WHERE mm_st=@st AND YEAR(mm_date)<@yr) As Foo LEFT OUTER JOIN vCustomers ON [Custid]=[Cust] AND [Stns]=[Sts] AND [Type]=0 LEFT OUTER JOIN vBankAccounts ON BankID=Cust AND BankSt=Stns GROUP BY Stns, Names, BankName, [Type], Cust ORDER BY Names");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ReportCustomerYearly entry = new ReportCustomerYearly();
                    entry.Customer.Id = Convert.ToInt64(dr[1]);
                    entry.Customer.Name = dr[2].ToString();
                    entry.Opening = Convert.ToDouble(dr[3]);
                    entry.Jan = Convert.ToDouble(dr[4]);
                    entry.Feb = Convert.ToDouble(dr[5]);
                    entry.Mar = Convert.ToDouble(dr[6]);
                    entry.Apr = Convert.ToDouble(dr[7]);
                    entry.May = Convert.ToDouble(dr[8]);
                    entry.Jun = Convert.ToDouble(dr[9]);
                    entry.Jul = Convert.ToDouble(dr[10]);
                    entry.Aug = Convert.ToDouble(dr[11]);
                    entry.Sep = Convert.ToDouble(dr[12]);
                    entry.Oct = Convert.ToDouble(dr[13]);
                    entry.Nov = Convert.ToDouble(dr[14]);
                    entry.Dec = Convert.ToDouble(dr[15]);
                    entry.Total = Convert.ToDouble(dr[16]);
                    entry.Closing = entry.Opening + entry.Total;

                    entries.Add(entry);
                }
            }

            return entries;
            
        }

        public LedgerTotals GetLedgerTotals(Stations st, DateTime date){
            LedgerTotals total = new LedgerTotals();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date DATE='" + date.Date + "', @st INT=" + st.Id +"; SELECT st_idnt, ISNULL(ar_cash,0)cs, ISNULL(mm_visa,0)vc, ISNULL(mm_mpesa,0)mp, ISNULL(mm_pos,0)ps, ISNULL(ex_amts,0)xp, ISNULL(ar_cwash,0)cw, ISNULL(ar_tyre,0)tc, ISNULL(ar_service,0)sv FROM Stations LEFT OUTER JOIN vLedgerAccounts ON st_idnt=mm_st AND mm_date=@date LEFT OUTER JOIN (SELECT ar_st, ar_cash, ar_cwash, ar_tyre, ar_service FROM vLedgerCash WHERE ar_date=@date) As Cash ON ar_st=st_idnt LEFT OUTER JOIN (SELECT ex_st, ex_amts FROM vLedgerExpenses WHERE ex_date=@date) As exps ON ex_st=st_idnt WHERE st_idnt=@st");
            if (dr.Read())
            {
                total.Station.Id = Convert.ToInt64(dr[0]);
                total.Date = date;
                total.Cash = Convert.ToDouble(dr[1]);
                total.Visa = Convert.ToDouble(dr[2]);
                total.Mpesa = Convert.ToDouble(dr[3]);
                total.POS = Convert.ToDouble(dr[4]);
                total.Expense = Convert.ToDouble(dr[5]);

                total.Account = total.Visa + total.Mpesa + total.POS;
                total.Summary = total.Account + total.Cash + total.Expense;

                total.CarWash = Convert.ToDouble(dr[6]);
                total.TyreCtr = Convert.ToDouble(dr[7]);
                total.Service = Convert.ToDouble(dr[8]);
            }

            return total;
        }

        public List<LedgerTotals> GetLedgerTotals(string stations, DateTime date1, DateTime date2){
            List<LedgerTotals> totals = new List<LedgerTotals>();
            String additionalquery = "";

            if (!string.IsNullOrEmpty(stations.Trim()))
            {
                additionalquery = "WHERE st_name IN (" + stations + ")";
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + date1.Date + "', @stop DATE='" + date2.Date + "'; SELECT st_idnt, st_code, st_name, ISNULL(SUM(SALE),0)SALE, ISNULL(SUM(CASH),0)CASH, ISNULL(SUM(INVS),0)INVS, ISNULL(SUM(VISA),0)VISA, ISNULL(SUM(MPESA),0)MPESA, ISNULL(SUM(POS),0)POS, ISNULL(SUM(EXPS),0)EXPS, ISNULL(SUM(DISC),0)DISC FROM Stations LEFT OUTER JOIN (SELECT pcol_station ST, pcol_amts SALE, 0 CASH, 0 INVS, 0 VISA, 0 MPESA, 0 POS, 0 EXPS, 0 DISC FROM vDailySales WHERE pcol_date BETWEEN @start AND @stop UNION ALL SELECT ar_st, 0, ar_cash, 0,0,0,0,0,0 FROM vLedgerCash WHERE ar_date BETWEEN @start AND @stop UNION ALL SELECT sr_st, 0,0, sr_amts, 0,0,0,0, ABS(sr_disc) FROM vLedgerInvoices WHERE sr_date BETWEEN @start AND @stop UNION ALL SELECT mm_st, 0,0,0, mm_visa, mm_mpesa, mm_pos,0,0 FROM vLedgerAccounts WHERE mm_date BETWEEN @start AND @stop UNION ALL SELECT ex_st, 0,0,0, 0,0,0,ex_amts,0 FROM vLedgerExpenses WHERE ex_date BETWEEN @start AND @stop) As Ledger ON ST=st_idnt " + additionalquery + " GROUP BY st_idnt, st_code, st_order, st_name ORDER BY st_order");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    LedgerTotals total = new LedgerTotals();
                    total.Station.Id = Convert.ToInt64(dr[0]);
                    total.Station.Code = dr[1].ToString().ToLower();
                    total.Station.Name = dr[2].ToString().ToUpper();

                    total.Sale = Convert.ToDouble(dr[3]);
                    total.Cash = Convert.ToDouble(dr[4]);
                    total.Invoice = Convert.ToDouble(dr[5]);
                    total.Visa = Convert.ToDouble(dr[6]);
                    total.Mpesa = Convert.ToDouble(dr[7]);
                    total.POS = Convert.ToDouble(dr[8]);
                    total.Expense = Convert.ToDouble(dr[9]);
                    total.Discount = Convert.ToDouble(dr[10]);

                    total.Account = total.Visa + total.Mpesa + total.POS;
                    total.Summary = total.Cash + total.Invoice + total.Discount + total.Expense + total.Account;

                    totals.Add(total);
                }
            }

            return totals;
        }

        public List<StationsReconcile> GetStationsReconciles(Int64 stid, Int64 year, Int64 mnth){
            List<StationsReconcile> reconciles = new List<StationsReconcile>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + stid + ", @year INT=" + year + ", @mnth INT=" + mnth + "; SELECT Dates, SUM(Amts)Amts, SUM(Pd)Pd, SUM(Uprna)Uprna, SUM(Debt)Debt, SUM(Disc)Disc, SUM(Transp)Transp FROM ( SELECT pcol_date Dates, pcol_amts Amts, 0 Pd, 0 Uprna, 0 Debt, 0 Disc, 0 Transp FROM vDailySales WHERE pcol_station=@st AND YEAR(pcol_date)=@year AND MONTH(pcol_date)=@mnth UNION ALL SELECT ar_date, 0, ar_cash,0,0,0,0 FROM vLedgerCash WHERE ar_st=@st AND YEAR(ar_date)=@year AND MONTH(ar_date)=@mnth UNION ALL SELECT sr_date, CASE sr_overpump when 1 THEN sr_amts-(sr_discount) ELSE 0 END, 0,0,sr_amts, sr_discount,0 FROM vInvoicesLedger WHERE sr_st=@st AND YEAR(sr_date)=@year AND MONTH(sr_date)=@mnth UNION ALL SELECT am_date, 0,0,0,am_amts,0,0 FROM vAccounts WHERE am_st=@st AND YEAR(am_date)=@year AND MONTH(am_date)=@mnth UNION ALL SELECT ex_date, 0,ex_amts,0,0,0,0 FROM vLedgerExpenses WHERE ex_st=@st AND YEAR(ex_date)=@year AND MONTH(ex_date)=@mnth UNION ALL SELECT pt_date,0,0,0,0,0, pt_amount FROM vTransport WHERE pt_st=@st AND YEAR(pt_date)=@year AND MONTH(pt_date)=@mnth UNION ALL SELECT cd_date, 0,0,0,0, cd_disc, 0 FROM vDiscounts WHERE cd_st=@st AND YEAR(cd_date)=@year AND MONTH(cd_date)=@mnth) As Foo GROUP BY Dates ORDER BY Dates");
            if (dr.HasRows) {
                while (dr.Read()) {
                    StationsReconcile recon = new StationsReconcile {
                        Date = Convert.ToDateTime(dr[0]).ToString("dd/MM/yyyy"),
                        Amount = Convert.ToDouble(dr[1]),
                        Payment = Convert.ToDouble(dr[2]),
                        Uprna = Convert.ToDouble(dr[3]),
                        Debt = Convert.ToDouble(dr[4]),
                        Discount = Convert.ToDouble(dr[5]),
                        Transport = Convert.ToDouble(dr[6])
                    };

                    recon.Balance = recon.Amount - recon.Payment - recon.Uprna - recon.Debt - recon.Transport - recon.Discount;
                    reconciles.Add(recon);
                }
            }

            return reconciles;
        }

        public List<LedgerEntries> GetLedgerEntries(Int64 stid, DateTime start, DateTime stop, String filter, Int64 custid = 0){
            List<LedgerEntries> entries = new List<LedgerEntries>();
            String AdditionalString = "";

            if (custid != 0){
                AdditionalString = " AND mm_account=" + custid;
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT mm_idnt, mm_action, mm_account, mm_date, mm_desc, am_lpos, am_invs, mm_name, mm_price, mm_amount FROM vLedgerzEntry " + conn.GetQueryString(filter, "mm_desc+'-'+am_lpos+'-'+am_invs+'-'+mm_name+'-'+CAST(mm_amount AS NVARCHAR)+'-'+CAST(mm_price AS NVARCHAR)", "mm_st=" + stid + " AND mm_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + AdditionalString + " ORDER BY mm_date, am_invs, am_lpos, mm_desc");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    LedgerEntries entry = new LedgerEntries();
                    entry.Station.Id = stid;
                    entry.Id = Convert.ToInt64(dr[0]);
                    entry.Action = Convert.ToInt64(dr[1]);
                    entry.Account = Convert.ToInt64(dr[2]);
                    entry.Date = Convert.ToDateTime(dr[3]).ToString("dd/MM/yyyy");

                    entry.Description = dr[4].ToString();
                    entry.Lpo = dr[5].ToString();
                    entry.Invoice = dr[6].ToString();
                    entry.Name = dr[7].ToString();

                    entry.Price = Convert.ToDouble(dr[8]);
                    entry.Amount = Convert.ToDouble(dr[9]);
                    entry.Quantity = (entry.Amount / entry.Price);

                    entries.Add(entry);
                }
            }

            return entries;
        }

        public List<LedgerEntries> GetLedgerDuplicates(DateTime start, DateTime stop){
            List<LedgerEntries> entries = new List<LedgerEntries>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date1 DATE='" + start.Date + "', @date2 DATE='" + stop.Date + "'; SELECT TOP(200) sr_idnt, sr_date, CASE sr_fuel WHEN 1 THEN 'LTS DIESEL' WHEN 2 THEN 'LTS SUPER' WHEN 3 THEN 'LTS VPOWER' WHEN 4 THEN 'LTS KEROSENE' ELSE 'OTHERS' END xdesc, sr_lpo, sr_invoice, sr_price, sr_amts, sr_cust, Names, sr_st, st_code, st_name FROM ( SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_invoice IN ( SELECT DISTINCT invs FROM vInvoicesDuplicates INNER JOIN vInvoicesLedger ON invs=sr_invoice WHERE sr_date BETWEEN @date1 AND @date2) UNION ALL SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_date BETWEEN @date1 AND @date2 AND sr_invoice=0 ) As Foo INNER JOIN Stations ON st_idnt=sr_st INNER JOIN vCustomers ON sr_cust=Custid AND Sts=st_idnt ORDER BY sr_invoice, sr_date, sr_fuel");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    LedgerEntries entry = new LedgerEntries
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),

                        Description = dr[2].ToString(),
                        Lpo = dr[3].ToString(),
                        Invoice = dr[4].ToString(),

                        Price = Convert.ToDouble(dr[5]),
                        Amount = Convert.ToDouble(dr[6])
                    };
                    entry.Quantity = (entry.Amount / entry.Price);

                    entry.Customer.Id = Convert.ToInt64(dr[7]);
                    entry.Customer.Name = dr[8].ToString();

                    entry.Name = entry.Customer.Name;

                    entry.Station.Id = Convert.ToInt64(dr[9]);
                    entry.Station.Code = dr[10].ToString();
                    entry.Station.Name = dr[11].ToString();

                    entries.Add(entry);
                }
            }

            return entries;
        }

        public List<LedgerEntries> GetLedgerEntriesDuplicates(DateTime start, DateTime stop){
            List<LedgerEntries> entries = new List<LedgerEntries>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date1 DATE='" + start.Date + "', @date2 DATE='" + stop.Date + "'; SELECT sr_idnt, sr_date, CASE sr_fuel WHEN 1 THEN 'LTS DIESEL' WHEN 2 THEN 'LTS SUPER' WHEN 3 THEN 'LTS VPOWER' WHEN 4 THEN 'LTS KEROSENE' ELSE 'OTHERS' END xdesc, sr_lpo, sr_invoice, sr_price, sr_amts, sr_cust, Names, sr_st, st_code, st_name FROM ( SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_invoice IN ( SELECT DISTINCT invs FROM vInvoicesDuplicates INNER JOIN vInvoicesLedger ON invs=sr_invoice WHERE sr_date BETWEEN @date1 AND @date2) UNION ALL SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_date BETWEEN @date1 AND @date2 AND sr_invoice=0 ) As Foo INNER JOIN Stations ON st_idnt=sr_st INNER JOIN vCustomers ON sr_cust=Custid ORDER BY sr_invoice, sr_date, sr_fuel");
            if (dr.HasRows) {
                while (dr.Read()){
                    LedgerEntries entry = new LedgerEntries();
                    entry.Id = Convert.ToInt64(dr[0]);
                    entry.Date = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy");
                    entry.Description = dr[2].ToString();
                    entry.Lpo = dr[3].ToString();
                    entry.Invoice = dr[4].ToString();
                    entry.Price = Convert.ToDouble(dr[5]);
                    entry.Amount = Convert.ToDouble(dr[6]);
                    entry.Quantity = (entry.Amount / entry.Price);

                    entry.Customer.Id = Convert.ToInt64(dr[7]);
                    entry.Customer.Name = dr[8].ToString();

                    entry.Station.Id = Convert.ToInt64(dr[9]);
                    entry.Station.Code = dr[10].ToString();
                    entry.Station.Name = dr[11].ToString();

                    entries.Add(entry);
                }
            }

            return entries;
        }

        public List<Expenses> GetExpenditure(Int64 stid, DateTime start, DateTime stop, String filter){
            List<Expenses> expenses = new List<Expenses>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ex_ids, ex_date, ex_loc, ex_desc, ex_amount, ex_tsid, SETT_NAME FROM vExpenses INNER JOIN vChartOfAccounts ON ex_st=ST_IDNT AND ex_tsid=LINE_IDNT " + conn.GetQueryString(filter, "ex_loc+'-'+ex_desc+'-'+FILT_NAME+'-'+CAST(ex_amount AS NVARCHAR)", "ex_st=" + stid + " AND ex_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + " ORDER BY ex_date, ex_ids");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Expenses expense = new Expenses();
                    expense.Station.Id = stid;

                    expense.Id = Convert.ToInt64(dr[0]);
                    expense.Date = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy");
                    expense.Location = dr[2].ToString();
                    expense.Description = dr[3].ToString();
                    expense.Amount = Convert.ToDouble(dr[4]);

                    expense.Account.Id = Convert.ToInt64(dr[5]);
                    expense.Account.Name = dr[6].ToString();

                    expenses.Add(expense);
                }
            }

            return expenses;
        }

        public List<PurchasesOthers> GetPurchasesOthers(long stid, DateTime start, DateTime stop, String filter)
        {
            List<PurchasesOthers> ledgers = new List<PurchasesOthers>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + stid + ", @dt1 DATE='" + start.Date + "', @dt2 DATE='" + stop.Date + "'; SELECT PurNo, Date, Category, Lpo, SuppInv, TaxRate, Total, Taxable, Supp, ISNULL(Names,'CASH'), Address, City, Telephone FROM vPurchasesOthers LEFT OUTER JOIN vSuppliers ON Stns=Stn AND Supp=Suppid " + conn.GetQueryString(filter, "Category+'-'+SuppInv+'-'+LpoNo+'-'+ISNULL(Names,'CASH')+'-'+CAST(Total AS NVARCHAR)+'-'+CAST(PurNo AS NVARCHAR)", "Stns=@st and Date BETWEEN @dt1 AND @dt2") + " ORDER BY [Date], SuppInv, PurNo");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ledgers.Add(new PurchasesOthers {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                        Type = dr[2].ToString(),
                        Lpo = dr[3].ToString(),
                        Invoice = dr[4].ToString(),
                        Rate = Convert.ToDouble(dr[5]),
                        Total = Convert.ToDouble(dr[6]),
                        Taxable = Convert.ToDouble(dr[7]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[8]),
                            Name = dr[9].ToString(),
                            Address = dr[10].ToString(),
                            City = dr[11].ToString(),
                            Telephone = dr[12].ToString()
                        }
                    });
                }
            }

            return ledgers;
        }

        public List<PurchasesLedger> GetPurchasesLedger(Int64 stid, DateTime start, DateTime stop, String filter)
        {
            List<PurchasesLedger> ledgers = new List<PurchasesLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + stid + ", @dt1 DATE='" + start.Date + "', @dt2 DATE='" + stop.Date + "'; SELECT id, fl_idnt, fl_name, Supp, ISNULL(Names,'N/A') Supp, lx, date_, SuppInv, qty, price FROM (SELECT 'PURC' lx, id, Supp, item_id, date_, SuppInv, qty, price FROM vFuelPurchasesLedger WHERE pr_st=@st AND date_ BETWEEN @dt1 AND @dt2 UNION ALL SELECT 'DELV' lx, fr_idnt, 0, fr_fuel, fr_date, 'N/A', fr_quantity, 0 FROM vFuelDeliveryLedger WHERE fr_st=@st AND fr_date BETWEEN @dt1 AND @dt2) As Foo INNER JOIN Fuel On fl_idnt=item_id LEFT OUTER JOIN vSuppliers ON Supp=Suppid AND Stn=@st " + conn.GetQueryString(filter, "fl_name+'-'+ISNULL(Names,'N/A')+'-'+lx+'-'+SuppInv+'-'+CAST(qty AS NVARCHAR)+'-'+CAST(qty*price AS NVARCHAR)") + " ORDER BY date_, lx DESC, item_id");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    PurchasesLedger item = new PurchasesLedger();
                    item.Station.Id = stid;

                    item.Id = Convert.ToInt64(dr[0]);
                    item.Fuel.Id = Convert.ToInt64(dr[1]);
                    item.Fuel.Name = dr[2].ToString();
                    item.Supplier.Id = Convert.ToInt64(dr[3]);
                    item.Supplier.Name = dr[4].ToString();
                    item.Type = dr[5].ToString();
                    item.Date = Convert.ToDateTime(dr[6]).ToString("dd/MM/yyyy");
                    item.Invoice = dr[7].ToString();
                    item.Quantity = Convert.ToDouble(dr[8]);
                    item.Price = Convert.ToDouble(dr[9]);
                    item.Total = item.Quantity * item.Price;

                    if (item.Type.Equals("DELV")){
                        item.Delivery = item.Quantity;
                    }
                    else {
                        item.Purchase = item.Quantity;
                    }

                    ledgers.Add(item);
                }
            }

            return ledgers;
        }

        public List<ReportVatBreakdown> GetReportVatBreakdown(Stations station, DateTime start, DateTime stop) {
            List<ReportVatBreakdown> reports = new List<ReportVatBreakdown>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @station INT=" + station.Id + ", @date1 DATE='" + start.Date + "', @date2 DATE='" + stop.Date + "'; SELECT ldg_date, SUM(dx_sale+dx_over)dx_total, SUM(ux_sale+ux_over)ux_total, SUM(vp_sale+vp_over)vp_total, SUM(ik_sale+ik_over)ik_total, SUM(dx_credit) dx_credit, SUM(ux_credit) ux_credit, SUM(vp_credit) vp_credit, SUM(ik_credit) ik_credit, SUM(dx_disc) dx_disc, SUM(ux_disc) ux_disc, SUM(vp_disc) vp_disc, SUM(ik_disc) ik_disc, SUM(ldg_lesses) ldg_lesses, SUM(ldg_transport) ldg_transport, SUM(ldg_cash) ldg_cash, MAX(dx_price) dx_price, MAX(ux_price) ux_price, MAX(vp_price) vp_price, MAX(ik_price) ik_price, MAX(dx_zero) dx_zero, MAX(ux_zero) ux_zero, MAX(vp_zero) vp_zero, MAX(ik_zero) ik_zero, SUM(dx_ltrs) dx_ltrs, SUM(ux_ltrs) ux_ltrs, SUM(vp_ltrs) vp_ltrs, SUM(ik_ltrs) ik_ltrs, SUM(dx_ltrs*dx_rate*(dx_price-dx_zero))+SUM(ux_rate*ux_ltrs*(ux_price-ux_zero))+SUM(vp_rate*vp_ltrs*(vp_price-vp_zero))+SUM(ik_rate*ik_ltrs*(ik_price-ik_zero)) vat_amt, SUM(dx_ltrs*dx_zero)+SUM(ux_ltrs*ux_zero)+SUM(vp_ltrs*vp_zero)+SUM(ik_ltrs*ik_zero) vat_zero, SUM(cr_zero) cr_zero, SUM(dx_credit+Ux_credit+vp_credit+ik_credit) cr_total FROM ( SELECT ldg_date, ldg_station, ldg_item, CASE WHEN ldg_item=1 THEN ldg_sale ELSE 0 END dx_sale, CASE WHEN ldg_item=2 THEN ldg_sale ELSE 0 END ux_sale, CASE WHEN ldg_item=3 THEN ldg_sale ELSE 0 END vp_sale, CASE WHEN ldg_item=4 THEN ldg_sale ELSE 0 END ik_sale, ldg_sale, CASE WHEN ldg_item=1 THEN ldg_over ELSE 0 END dx_over, CASE WHEN ldg_item=2 THEN ldg_over ELSE 0 END ux_over, CASE WHEN ldg_item=3 THEN ldg_over ELSE 0 END vp_over, CASE WHEN ldg_item=4 THEN ldg_over ELSE 0 END ik_over, ldg_over, CASE WHEN ldg_item=1 THEN ldg_credit ELSE 0 END dx_credit, CASE WHEN ldg_item=2 THEN ldg_credit ELSE 0 END ux_credit, CASE WHEN ldg_item=3 THEN ldg_credit ELSE 0 END vp_credit, CASE WHEN ldg_item=4 THEN ldg_credit ELSE 0 END ik_credit, ldg_credit, ldg_price, CASE WHEN ldg_item=1 THEN ldg_disc ELSE 0 END dx_disc, CASE WHEN ldg_item=2 THEN ldg_disc ELSE 0 END ux_disc, CASE WHEN ldg_item=3 THEN ldg_disc ELSE 0 END vp_disc, CASE WHEN ldg_item=4 THEN ldg_disc ELSE 0 END ik_disc, ldg_disc, CASE WHEN ldg_item=1 THEN pp_price ELSE 0 END dx_price, CASE WHEN ldg_item=2 THEN pp_price ELSE 0 END ux_price, CASE WHEN ldg_item=3 THEN pp_price ELSE 0 END vp_price, CASE WHEN ldg_item=4 THEN pp_price ELSE 0 END ik_price, CASE WHEN ldg_item=1 THEN vat_zero ELSE 0 END dx_zero, CASE WHEN ldg_item=2 THEN vat_zero ELSE 0 END ux_zero, CASE WHEN ldg_item=3 THEN vat_zero ELSE 0 END vp_zero, CASE WHEN ldg_item=4 THEN vat_zero ELSE 0 END ik_zero, CASE WHEN ldg_item=1 THEN vat_rate/(vat_rate+100.0) ELSE 0 END dx_rate, CASE WHEN ldg_item=2 THEN vat_rate/(vat_rate+100.0) ELSE 0 END ux_rate, CASE WHEN ldg_item=3 THEN vat_rate/(vat_rate+100.0) ELSE 0 END vp_rate, CASE WHEN ldg_item=4 THEN vat_rate/(vat_rate+100.0) ELSE 0 END ik_rate, CASE WHEN ldg_item=1 THEN (ldg_sale+ldg_over)/pp_price ELSE 0 END dx_ltrs, CASE WHEN ldg_item=2 THEN (ldg_sale+ldg_over)/pp_price ELSE 0 END ux_ltrs, CASE WHEN ldg_item=3 THEN (ldg_sale+ldg_over)/pp_price ELSE 0 END vp_ltrs, CASE WHEN ldg_item=4 THEN (ldg_sale+ldg_over)/pp_price ELSE 0 END ik_ltrs, CASE WHEN ldg_source IN (2,3) THEN (ldg_credit/ldg_price)*(vat_zero) ELSE 0 END cr_zero, ldg_lesses, ldg_transport, ldg_cash FROM vETRLedger INNER JOIN vPumpsPrices ON pp_date=ldg_date AND pp_fuel=ldg_item AND ldg_station=pp_st INNER JOIN VatReports ON vat_date=ldg_date AND ldg_item=vat_item WHERE ldg_date BETWEEN @date1 AND @date2 ) As Foo WHERE ldg_station=@station GROUP BY ldg_station, ldg_date");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ReportVatBreakdown report = new ReportVatBreakdown();

                    report.date = Convert.ToDateTime(dr[0]);

                    report.dx_sales = Convert.ToDouble(dr[1]);
                    report.ux_sales = Convert.ToDouble(dr[2]);
                    report.vp_sales = Convert.ToDouble(dr[3]);
                    report.ik_sales = Convert.ToDouble(dr[4]);
                    report.tt_sales = report.dx_sales + report.ux_sales + report.vp_sales + report.ik_sales;

                    report.dx_credit = Convert.ToDouble(dr[5]);
                    report.ux_credit = Convert.ToDouble(dr[6]);
                    report.vp_credit = Convert.ToDouble(dr[7]);
                    report.ik_credit = Convert.ToDouble(dr[8]);
                    report.tt_credit = report.dx_credit + report.ux_credit + report.vp_credit + report.ik_credit;


                    report.dx_discs = Convert.ToDouble(dr[9]);
                    report.ux_discs = Convert.ToDouble(dr[10]);
                    report.vp_discs = Convert.ToDouble(dr[11]);
                    report.ik_discs = Convert.ToDouble(dr[12]);
                    report.tt_discs = report.dx_discs + report.ux_discs + report.vp_discs + report.ik_discs; 

                    report.dx_lesses = Convert.ToDouble(dr[13]);
                    report.dx_transp = Convert.ToDouble(dr[14]);
                    report.dx_cashes = Convert.ToDouble(dr[15]);

                    report.dx_price = Convert.ToDouble(dr[16]);
                    report.ux_price = Convert.ToDouble(dr[17]);
                    report.vp_price = Convert.ToDouble(dr[18]);
                    report.ik_price = Convert.ToDouble(dr[19]);

                    report.dx_zero = Convert.ToDouble(dr[20]);
                    report.ux_zero = Convert.ToDouble(dr[21]);
                    report.vp_zero = Convert.ToDouble(dr[22]);
                    report.ik_zero = Convert.ToDouble(dr[23]);

                    report.dx_ltrs = Convert.ToDouble(dr[24]);
                    report.ux_ltrs = Convert.ToDouble(dr[25]);
                    report.vp_ltrs = Convert.ToDouble(dr[26]);
                    report.ik_ltrs = Convert.ToDouble(dr[27]);

                    report.vt_vatsx = Convert.ToDouble(dr[28]);
                    report.vt_zeros = Convert.ToDouble(dr[29]);
                    report.vt_total = report.tt_sales;
                    report.vt_vatab = report.tt_sales - report.vt_zeros;

                    report.cr_zeros = Convert.ToDouble(dr[30]);
                    report.cr_total = Convert.ToDouble(dr[31]);
                    report.cr_vatab = report.cr_total - report.cr_zeros;
                    report.cr_vatsx = report.cr_vatab * (8/108);

                    report.dxc_ltrs = (report.dx_sales - report.dx_credit - report.dx_lesses - report.dx_transp + report.dx_discs) / report.dx_price;
                    report.uxc_ltrs = (report.ux_sales - report.ux_credit + report.ux_discs) / report.ux_price;
                    report.vpc_ltrs = (report.vp_sales - report.vp_credit + report.vp_discs) / report.vp_price;
                    report.ikc_ltrs = (report.ik_sales - report.ik_credit + report.ik_discs) / report.ik_price;

                    if (Double.IsNaN(report.dxc_ltrs) || Double.IsInfinity(report.dxc_ltrs))
                        report.dxc_ltrs = 0;
                    if (Double.IsNaN(report.uxc_ltrs) || Double.IsInfinity(report.uxc_ltrs))
                        report.uxc_ltrs = 0;
                    if (Double.IsNaN(report.vpc_ltrs) || Double.IsInfinity(report.vpc_ltrs))
                        report.vpc_ltrs = 0;
                    if (Double.IsNaN(report.ikc_ltrs) || Double.IsInfinity(report.ikc_ltrs))
                        report.ikc_ltrs = 0;

                    report.ca_zeros = (report.dxc_ltrs * report.dx_zero) + (report.uxc_ltrs * report.ux_zero) + (report.vpc_ltrs * report.vp_zero) + (report.ikc_ltrs * report.ik_zero);
                    report.ca_vatab = (report.dxc_ltrs * (report.dx_price - report.dx_zero)) + (report.uxc_ltrs * (report.ux_price - report.ux_zero)) + (report.vpc_ltrs * (report.vp_price - report.vp_zero)) + (report.ikc_ltrs * (report.ik_price - report.ik_zero));
                    report.ca_total = report.ca_zeros + report.ca_vatab;
                    report.ca_vatsx = report.ca_vatab * (8.0 / 108.0);

                    reports.Add(report);
                }
            }

            return reports;
        }

        public CustomersPayments GetCustomerPayment(long idnt, Stations station) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT im_idnt, im_type, im_date, rcpt, imcq, mm_notes, im_paid, st_idnt, st_code, st_name, st_database, im_cust, CASE im_type WHEN 4 THEN 'LIPA NA MPESA' WHEN 3 THEN BankName+(CASE WHEN BankName IN ('CFC', 'KCB', 'EQUITY') THEN ' VISA' ELSE '' END) ELSE ISNULL(Names,'N/A') END NameX FROM (SELECT im_st, im_idnt, im_cust, 0 im_type, im_date, rcpt, imcq, mm_notes, im_paid FROM vCustomersPayment UNION ALL SELECT aw_st, aw_idnt, aw_account, aw_type, aw_date, 88, aw_chqs, aw_notes, aw_amount FROM vAccountsWithdraw) As Foo INNER JOIN Stations ON im_st=st_idnt LEFT OUTER JOIN vBankAccounts ON im_cust=BankID AND im_st=BankSt LEFT OUTER JOIN vCustomers ON im_cust=Custid AND im_st=Sts WHERE st_code='" + station.Code + "' AND im_idnt=" + idnt);
            if (dr.HasRows) {
                while (dr.Read()) {
                    return new CustomersPayments {
                        Id = Convert.ToInt64(dr[0]),
                        Type = new Types { Id = Convert.ToInt64(dr[1]) },
                        Date = Convert.ToDateTime(dr[2]).ToString("d MMMM, yyyy"),
                        PostDate = Convert.ToDateTime(dr[2]),
                        Receipt = dr[3].ToString(),
                        Cheque = dr[4].ToString(),
                        Notes = dr[5].ToString(),
                        Amount = Convert.ToDouble(dr[6]),
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[7]),
                            Code = dr[8].ToString(),
                            Name = dr[9].ToString(),
                            Prefix = dr[10].ToString()
                        },
                        Customer = new Customers {
                            Id = Convert.ToInt64(dr[11]),
                            Name = dr[12].ToString()
                        }
                    };
                }
            }

            return null;
        }

        public CustomersWithholding GetCustomersWithholding(long idnt, Stations station) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; SELECT cw_idnt, cw_date, cw_receipt, cw_invoice, cw_description, cw_amount, cw_added_by, cw_type, CASE WHEN cw_type=1 THEN 'WHT VAT' ELSE 'WHT TAX' END cw_types, Custid, Names FROM CustomersWithholding INNER JOIN Customers ON cw_cust=Custid WHERE cw_idnt=" + idnt);
            if (dr.HasRows) {
                while (dr.Read()) {
                    return new CustomersWithholding {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        DateString = Convert.ToDateTime(dr[1]).ToString("d MMMM, yyyy"),
                        Receipt = dr[2].ToString(),
                        Invoice = dr[3].ToString(),
                        Description = dr[4].ToString(),
                        Amount = Convert.ToDouble(dr[5]),
                        User = new Users { Username = dr[6].ToString() },
                        Type = new Types {
                            Id = Convert.ToInt64(dr[7]),
                            Name = dr[8].ToString(),
                        },
                        Customer = new Customers {
                            Id = Convert.ToInt64(dr[9]),
                            Name = dr[10].ToString(),
                            Station = station
                        }
                    };
                }
            }

            return null;
        }

        public List<CustomersPayments> GetCustomerPayments(DateTime start, DateTime stop, string stations, string customers, string filter = "") {
            List<CustomersPayments> payments = new List<CustomersPayments>();

            SqlServerConnection conn = new SqlServerConnection();
            string q = conn.GetQueryString(filter, "CAST(rcpt AS NVARCHAR)+'-'+CAST(imcq AS NVARCHAR)+'-'+CAST(im_paid AS NVARCHAR)+'-'+st_code+'-'+st_name+'-'+CASE im_type WHEN 4 THEN 'LIPA NA MPESA' WHEN 3 THEN BankName+(CASE WHEN BankName IN ('CFC', 'KCB', 'EQUITY') THEN ' VISA' END)ELSE ISNULL(Names,'-- INVALID') END", "im_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");
            if (!string.IsNullOrEmpty(stations)) {
                q += " AND im_st IN (" + stations + ")";
            }

            if (!string.IsNullOrEmpty(customers)) {
                q += " AND im_type=0 AND im_cust IN (" + customers + ")";
            }

            SqlDataReader dr = conn.SqlServerConnect("SELECT im_idnt, im_type, im_date, rcpt, imcq, mm_notes, im_paid, st_idnt, st_code, st_name, st_database, im_cust, CASE im_type WHEN 4 THEN 'LIPA NA MPESA' WHEN 3 THEN BankName+(CASE WHEN BankName IN ('CFC', 'KCB', 'EQUITY') THEN ' VISA' ELSE '' END) ELSE ISNULL(Names,'N/A') END NameX FROM (SELECT im_st, im_idnt, im_cust, 0 im_type, im_date, rcpt, imcq, mm_notes, im_paid FROM vCustomersPayment UNION ALL SELECT aw_st, aw_idnt, aw_account, aw_type, aw_date, 88, aw_chqs, aw_notes, aw_amount FROM vAccountsWithdraw) As Foo INNER JOIN Stations ON im_st=st_idnt LEFT OUTER JOIN vBankAccounts ON im_cust=BankID AND im_st=BankSt LEFT OUTER JOIN vCustomers ON im_cust=Custid AND im_st=Sts " + q + " ORDER BY im_date, st_order, rcpt, NameX");
            if (dr.HasRows) {
                while (dr.Read()) {
                    CustomersPayments pymt = new CustomersPayments {
                        Id = Convert.ToInt64(dr[0]),
                        Type = new Types { Id = Convert.ToInt64(dr[1]) },
                        Date = Convert.ToDateTime(dr[2]).ToString("dd/MM/yyyy"),
                        PostDate = Convert.ToDateTime(dr[2]),
                        Receipt = dr[3].ToString(),
                        Cheque = dr[4].ToString(),
                        Notes = dr[5].ToString(),
                        Amount = Convert.ToDouble(dr[6])
                    };

                    pymt.Station = new Stations {
                        Id = Convert.ToInt64(dr[7]),
                        Code = dr[8].ToString(),
                        Name = dr[9].ToString(),
                        Prefix = dr[10].ToString()
                    };

                    pymt.Customer = new Customers {
                        Id = Convert.ToInt64(dr[11]),
                        Name = dr[12].ToString()
                    };

                    payments.Add(pymt);
                }
            }

            return payments;
        }

        public List<CustomersPayments> GetCustomerPayments(Stations station, DateTime start, DateTime stop, Customers customer, string filter = "") {
            List<CustomersPayments> payment = new List<CustomersPayments>();
            SqlServerConnection conn = new SqlServerConnection();

            string query = conn.GetQueryString(filter, "Names+'-'+im_type_name+'-'+im_notes+'-'+im_rcpt+'-'+im_chqs+'-'+CAST(im_paid AS NVARCHAR)");
            if (customer != null)
                query += " AND Custid=" + customer.Id;

            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start + "', @stop DATE='" + stop + "'; SELECT im_idnt, im_date, im_rcpt, im_chqs, im_notes, im_paid, im_type, im_type_name, im_cust, Names FROM (SELECT im_cust, im_idnt, im_date, 0 im_type, 'PAYMENT' im_type_name, CAST(CAST(im_change AS INT)AS NVARCHAR) im_rcpt, CAST(CAST(im_tendered AS INT) AS NVARCHAR)im_chqs, im_paid, N'N/A' im_notes FROM InvoicePayment WHERE im_date BETWEEN @start AND @stop UNION ALL SELECT cw_cust, cw_idnt, cw_date, cw_type, CASE WHEN cw_type=1 THEN 'WHT VAT' ELSE 'WHT TAX' END, cw_receipt, '-', cw_amount, cw_description FROM CustomersWithholding WHERE cw_date BETWEEN @start AND @stop) As Foo INNER JOIN Customers ON im_cust=Custid " + query + "ORDER BY im_date, im_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    payment.Add(new CustomersPayments {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                        PostDate = Convert.ToDateTime(dr[1]),
                        Receipt = dr[2].ToString(),
                        Cheque = dr[3].ToString(),
                        Notes = dr[4].ToString(),
                        Amount = Convert.ToDouble(dr[5]),
                        Type = new Types {
                            Id = Convert.ToInt64(dr[6]),
                            Name = dr[7].ToString(),
                        },
                        Customer = new Customers {
                            Id = Convert.ToInt64(dr[8]),
                            Name = dr[9].ToString(),
                        },
                        Station = station
                    });
                }
            }

            return payment;
        }

        public List<StationsMonthlySummary> GetStationsMonthlySummary(Stations station, int month, int year) {
            List<StationsMonthlySummary> summary = new List<StationsMonthlySummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + station.Id + ", @mnth INT=" + month + ", @year INT=" + year + "; SELECT dt [date], SUM(CASE WHEN fuel=1 THEN sale ELSE 0 END) dx_sale, SUM(CASE WHEN fuel=2 THEN sale ELSE 0 END) ux_sale, SUM(CASE WHEN fuel=3 THEN sale ELSE 0 END) vp_sale, SUM(CASE WHEN fuel=4 THEN sale ELSE 0 END) ik_sale, SUM(sale) tt_sale, SUM(CASE WHEN fuel=1 THEN credit ELSE 0 END) dx_credit, SUM(CASE WHEN fuel=2 THEN credit ELSE 0 END) ux_credit, SUM(CASE WHEN fuel=3 THEN credit ELSE 0 END) vp_credit, SUM(CASE WHEN fuel=4 THEN credit ELSE 0 END) ik_credit, SUM(CASE WHEN fuel BETWEEN 1 AND 4 THEN credit ELSE 0 END) tt_credit, SUM(CASE WHEN fuel NOT BETWEEN 1 AND 4 THEN credit ELSE 0 END) ot_credit, SUM(cash) cash, SUM(discount) discount, SUM(transport) transport, SUM(sale-cash-credit-discount-transport) lesses, SUM(lubes) lubes, SUM(CASE WHEN fuel=0 THEN credit ELSE 0 END) lubes_credit, SUM(gas) gas, SUM(gas_vat) gas_vat, SUM(soda) soda, SUM(rent) rent, SUM(carwash) carwash, SUM(CASE WHEN fuel=-1 THEN credit ELSE 0 END) carwash_credit, SUM(srvs) srvs, SUM(CASE WHEN fuel=-2 THEN credit ELSE 0 END) srvs_credit, SUM(tyre) tyre, SUM(CASE WHEN fuel=-3 THEN credit ELSE 0 END) tyre_credit FROM( SELECT pcol_date dt, pcol_fuel fuel, pcol_amts sale, 0 credit, 0 discount, 0 cash, 0 transport, 0 carwash, 0 tyre, 0 srvs, 0 lubes, 0 gas, 0 gas_vat, 0 soda, 0 rent FROM vFuelSales WHERE pcol_st=@st AND MONTH(pcol_date)=@mnth AND YEAR(pcol_date)=@year UNION ALL SELECT sr_date, sr_fuel, 0, sr_amts, sr_discount,0,0,0,0,0,0,0,0,0,0 FROM vFuelSalesCredit WHERE sr_st=@st AND MONTH(sr_date)=@mnth AND YEAR(sr_date)=@year UNION ALL SELECT am_date, am_item, 0, am_amount, 0,0,0,0,0,0,0,0,0,0,0 FROM vFuelSalesAccounts WHERE am_st=@st AND MONTH(am_date)=@mnth AND YEAR(am_date)=@year UNION ALL SELECT ex_date, 0,0,0,0,ex_amount,0,0,0,0,0,0,0,0,0 FROM vExpenses WHERE ex_st=@st AND MONTH(ex_date)=@mnth AND YEAR(ex_date)=@year UNION ALL SELECT ar_date, 0,0,0,0,ar_cash, 0, ar_cwash, ar_tyre, ar_service,0,0,0,0,0 FROM vCash WHERE ar_st=@st AND MONTH(ar_date)=@mnth AND YEAR(ar_date)=@year UNION ALL SELECT pt_date, 0,0,0,0,0, pt_amount,0,0,0,0,0,0,0,0 FROM vTransport WHERE pt_st=@st AND MONTH(pt_date)=@mnth AND YEAR(pt_date)=@year UNION ALL SELECT bk_date, 0,0,0,0,0,0,0,0,0, bk_lubes, bk_gas, bk_gas_vat, bk_soda,0 FROM vBanking WHERE bk_st=@st AND MONTH(bk_date)=@mnth AND YEAR(bk_date)=@year UNION ALL SELECT rt_date, 0,0,0,0,0,0,0,0,0,0,0,0,0, rt_amount FROM vRent WHERE rt_st=@st AND MONTH(rt_date)=@mnth AND YEAR(rt_date)=@year) As Foo GROUP BY dt ORDER BY dt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    summary.Add(new StationsMonthlySummary {
                        Date = Convert.ToDateTime(dr[0]),

                        DxSale = Convert.ToDouble(dr[1]),
                        UxSale = Convert.ToDouble(dr[2]),
                        VpSale = Convert.ToDouble(dr[3]),
                        IkSale = Convert.ToDouble(dr[4]),
                        Sale = Convert.ToDouble(dr[5]),

                        DxCredit = Convert.ToDouble(dr[6]),
                        UxCredit = Convert.ToDouble(dr[7]),
                        VpCredit = Convert.ToDouble(dr[8]),
                        IkCredit = Convert.ToDouble(dr[9]),
                        Credit = Convert.ToDouble(dr[10]),
                        OxCredit = Convert.ToDouble(dr[11]),

                        Cash = Convert.ToDouble(dr[12]),
                        Discount = Convert.ToDouble(dr[13]),
                        Transport = Convert.ToDouble(dr[14]),
                        Lesses = Convert.ToDouble(dr[15]),

                        Lube = Convert.ToDouble(dr[16]),
                        LubeCredit = Convert.ToDouble(dr[17]),
                        Gas = Convert.ToDouble(dr[18]),
                        GasVat = Convert.ToDouble(dr[19]),
                        Soda = Convert.ToDouble(dr[20]),
                        Rent = Convert.ToDouble(dr[21]),

                        Carwash = Convert.ToDouble(dr[22]),
                        CarwashCredit = Convert.ToDouble(dr[23]),
                        Service = Convert.ToDouble(dr[24]),
                        ServiceCredit = Convert.ToDouble(dr[25]),
                        Tyre = Convert.ToDouble(dr[26]),
                        TyreCredit = Convert.ToDouble(dr[27]),
                    });
                }
            }

            return summary;
        }

        public List<EtrSheet> GetEtrSheet(Stations station, int month, int year) {
            List<EtrSheet> sheets = new List<EtrSheet>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + station.Id + ", @mnth INT=" + month + ", @year INT=" + year + "; SELECT op_date, SUM(ldg_cash)cash, SUM(CASE WHEN itm=1 THEN (((ldg_sale-ldg_credit-ldg_lesses-ldg_transport+ldg_disc)/pp_price)*(pp_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+SUM(CASE WHEN itm=2 THEN (((ldg_sale-ldg_credit+ldg_disc)/pp_price)*(pp_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+SUM(CASE WHEN itm=3 THEN (((ldg_sale-ldg_credit+ldg_disc)/pp_price)*(pp_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+SUM(CASE WHEN itm=4 THEN (((ldg_sale-ldg_credit+ldg_disc)/pp_price)*(pp_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END) vat_amts, SUM(CASE WHEN itm=1 THEN ((ldg_sale-ldg_credit-ldg_lesses-ldg_transport+ldg_disc)/pp_price)*vat_zero ELSE 0 END)+SUM(CASE WHEN itm=2 THEN ((ldg_sale-ldg_credit+ldg_disc)/pp_price)*vat_zero ELSE 0 END)+SUM(CASE WHEN itm=3 THEN ((ldg_sale-ldg_credit+ldg_disc)/pp_price)*vat_zero ELSE 0 END)+SUM(CASE WHEN itm=4 THEN ((ldg_sale-ldg_credit+ldg_disc)/pp_price)*vat_zero ELSE 0 END) vat_zero, SUM(cw_serv) cw_serv, SUM(soda) soda, SUM(lubes)-SUM(CASE WHEN itm=0 THEN ldg_credit ELSE 0 END) lubes, SUM(ldg_rent) rent, SUM(gas) gas, SUM(gas_vat) gas_vat, SUM(ldg_credit) credit, SUM(CASE WHEN ldg_credit>0 AND itm=1 THEN (ldg_credit/ldg_price)*vat_zero ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=2 THEN (ldg_credit/ldg_price)*vat_zero ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=3 THEN (ldg_credit/ldg_price)*vat_zero ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=4 THEN (ldg_credit/ldg_price)*vat_zero ELSE 0 END) credit_zero, SUM(CASE WHEN ldg_credit>0 AND itm=1 THEN ((ldg_credit/ldg_price)*(ldg_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=2 THEN ((ldg_credit/ldg_price)*(ldg_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=3 THEN ((ldg_credit/ldg_price)*(ldg_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END)+ SUM(CASE WHEN ldg_credit>0 AND itm=4 THEN ((ldg_credit/ldg_price)*(ldg_price-vat_zero))*(vat_rate/(100+vat_rate)) ELSE 0 END) credit_vat, SUM(CASE WHEN itm=0 THEN ldg_credit ELSE 0 END) credit_lubes, SUM(ldg_cash+ldg_credit+ldg_rent+gas+gas_vat+soda+lubes) total, ISNULL(etr_totalvat,0) etr_check FROM ( SELECT op_st, op_date, op_fuel itm, 0 ldg_sale, op_amount+(op_discount*-1) ldg_over, 0 ldg_disc, 0 ldg_lesses, 0 ldg_transport, 0 ldg_credit, 0 ldg_cash, 0 ldg_price, 0 ldg_rent, 0 soda, 0 lubes, 0 gas, 0 gas_vat, 0 cw_serv FROM vOverpump WHERE op_st=@st AND YEAR(op_date)=@year AND MONTH(op_date)=@mnth UNION ALL SELECT sr_st, sr_date, sr_fuel, 0,0, sr_discount,0,0, sr_amts,0, sr_price,0,0,0,0,0,0 FROM vInvoicesLedger WHERE sr_st=@st AND YEAR(sr_date)=@year AND MONTH(sr_date)=@mnth UNION ALL SELECT am_st, am_date, am_item, 0,0,0,0,0, am_amts,0, am_price,0,0,0,0,0,0 FROM vAccounts WHERE am_st=@st AND YEAR(am_date)=@year AND MONTH(am_date)=@mnth UNION ALL SELECT pcol_st, pcol_date, pmp_item, pcol_price*(pcol_cl-pcol_op-pcol_adjust-pcol_test),0,0,0,0,0,0,0,0,0,0,0,0,0 FROM vReadings INNER JOIN vPumps ON pcol_st = pmp_st AND pcol_pump = pmp_idnt WHERE pcol_st=@st AND YEAR(pcol_date)=@year AND MONTH(pcol_date)=@mnth UNION ALL SELECT dd_st, dd_date, 0, 0,0,0,dd_lesses,dd_transport,0, dd_cash+dd_expenses,0,0,0,0,0,0,(dd_cwash+dd_service+dd_tyre) FROM vDiffs WHERE dd_st=@st AND YEAR(dd_date)=@year AND MONTH(dd_date)=@mnth UNION ALL SELECT rt_st, rt_date, 0,0,0,0,0,0,0,0,0,rt_amount,0,0,0,0,0 FROM vRent WHERE rt_st=@st AND YEAR(rt_date)=@year AND MONTH(rt_date)=@mnth UNION ALL SELECT bk_st, bk_date, 0,0,0,0,0,0,0,0,0,0, bk_soda, bk_lubes, bk_gas, bk_gas_vat,0 FROM vBanking WHERE bk_st=@st AND YEAR(bk_date)=@year AND MONTH(bk_date)=@mnth ) As Foo LEFT OUTER JOIN vPumpsPrices ON pp_date=op_date AND pp_fuel=itm AND op_st=pp_st LEFT OUTER JOIN VatReports ON vat_date=op_date AND vat_item=itm LEFT OUTER JOIN vEtrBreakdown ON etr_date=op_date AND etr_st=op_st GROUP BY op_date, etr_totalvat ORDER BY op_date");
            if (dr.HasRows) {
                while (dr.Read()) {
                    EtrSheet item = new EtrSheet {
                        Date = Convert.ToDateTime(dr[0]),
                        Cash = Convert.ToDouble(dr[1]),
                        CashVats = Convert.ToDouble(dr[2]),
                        CashZero = Convert.ToDouble(dr[3]),

                        Serv = Convert.ToDouble(dr[4]),
                        Soda = Convert.ToDouble(dr[5]),
                        Lube = Convert.ToDouble(dr[6]),
                        Rent = Convert.ToDouble(dr[7]),
                        Gas = Convert.ToDouble(dr[8]),
                        GasVat = Convert.ToDouble(dr[9]),

                        Credit = Convert.ToDouble(dr[10]),
                        CreditZero = Convert.ToDouble(dr[11]),
                        CreditVats = Convert.ToDouble(dr[12]),
                        CreditLube = Convert.ToDouble(dr[13]),

                        Total = Convert.ToDouble(dr[14]),
                        Check = Convert.ToDouble(dr[15]),
                    };

                    item.Vat08 = item.CashVats + item.CreditVats;
                    item.Vat16 = (item.Serv + item.Soda + item.Lube + item.CreditLube + item.Rent + item.GasVat) * (16.0 / 116.0);

                    sheets.Add(item);
                }
            }

            return sheets;
        }

        public List<ProductsSales> GetProductsSales(Stations station, DateTime start, DateTime stop, string category) {
            List<ProductsSales> sales = new List<ProductsSales>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "', @catg NVARCHAR(250)='" + category + "'; SELECT DISTINCT id_, Items, ISNULL((sAmts/sQnty),uPrice) sp, ISNULL((SELECT TOP(1) price FROM PurchasesDetails INNER JOIN Purchases ON PurNum=PurNo WHERE date_<=@stop AND id_=item_id ORDER BY date_ DESC),ISNULL((sAmts/sQnty),uPrice)) bp, ISNULL(pQnty,0) inn, ISNULL(sQnty,0) sl, ISNULL(tQnty,0)tr, uAvailable-ISNULL(aQnty,0) cl, ISNULL(sAmts,0)income, Category, itemName FROM pProducts LEFT OUTER JOIN (SELECT item_idnt aIdnt, SUM(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END) As aQnty FROM ProductsMovement INNER JOIN ProductsMovementSources ON srcs_idnt=pm_idnt WHERE date_>@stop GROUP BY item_idnt) As aFoo ON id_=aIdnt LEFT OUTER JOIN (SELECT item_ sIdnt, SUM(qty) sQnty, SUM(qty*price) sAmts FROM SalesDetails INNER JOIN Sales ON RcptNo_=RcptNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_) As sFoo ON id_=sIdnt LEFT OUTER JOIN (SELECT item_id pIdnt, SUM(qty) pQnty, SUM(qty*price) pAmts FROM PurchasesDetails INNER JOIN Purchases ON PurNum=PurNo WHERE date_ BETWEEN @start AND @stop GROUP BY item_id) As pFoo On id_=pIdnt LEFT OUTER JOIN (SELECT tr_item, SUM(tr_qt1+tr_qt2+tr_qt3+tr_qt4+tr_qt5+tr_qt6) tQnty FROM ProductsTransfer WHERE tr_date BETWEEN @start AND @stop GROUP BY tr_item) As tFoo On id_=tr_item LEFT OUTER JOIN (SELECT item_id itmx, price costx FROM PurchasesDetails INNER JOIN Purchases ON PurNum=PurNo INNER JOIN (SELECT MAX(CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR)) pd, item_id itm FROM PurchasesDetails INNER JOIN Purchases ON PurNum = PurNo WHERE date_<=@stop GROUP BY item_id) As Foo ON pd=CAST(date_ AS NVARCHAR)+'-'+CAST(PurNum AS NVARCHAR) AND item_id=itm) As cFoo ON id_=itmx WHERE id_>=10 AND Category=@catg ORDER BY Category, itemName, Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ProductsSales sale = new ProductsSales {
                        Product = new Products {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Sp = Convert.ToDouble(dr[2]),
                            Bp = Convert.ToDouble(dr[3]),
                        },
                        Inns = Convert.ToDouble(dr[4]),
                        Sales = Convert.ToDouble(dr[5]),
                        Transfer = Convert.ToDouble(dr[6]),
                        Closing = Convert.ToDouble(dr[7]),
                        Amounts = Convert.ToDouble(dr[8]),
                    };

                    sale.Opening = sale.Closing + sale.Sales + sale.Transfer - sale.Inns;
                    sales.Add(sale);
                }
            }

            return sales;
        }

        public List<ProductsTransfer> GetProductsTransfers(DateTime start, DateTime stop, string filter = "") {
            List<ProductsTransfer> ledger = new List<ProductsTransfer>();

            SqlServerConnection conn = new SqlServerConnection();
            var query = conn.GetQueryString(filter, "Items+'-'+CAST(uPrice AS NVARCHAR)+'-'+CAST(tr_qt1 AS NVARCHAR)+'-'+CAST(tr_qt2 AS NVARCHAR)+'-'+CAST(tr_qt3 AS NVARCHAR)+'-'+CAST(tr_qt4 AS NVARCHAR)", "tr_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");

            SqlDataReader dr = conn.SqlServerConnect("USE shell_kinoru; SELECT tr_idnt, tr_date, tr_qt1, tr_qt2, tr_qt3, tr_qt4, tr_item, Items, Category, uPrice FROM ProductsTransfer INNER JOIN pProducts ON tr_item=id_ " + query + " ORDER BY tr_date, Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ProductsTransfer transfer = new ProductsTransfer {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        Gitimbine = Convert.ToDouble(dr[2]),
                        Kaaga = Convert.ToDouble(dr[3]),
                        Kirunga = Convert.ToDouble(dr[4]),
                        Nkubu = Convert.ToDouble(dr[5]),
                        Product = new Products {
                            Id = Convert.ToInt64(dr[6]),
                            Name = dr[7].ToString(),
                            Category = dr[8].ToString(),
                            Sp = Convert.ToDouble(dr[9])
                        }
                    };

                    transfer.DateString = transfer.Date.ToString("dd/MM/yyyy");
                    transfer.Total = transfer.Gitimbine + transfer.Kaaga + transfer.Kirunga + transfer.Nkubu;

                    ledger.Add(transfer);
                }
            }

            return ledger;
        }

        public List<ProductsTransfer> GetProductsTransfersSummary(DateTime start, DateTime stop) {
            List<ProductsTransfer> ledger = new List<ProductsTransfer>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE shell_kinoru; DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT tr_item, Items, Category, uPrice, SUM(tr_qt1)git, SUM(tr_qt2)kaag, SUM(tr_qt3)kir, SUM(tr_qt4)nkbu FROM ProductsTransfer INNER JOIN pProducts ON tr_item=id_ WHERE tr_date BETWEEN @start AND @stop GROUP BY Items, Category, uPrice, tr_item ORDER BY Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ProductsTransfer transfer = new ProductsTransfer {
                        Product = new Products {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Category = dr[2].ToString(),
                            Sp = Convert.ToDouble(dr[3])
                        },
                        Gitimbine = Convert.ToDouble(dr[4]),
                        Kaaga = Convert.ToDouble(dr[5]),
                        Kirunga = Convert.ToDouble(dr[6]),
                        Nkubu = Convert.ToDouble(dr[7]),
                    };

                    transfer.Total = transfer.Gitimbine + transfer.Kaaga + transfer.Kirunga + transfer.Nkubu;
                    ledger.Add(transfer);
                }
            }

            return ledger;
        }

        public List<ProductsTransferCompare> GetProductsTransferCompare(DateTime start, DateTime stop, string filter = "") {
            List<ProductsTransferCompare> compare = new List<ProductsTransferCompare>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT tr_item, Items, SUM(tr_git)tr_git, SUM(tr_kg)tr_kg, SUM(tr_kr)tr_kr, SUM(tr_nkb)tr_nkb, SUM(dlv_git)dlv_git, SUM(dlv_kg)dlv_kg, SUM(dlv_kr)dlv_kr, SUM(dlv_nkb)dlv_nkb FROM vTransfersCompare INNER JOIN shell_kinoru.dbo.pProducts ON id_=tr_item " + conn.GetQueryString(filter, "Items", "tr_date BETWEEN @start AND @stop") + " GROUP BY tr_item, Items ORDER BY Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ProductsTransferCompare comp = new ProductsTransferCompare {
                        Load = new ProductsTransfer {
                            Product = new Products {
                                Id = Convert.ToInt64(dr[0]),
                                Name = dr[1].ToString()
                            },
                            Gitimbine = Convert.ToDouble(dr[2]),
                            Kaaga = Convert.ToDouble(dr[3]),
                            Kirunga = Convert.ToDouble(dr[4]),
                            Nkubu = Convert.ToDouble(dr[5])
                        },
                        Delv = new ProductsTransfer {
                            Gitimbine = Convert.ToDouble(dr[6]),
                            Kaaga = Convert.ToDouble(dr[7]),
                            Kirunga = Convert.ToDouble(dr[8]),
                            Nkubu = Convert.ToDouble(dr[9])
                        }
                    };

                    comp.Load.Total = comp.Load.Gitimbine + comp.Load.Kaaga + comp.Load.Kirunga + comp.Load.Nkubu;
                    comp.Delv.Total = comp.Delv.Gitimbine + comp.Delv.Kaaga + comp.Delv.Kirunga + comp.Delv.Kirunga;

                    if (!comp.Load.Gitimbine.Equals(comp.Delv.Gitimbine) || !comp.Load.Kaaga.Equals(comp.Delv.Kaaga) || !comp.Load.Kirunga.Equals(comp.Delv.Kirunga) || !comp.Load.Kirunga.Equals(comp.Delv.Kirunga))
                        comp.Error++;

                    compare.Add(comp);
                }
            }

            return compare;
        }

        public List<ProductsSales> GetFuelSales(Stations station, DateTime start, DateTime stop) {
            List<ProductsSales> sales = new List<ProductsSales>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "'; SELECT pcol_fuel, pcol_item, (SELECT TOP(1) price FROM PurchasesDetails INNER JOIN Purchases ON PurNo=PurNum WHERE item_id=pcol_fuel AND date_<pcol_to ORDER BY date_ DESC) pcol_bp, pcol_sp, pcol_from, pcol_to, pcol_open, pcol_sales FROM (SELECT pcol_fuel, ItemName pcol_item, pcol_price pcol_sp, MIN(pcol_date)pcol_from, MAX(pcol_date)pcol_to, SUM(pcol_electronic_cl-pcol_electronic_op-pcol_electronic_test-pcol_electronic_adjust) pcol_sales FROM PumpsCollections INNER JOIN Products ON id_=pcol_fuel WHERE pcol_date BETWEEN @start AND @stop group by pcol_fuel, pcol_price, ItemName) As Foo LEFT OUTER JOIN (SELECT td_fuel, td_date, SUM(td_reading)pcol_open FROM TanksDips WHERE td_date BETWEEN DATEADD(DAY,-1, @start) AND @stop GROUP BY td_fuel, td_date) As Op ON td_date=DATEADD(DAY,-1, pcol_from) AND pcol_fuel=td_fuel ORDER BY pcol_fuel, pcol_from");
            if (dr.HasRows) {
                while (dr.Read()) {
                    sales.Add(new ProductsSales {
                        Product = new Products {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Bp = Convert.ToDouble(dr[2]),
                            Sp = Convert.ToDouble(dr[3]),
                        },
                        From = Convert.ToDateTime(dr[4]),
                        To = Convert.ToDateTime(dr[5]),
                        Opening = Convert.ToDouble(dr[6]),
                        Sales = Convert.ToDouble(dr[7])
                    });
                }
            }

            return sales;
        }

        public ProductsBanking GetProductsBanking(Stations station, DateTime start, DateTime stop, string category) {
            ProductsBanking banking = new ProductsBanking();
            int item = 99;

            SqlDataReader dr = new SqlServerConnection().SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; SELECT ISNULL(SUM(bk_lubes),0)x, ISNULL(SUM(bk_gas),0)x, ISNULL(SUM(bk_other_1),0)x, ISNULL(SUM(bk_soda),0)x FROM Banking WHERE bk_date BETWEEN '" + start + "' AND '" + stop + "'");
            if (dr.Read()) {
                if (category.Equals("lubes")) {
                    item = -1;
                    banking.Banked = Convert.ToDouble(dr[0]);
                }
                else if (category.Equals("lubes")) {
                    item = -5;
                    banking.Banked = Convert.ToDouble(dr[1]);
                    banking.Exempt = Convert.ToDouble(dr[2]);
                }
                else if (category.Equals("lubes")) {
                    banking.Banked = Convert.ToDouble(dr[3]);
                }
            }

            dr = new SqlServerConnection().SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; SELECT ISNULL(SUM(cd_disc),0)xDisc FROM Discounts WHERE cd_item=" + item + " AND cd_date BETWEEN '" + start + "' AND '" + stop + "'");
            if (dr.Read()) {
                banking.Discount = Convert.ToDouble(dr[0]);
            }

            return banking;
        }

        public List<ProductsLedger> GetProductsVariance(Stations station, DateTime start, DateTime stop) {
            List<ProductsLedger> variance = new List<ProductsLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @date1 DATE='" + start.Date + "', @date2 DATE='" + stop.Date + "'; SELECT vat_item, itemName, vat_date, SUM(ISNULL(purc,0)) PURC, SUM(ISNULL(delv,0)) DELV, SUM(ISNULL(ovp,0)) OVP FROM " + conn.Database + ".dbo.VatReports INNER JOIN Products ON id_=vat_item LEFT OUTER JOIN (SELECT date_ dts, item_id itm, qty purc, 0 delv, 0 ovp FROM PurchasesDetails INNER JOIN Purchases ON PurNum = PurNo WHERE item_id BETWEEN 1 AND 10 AND date_ BETWEEN @date1 AND @date2 UNION ALL SELECT fr_date, fr_fuel, 0, fr_quantity, 0 FROM FuelReceipts WHERE fr_date BETWEEN @date1 AND @date2 UNION ALL SELECT sr_date, sr_fuel, 0,0,sr_qnty FROM Receipts WHERE sr_overpump=1) As foo ON dts=vat_date AND vat_item=itm WHERE vat_date BETWEEN @date1 AND @date2 GROUP BY vat_date, vat_item, itemName ORDER BY vat_date, vat_item");
            if (dr.HasRows) {
                while (dr.Read()) {
                    variance.Add(new ProductsLedger {
                        Product = new Products {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString()
                        },
                        Date = Convert.ToDateTime(dr[2]),
                        Purchase = Convert.ToDouble(dr[3]),
                        Delivery = Convert.ToDouble(dr[4]),
                        Overpump = Convert.ToDouble(dr[5]),
                    });
                }
            }

            return variance;
        }

        public List<Products> GetProductsUnlinked(string filter = "") {
            List<Products> products = new List<Products>();

            SqlServerConnection conn = new SqlServerConnection();
            string query = conn.GetQueryString(filter, "Items+'-'+uMeasure+'-'+Category+'-'+CAST(uPrice AS NVARCHAR)+'-'+CAST(uAvailable AS NVARCHAR)+'-'+CAST(qLitres AS NVARCHAR)+'-'+st_code+'-'+st_name");

            SqlDataReader dr = conn.SqlServerConnect("SELECT id_, Items, Category, uMeasure, tax, uPrice, uAvailable, qLitres, st_idnt, st_code, st_name FROM vLubesUnlinked INNER JOIN Stations ON st_idnt=st " + query + " ORDER BY st_order, Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    products.Add(new Products {
                        Id = Convert.ToInt64(dr[0]),
                        Name = dr[1].ToString(),
                        Category = dr[2].ToString(),
                        Measure = dr[3].ToString(),
                        Tax = Convert.ToDouble(dr[4]),
                        Sp = Convert.ToDouble(dr[5]),
                        Quantity = Convert.ToInt64(dr[6]),
                        Ltrs = Convert.ToDouble(dr[7]),
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[8]),
                            Code = dr[9].ToString(),
                            Name = dr[10].ToString(),
                        }
                    });
                }
            }

            return products;
        }

        public List<ProductsLinked> GetProductsLinked(string filter = "") {
            List<ProductsLinked> products = new List<ProductsLinked>();

            SqlServerConnection conn = new SqlServerConnection();
            string query = conn.GetQueryString(filter, "mk.Items+'-'+ISNULL(gt.Items,'-')+'-'+ISNULL(kg.Items,'-')+'-'+ISNULL(nk.Items,'-')+''+ISNULL(ki.Items,'-')", "mk.Category LIKE 'LUBES'");

            SqlDataReader dr = conn.SqlServerConnect("SELECT ISNULL(prd_idnt,0)LinkId, mk.id_, mk.Items, ISNULL(gt.Items,'-') gitimbine, ISNULL(kg.Items,'-') kaaga, ISNULL(nk.Items,'-') nkubu, ISNULL(ki.Items,'-') kirunga FROM shell_kinoru.dbo.pProducts mk LEFT OUTER JOIN core_system.dbo.ProductsLink ON mk.id_=prd_idnt LEFT OUTER JOIN shell_gitimbine.dbo.pProducts gt ON gt.id_=prd_gitimbine AND gt.id_<>0 LEFT OUTER JOIN shell_kaaga.dbo.pProducts kg ON kg.id_=prd_kaaga AND kg.id_<>0 LEFT OUTER JOIN shell_nkubu.dbo.pProducts nk ON nk.id_=prd_nkubu AND nk.id_<>0 LEFT OUTER JOIN shell_kirunga.dbo.pProducts ki ON ki.id_=prd_kirunga AND ki.id_<>0 " + query + " ORDER BY mk.Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    products.Add(new ProductsLinked {
                        Id = Convert.ToInt64(dr[0]),
                        Product = new Products {
                            Id = Convert.ToInt64(dr[1]),
                            Name = dr[2].ToString(),
                        },
                        Gitimbine = new Products {
                            Name = dr[3].ToString(),
                        },
                        Kaaga = new Products {
                            Name = dr[4].ToString(),
                        },
                        Nkubu = new Products {
                            Name = dr[5].ToString()
                        },
                        Kirunga = new Products {
                            Name = dr[6].ToString()
                        }
                    });
                }
            }

            return products;
        }

        //Deliveries
        public List<SelectListItem> GetDeliveryTypesIEnumerable() {
            return Core.GetIEnumerable("SELECT dt_idnt, dt_type FROM DeliveryType ORDER BY dt_idnt");
        }

        public Delivery GetDelivery(int idnt) {
            Delivery delivery = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT dlv_idnt, dlv_date, dlv_rcpt, dlv_notes, ISNULL(pc_json, '{\"PettyCash\":[]}')pc_json, dlv_amount, ISNULL(pc_amount,0) dlv_pc, dlv_discount, dlv_added_on, dlv_added_by, dt_idnt, dt_type, st_idnt, st_code, st_name, bk_idnt, bk_code, bk_bank, bk_branch FROM Delivery INNER JOIN DeliveryType ON dlv_type=dt_idnt INNER JOIN BankAccounts ON dlv_bank=bk_idnt INNER JOIN Stations ON dlv_station=st_idnt LEFT OUTER JOIN vDeliveryPettyCash ON dlv_idnt=pc_delv WHERE dlv_idnt=" + idnt);
            if (dr.Read()) {
                delivery = new Delivery {
                    Id = Convert.ToInt64(dr[0]),
                    Date = Convert.ToDateTime(dr[1]),
                    Receipt = dr[2].ToString(),
                    Description = dr[3].ToString(),
                    JSonExpense = dr[4].ToString(),
                    Amount = Convert.ToDouble(dr[5]),
                    Expense = Convert.ToDouble(dr[6]),
                    Discount = Convert.ToDouble(dr[7]),
                    AddedOn = Convert.ToDateTime(dr[8]),
                    AddedBy = new Users(Convert.ToInt64(dr[9])),
                    Type = new DeliveryType {
                        Id = Convert.ToInt64(dr[10]),
                        Name = dr[11].ToString()
                    },
                    Station = new Stations {
                        Id = Convert.ToInt64(dr[12]),
                        Code = dr[13].ToString(),
                        Name = dr[14].ToString()
                    },
                    Bank = new Bank {
                        Id = Convert.ToInt64(dr[15]),
                        Code = dr[16].ToString(),
                        Name = dr[17].ToString(),
                        Branch = dr[18].ToString()
                    }
                };
                
                delivery.Banking = delivery.Amount - delivery.Expense - delivery.Discount;
                delivery.DateString = delivery.Date.ToString("d MMMM, yyyy");
            }

            return delivery;
        }

        public List<Delivery> GetDeliveries(DateTime start, DateTime stop, string filter = "") {
            List<Delivery> deliveries = new List<Delivery>();

            SqlServerConnection conn = new SqlServerConnection();
            string query = conn.GetQueryString(filter, "dlv_rcpt+'-'+dlv_notes+'-'+CAST(dlv_amount AS NVARCHAR)+'-'+CAST(ISNULL(pc_amount,0) AS NVARCHAR)+'-'+CAST(dlv_discount AS NVARCHAR)+'-'+dt_type+'-'+st_code+'-'+st_name+'-'+bk_bank+'-'+bk_branch", "dlv_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'");

            SqlDataReader dr = conn.SqlServerConnect("SELECT dlv_idnt, dlv_date, dlv_rcpt, dlv_notes, ISNULL(pc_json, '{\"PettyCash\":[]}')pc_json, dlv_amount, ISNULL(pc_amount,0) dlv_pc, dlv_discount, dlv_added_on, dlv_added_by, dt_idnt, dt_type, st_idnt, st_code, st_name, bk_idnt, bk_code, bk_bank, bk_branch FROM Delivery INNER JOIN DeliveryType ON dlv_type=dt_idnt INNER JOIN BankAccounts ON dlv_bank=bk_idnt INNER JOIN Stations ON dlv_station=st_idnt LEFT OUTER JOIN vDeliveryPettyCash ON dlv_idnt=pc_delv " + query + " ORDER BY dlv_date, dlv_rcpt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Delivery delivery = new Delivery {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        Receipt = dr[2].ToString(),
                        Description = dr[3].ToString(),
                        JSonExpense = dr[4].ToString(),
                        Amount = Convert.ToDouble(dr[5]),
                        Expense = Convert.ToDouble(dr[6]),
                        Discount = Convert.ToDouble(dr[7]),
                        AddedOn = Convert.ToDateTime(dr[8]),
                        AddedBy = new Users(Convert.ToInt64(dr[9])),
                        Type = new DeliveryType {
                            Id = Convert.ToInt64(dr[10]),
                            Name = dr[11].ToString()
                        },
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[12]),
                            Code = dr[13].ToString(),
                            Name = dr[14].ToString()
                        },
                        Bank = new Bank {
                            Id = Convert.ToInt64(dr[15]),
                            Code = dr[16].ToString(),
                            Name = dr[17].ToString(),
                            Branch = dr[18].ToString()
                        }
                    };

                    delivery.Banking = delivery.Amount - delivery.Expense - delivery.Discount;
                    delivery.DateString = delivery.Date.ToString("dd/MM/yyyy");
                    deliveries.Add(delivery);
                }
            }

            return deliveries;
        }


        public List<MonthsModel> InitializeMonthsModel() {
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            List<MonthsModel> months = new List<MonthsModel>();

            for (int i = 1; i < 13; i++) {
                MonthsModel month = new MonthsModel {
                    Value = date.Month,
                    Name = date.ToString("MMMM")
                };

                month.Select |= DateTime.Now.Month == date.Month;
                months.Add(month);

                date = date.AddMonths(1);
            }

            return months;
        }

        //UPDATES
        public void UpdateStationsZeroRate(Stations station, FuelPriceChange change) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE " + station.Prefix + "Products SET tax=" + change.Taxx + ", bPriceW=" + change.Zero + " WHERE id_=" + change.Fuel.Id);
        }

        public void UpdateProductsAvailable(Stations station) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("USE " + station.Prefix.Replace(".dbo.", "") + "; UPDATE Products SET uAvailable=ISNULL(qnty,0) FROM Products LEFT OUTER JOIN pProductsAvailable ON id_=item WHERE id_>=10");
        }
    }
}
