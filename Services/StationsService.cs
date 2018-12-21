using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Models;
using Core.Extensions;

namespace Core.Services
{
    public class StationsService
    {
        
        public List<Stations> GetPendingPush()
        {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_code, st_name, push_date FROM vLastPush INNER JOIN Stations ON push_station=st_idnt WHERE push_date<CAST(DATEADD(DAY, -1, GETDATE())AS DATE) ORDER BY push_date DESC, st_order, st_name");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Stations station = new Stations();
                    station.Code = dr[0].ToString();
                    station.Name = dr[1].ToString();
                    station.Push = Convert.ToDateTime(dr[2]);

                    stations.Add(station);
                }
            }

            return stations;
        }

        public List<Stations> GetUpdatedPush()
        {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_code, st_name FROM vLastPush INNER JOIN Stations ON push_station=st_idnt WHERE push_date>=CAST(DATEADD(DAY, -1, GETDATE())AS DATE) ORDER BY push_date DESC, st_order, st_name");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Stations station = new Stations();
                    station.Code = dr[0].ToString();
                    station.Name = dr[1].ToString();

                    stations.Add(station);
                }
            }

            return stations;
        }

        public Stations GetStation(string code){
            Stations station = new Stations();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_code, st_name, st_database, push_date FROM Stations INNER JOIN vLastPush ON push_station=st_idnt WHERE st_code='" + code + "'");
            if (dr.Read())
            {
                station.Id = Convert.ToInt64(dr[0]);
                station.Code = dr[1].ToString();
                station.Name = dr[2].ToString();
                station.Prefix = dr[3].ToString();
                station.Push = Convert.ToDateTime(dr[4]);
            }

            return station;
        }

        public List<Stations> GetStations(){
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_code, st_name, sb_idnt, sb_brand, push_date, pcol_ltrs, pcol_amts FROM vLastPush INNER JOIN Stations ON push_station=st_idnt INNER JOIN StationsBrand ON st_brand=sb_idnt INNER JOIN vDailySales ON push_station=pcol_station AND push_date=pcol_date ORDER BY push_date DESC, st_order");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Stations station = new Stations();
                    station.Id = Convert.ToInt64(dr[0]);
                    station.Code = dr[1].ToString();
                    station.Name = dr[2].ToString();

                    station.Brand.Id = Convert.ToInt64(dr[3]);
                    station.Brand.Name = dr[4].ToString();

                    station.Push = Convert.ToDateTime(dr[5]);
                    station.FuelLtrs = Convert.ToDouble(dr[6]);
                    station.FuelSales = Convert.ToDouble(dr[7]);

                    stations.Add(station);
                }
            }

            return stations;
        }

        public List<Stations> GetStationsByNames()
        {
            List<Stations> stations = new List<Stations>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_idnt, st_code, st_name FROM Stations ORDER BY st_name");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Stations station = new Stations();
                    station.Id = Convert.ToInt64(dr[0]);
                    station.Code = dr[1].ToString();
                    station.Name = dr[2].ToString();

                    stations.Add(station);
                }
            }

            return stations;
        }

        public List<PumpReadings> GetMetreReadings(Stations st, DateTime date) {
            List<PumpReadings> readings = new List<PumpReadings>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pcol_idnt, pcol_pump, pmp_name, pcol_price, pcol_op, pcol_adjust, pcol_test, pcol_cl FROM vPumps LEFT OUTER JOIN vReadings ON pmp_st=pcol_st AND pmp_idnt=pcol_pump WHERE pmp_st=" + st.Id + " AND pcol_date='" + date.Date + "'");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    PumpReadings reading = new PumpReadings();
                    reading.Id = Convert.ToInt64(dr[0]);
                    reading.Pump.Id = Convert.ToInt64(dr[1]);
                    reading.Pump.Name = dr[2].ToString();

                    reading.Price = Convert.ToDouble(dr[3]);
                    reading.Opening = Convert.ToDouble(dr[4]);
                    reading.Adjustment = Convert.ToDouble(dr[5]);
                    reading.Tests = Convert.ToDouble(dr[6]);
                    reading.Closing = Convert.ToDouble(dr[7]);

                    readings.Add(reading);
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
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @yr INT=" + year + ", @st INT=" + stid + "; SELECT Cust, Names, SUM(CASE WHEN Mnths=0 THEN Amts ELSE 0 END) OPN, SUM(CASE WHEN Mnths=1 THEN Amts ELSE 0 END) JAN, SUM(CASE WHEN Mnths=2 THEN Amts ELSE 0 END) FEB, SUM(CASE WHEN Mnths=3 THEN Amts ELSE 0 END) MAR, SUM(CASE WHEN Mnths=4 THEN Amts ELSE 0 END) APR, SUM(CASE WHEN Mnths=5 THEN Amts ELSE 0 END) MAY, SUM(CASE WHEN Mnths=6 THEN Amts ELSE 0 END) JUN, SUM(CASE WHEN Mnths=7 THEN Amts ELSE 0 END) JUL, SUM(CASE WHEN Mnths=8 THEN Amts ELSE 0 END) AUG, SUM(CASE WHEN Mnths=9 THEN Amts ELSE 0 END) SEP, SUM(CASE WHEN Mnths=10 THEN Amts ELSE 0 END) OCT, SUM(CASE WHEN Mnths=11 THEN Amts ELSE 0 END) NOV, SUM(CASE WHEN Mnths=12 THEN Amts ELSE 0 END) DEC, SUM(Amts) TTL FROM (SELECT Stns, Cust, MONTH(Dates) Mnths, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)=@yr " + additionalQuery + " UNION ALL SELECT Stns, Cust, 0, (Invs-Pymt) Amts FROM vCustomerStatements WHERE Stns=@st AND YEAR(Dates)<@yr) As Foo INNER JOIN vCustomers ON [Custid]=[Cust] AND [Stns]=[Sts] GROUP BY Stns, Names, Cust ORDER BY Names");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ReportCustomerYearly entry = new ReportCustomerYearly();
                    entry.Customer.Id = Convert.ToInt64(dr[0]);
                    entry.Customer.Name = dr[1].ToString();
                    entry.Opening = Convert.ToDouble(dr[2]);
                    entry.Jan = Convert.ToDouble(dr[3]);
                    entry.Feb = Convert.ToDouble(dr[4]);
                    entry.Mar = Convert.ToDouble(dr[5]);
                    entry.Apr = Convert.ToDouble(dr[6]);
                    entry.May = Convert.ToDouble(dr[7]);
                    entry.Jun = Convert.ToDouble(dr[8]);
                    entry.Jul = Convert.ToDouble(dr[9]);
                    entry.Aug = Convert.ToDouble(dr[10]);
                    entry.Sep = Convert.ToDouble(dr[11]);
                    entry.Oct = Convert.ToDouble(dr[12]);
                    entry.Nov = Convert.ToDouble(dr[13]);
                    entry.Dec = Convert.ToDouble(dr[14]);
                    entry.Total = Convert.ToDouble(dr[15]);
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

        public List<LedgerTotals> GetLedgerTotals(String stations, DateTime date1, DateTime date2){
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
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @st INT=" + stid + ", @year INT=" + year + ", @mnth INT=" + mnth + "; SELECT Dates, SUM(Amts)Amts, SUM(Pd)Pd, SUM(Uprna)Uprna, SUM(Debt)Debt, SUM(Disc)Disc, SUM(Transp)Transp FROM ( SELECT pcol_date Dates, pcol_amts Amts, 0 Pd, 0 Uprna, 0 Debt, 0 Disc, 0 Transp FROM vDailySales WHERE pcol_station=@st AND YEAR(pcol_date)=@year AND MONTH(pcol_date)=@mnth UNION ALL SELECT ar_date, 0, ar_cash,0,0,0,0 FROM vLedgerCash WHERE ar_st=@st AND YEAR(ar_date)=@year AND MONTH(ar_date)=@mnth UNION ALL SELECT sr_date, CASE sr_overpump when 1 THEN sr_amts-(sr_discount) ELSE 0 END, 0,0,sr_amts, sr_discount,0 FROM vInvoicesLedger WHERE sr_st=@st AND YEAR(sr_date)=@year AND MONTH(sr_date)=@mnth UNION ALL SELECT am_date, 0,0,0,am_amts,0,0 FROM vAccounts WHERE am_st=@st AND YEAR(am_date)=@year AND MONTH(am_date)=@mnth UNION ALL SELECT ex_date, 0,ex_amts,0,0,0,0 FROM vLedgerExpenses WHERE ex_st=@st AND YEAR(ex_date)=@year AND MONTH(ex_date)=@mnth UNION ALL SELECT pt_date,0,0,0,0,0, pt_amount FROM vTransport WHERE pt_st=@st AND YEAR(pt_date)=@year AND MONTH(pt_date)=@mnth) As Foo GROUP BY Dates ORDER BY Dates");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    StationsReconcile recon = new StationsReconcile();
                    recon.Date = Convert.ToDateTime(dr[0]).ToString("dd/MM/yyyy");
                    recon.Amount = Convert.ToDouble(dr[1]);
                    recon.Payment = Convert.ToDouble(dr[2]);
                    recon.Uprna = Convert.ToDouble(dr[3]);
                    recon.Debt = Convert.ToDouble(dr[4]);
                    recon.Discount = Convert.ToDouble(dr[5]);
                    recon.Discount = Convert.ToDouble(dr[5]);
                    recon.Transport = Convert.ToDouble(dr[6]);
                    recon.Balance = recon.Amount - recon.Payment - recon.Uprna - recon.Debt - recon.Transport + recon.Discount;

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
            SqlDataReader dr = conn.SqlServerConnect("SELECT mm_idnt, mm_action, mm_account, mm_date, mm_desc, am_lpos, am_invs, mm_name, mm_price, mm_amount FROM vLedgerzEntry " + conn.GetQueryString(filter, "mm_desc+'-'+am_lpos+'-'+am_invs+'-'+mm_name+'-'+CAST(mm_amount AS NVARCHAR)", "mm_st=" + stid + " AND mm_date BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + AdditionalString + " ORDER BY mm_date, am_invs, am_lpos, mm_desc");
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
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date1 DATE='" + start.Date + "', @date2 DATE='" + stop.Date + "'; SELECT sr_idnt, sr_date, CASE sr_fuel WHEN 1 THEN 'LTS DIESEL' WHEN 2 THEN 'LTS SUPER' WHEN 3 THEN 'LTS VPOWER' WHEN 4 THEN 'LTS KEROSENE' ELSE 'OTHERS' END xdesc, sr_lpo, sr_invoice, sr_price, sr_amts, sr_cust, Names, sr_st, st_code, st_name FROM ( SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_invoice IN ( SELECT DISTINCT invs FROM vInvoicesDuplicates INNER JOIN vInvoicesLedger ON invs=sr_invoice WHERE sr_date BETWEEN @date1 AND @date2) UNION ALL SELECT sr_idnt, sr_st, sr_cust, sr_date, sr_fuel, sr_overpump, sr_lpo, sr_invoice, sr_price, sr_amts, sr_discount FROM vInvoicesLedger WHERE sr_date BETWEEN @date1 AND @date2 AND sr_invoice=0 ) As Foo INNER JOIN Stations ON st_idnt=sr_st INNER JOIN vCustomers ON sr_cust=Custid ORDER BY sr_invoice, sr_date, sr_fuel");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
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
    }
}
