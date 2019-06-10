using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Models;
using Core.Extensions;
using Core.DataModel;

namespace Core.Services
{
    public class PurchasesService
    {
        public List<Purchases> GetPurchases(DateTime date1x, DateTime date2x, string category, Stations station, Suppliers supplier, string filter = "")
        {
            List<Purchases> purchases = new List<Purchases>();
            SqlServerConnection conn = new SqlServerConnection();

            string query = conn.GetQueryString(filter, "CAST(PurNum AS NVARCHAR)+'-'+ISNULL(Names,'CASH PURCHASE')+'-'+SuppInv+'-'+Category+'-'+CAST(qty*price AS NVARCHAR)+'-'+Names+'-'+st_name+'-'+st_synonym", "Date BETWEEN '" + date1x.Date + "' AND '" + date2x.Date + "'");
            if (!string.IsNullOrEmpty(category))
                query += " AND Category='" + category + "'";
            if (!(station is null))
                query += " AND st_idnt=" + station.Id;
            if (!(supplier is null))
                query += " AND Suppid=" + supplier.Id;

            SqlDataReader dr = conn.SqlServerConnect("SELECT PurNum, Date, Lpo, SuppInv, MAX(Category)Catg, SUM(qty*price) Amts, Supp, ISNULL(Names,'CASH PURCHASE')Names, st_idnt, st_code, st_name, st_synonym FROM vPurchasesAll INNER JOIN Stations ON Stns=st_idnt LEFT OUTER JOIN vSuppliers ON Suppid=Supp AND Stn=Stns " + query + " GROUP BY PurNum, Date, Lpo, SuppInv, Supp, Names, st_idnt, st_code, st_name, st_order, st_synonym ORDER BY Date, SuppInv, st_order, PurNum");
            if (dr.HasRows) {
                while (dr.Read()) {
                    purchases.Add(new Purchases {
                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]),
                        DateString = Convert.ToDateTime(dr[1]).ToString("dd/MM/yyyy"),
                        Lpo = dr[2].ToString(),
                        Invoice = dr[3].ToString(),
                        Category = dr[4].ToString(),
                        Amount = Convert.ToDouble(dr[5]),
                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[6]),
                            Name = dr[7].ToString()
                        },
                        Station = new Stations {
                            Id = Convert.ToInt64(dr[8]),
                            Code = dr[9].ToString(),
                            Name = dr[10].ToString(),
                            Synonym = dr[11].ToString(),
                        }
                    });
                }
            }

            return purchases;
        }

        public List<VatDownloadEntries> GetPurchases00PercEntries(int month, int year) {
            List<VatDownloadEntries> entries = new List<VatDownloadEntries>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @year INT=" + year + ", @mnth INT=" + month + "; SELECT * FROM (SELECT [Date], SuppInv, 'FUEL PURCHASE' xDesc, SUM((qty*price)-CAST(tax_amount As FLOAT)/0.08-tax_amount) Vatable, ISNULL(sp_name,'CASH PURCHASE') Supplier, ISNULL(sp_pin,'N/A') Pin FROM Suppliers INNER JOIN SuppliersMap ON sp_idnt=sm_mapp RIGHT OUTER JOIN vPurchasesAll ON sm_station=Stns AND sm_code=Supp WHERE tax=8 AND YEAR(Date)=@year AND MONTH(Date)=@mnth GROUP BY sp_pin, sp_name, [Date], SuppInv UNION ALL SELECT tf_date, tf_invoice, 'MOTOR VEHICLE FUEL' x, tf_amount-(tf_vatamts/0.08)-tf_vatamts Vatable, sp_name, sp_pin FROM TrucksFuel INNER JOIN Suppliers ON tf_supplier=sp_idnt WHERE YEAR(tf_date)=@year AND MONTH(tf_date)=@mnth UNION ALL SELECT [Date], SuppInv, 'L.P.G PURCHASES' xDesc, SUM((qty*price)) Vatable, ISNULL(sp_name,'CASH PURCHASE') Supplier, ISNULL(sp_pin,'N/A') Pin FROM Suppliers INNER JOIN SuppliersMap ON sp_idnt=sm_mapp RIGHT OUTER JOIN vPurchasesAll ON sm_station=Stns AND sm_code=Supp WHERE tax=0 AND YEAR(Date)=@year AND MONTH(Date)=@mnth GROUP BY sp_pin, sp_name, [Date], SuppInv, Category) As Foo ORDER BY xDesc, [Date], SuppInv");
            if (dr.HasRows) {
                while (dr.Read()) {
                    entries.Add(new VatDownloadEntries {
                        Date = Convert.ToDateTime(dr[0]).ToString("dd/MM/yyyy"),
                        Invoice = dr[1].ToString(),
                        Description = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Supplier = new Suppliers {
                            Name = dr[4].ToString(),
                            Pin = dr[5].ToString()
                        },
                    });
                }
            }

            return entries;
        }

        public List<VatDownloadEntries> GetPurchases08PercEntries(int month, int year) {
            List<VatDownloadEntries> entries = new List<VatDownloadEntries>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @year INT=" + year + ", @mnth INT=" + month + "; SELECT * FROM (SELECT [Date], SuppInv, 'FUEL PURCHASE' xDesc, SUM(CAST(tax_amount As FLOAT)/0.08) Vatable, ISNULL(sp_name,'CASH PURCHASE') Supplier, ISNULL(sp_pin,'N/A') Pin FROM Suppliers INNER JOIN SuppliersMap ON sp_idnt=sm_mapp RIGHT OUTER JOIN vPurchasesAll ON sm_station=Stns AND sm_code=Supp WHERE tax=8 AND YEAR(Date)=@year AND MONTH(Date)=@mnth GROUP BY sp_pin, sp_name, [Date], SuppInv UNION ALL SELECT tf_date, tf_invoice, 'MOTOR VEHICLE FUEL' x, (tf_vatamts/0.08)Vatable, sp_name, sp_pin FROM TrucksFuel INNER JOIN Suppliers ON tf_supplier=sp_idnt WHERE YEAR(tf_date)=@year AND MONTH(tf_date)=@mnth) As Foo ORDER BY xDesc, [Date], SuppInv");
            if (dr.HasRows) {
                while (dr.Read()) {
                    entries.Add(new VatDownloadEntries {
                        Date = Convert.ToDateTime(dr[0]).ToString("dd/MM/yyyy"),
                        Invoice = dr[1].ToString(),
                        Description = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Supplier = new Suppliers {
                            Name = dr[4].ToString(),
                            Pin = dr[5].ToString()
                        },
                    });
                }
            }

            return entries;
        }

        public List<VatDownloadEntries> GetPurchases16PercEntries(int month, int year) {
            List<VatDownloadEntries> entries = new List<VatDownloadEntries>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @year INT=" + year + ", @mnth INT=" + month + "; SELECT * FROM (SELECT [Date], SuppInv, CASE WHEN Category='GAS' THEN 'L.P.G' WHEN Category='SODA' THEN 'DRINKS' ELSE Category END +' PURCHASES' xDesc, SUM((qty*price)-((qty*price)*(16.0/116.0))) Vatable, ISNULL(sp_name,'CASH PURCHASE') Supplier, ISNULL(sp_pin,'N/A') Pin FROM Suppliers INNER JOIN SuppliersMap ON sp_idnt=sm_mapp RIGHT OUTER JOIN vPurchasesAll ON sm_station=Stns AND sm_code=Supp WHERE tax=16 AND YEAR(Date)=@year AND MONTH(Date)=@mnth AND NOT (Category='LUBES' AND Stns IN (2,3,6,9)) GROUP BY sp_pin, sp_name, [Date], SuppInv, Category UNION ALL SELECT xp_date, xp_invoice, ec_category, xp_amount-xp_vat_amts xp_vatable, sp_name, ISNULL(NULLIF(sp_pin,''),'N/A') FROM Expenses INNER JOIN ExpensesCategory ON xp_category=ec_idnt INNER JOIN Suppliers ON sp_idnt=xp_supplier WHERE YEAR(xp_date)=@year AND MONTH(xp_date)=@mnth AND xp_vat_amts<>0 ) As Foo ORDER BY [Date], SuppInv");
            if (dr.HasRows) {
                while (dr.Read()) {
                    entries.Add(new VatDownloadEntries {
                        Date = Convert.ToDateTime(dr[0]).ToString("dd/MM/yyyy"),
                        Invoice = dr[1].ToString(),
                        Description = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Supplier = new Suppliers {
                            Name = dr[4].ToString(),
                            Pin = dr[5].ToString()
                        },
                    });
                }
            }

            return entries;
        }


        public List<PurchasesVat> GetLatestPurchasesVat()
        {
            List<PurchasesVat> Entries = new List<PurchasesVat>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @date DATE = (SELECT MAX(pv_date) dt FROM PurchaseVATCalc); SELECT pv_idnt, pv_date, pv_product, pv_landing, pv_pipeline, pv_road, pv_pipeloss, pv_depotloss, pv_delivery, (pv_pipeline+ pv_road+pv_pipeloss+pv_depotloss+pv_delivery) B_STORAGE_DIST, pv_margin_import, pv_margin_dealer, (pv_margin_import+pv_margin_dealer) C_MARGINS, pv_excise_duty, pv_merchant_levy, pv_declaration_fee, pv_road_maintainance, pv_devt_petroleum, pv_regu_petroleum, pv_devt_railway, pv_vat, (pv_excise_duty+pv_merchant_levy+pv_declaration_fee+pv_road_maintainance+pv_devt_petroleum+pv_regu_petroleum+pv_devt_railway+pv_vat) D_TAX_LEVIES, pv_landing+(pv_pipeline+ pv_road+pv_pipeloss+pv_depotloss+pv_delivery)+(pv_margin_import+pv_margin_dealer)+(pv_excise_duty+pv_merchant_levy+pv_declaration_fee+pv_road_maintainance+pv_devt_petroleum+pv_regu_petroleum+pv_devt_railway+pv_vat) E_RETAIL, (pv_pipeloss+pv_depotloss+pv_merchant_levy+pv_declaration_fee+pv_road_maintainance+pv_devt_petroleum+pv_regu_petroleum+pv_devt_railway) F_EXEMPT FROM PurchaseVATCalc WHERE pv_date=@date ORDER BY pv_product");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    PurchasesVat Entry = new PurchasesVat();
                    Entry.Id = Convert.ToInt64(dr[0]);
                    Entry.Date = Convert.ToDateTime(dr[1]);
                    Entry.Product.Id = Convert.ToInt64(dr[2]);

                    Entry.LandingCost = Convert.ToDouble(dr[3]);
                    Entry.Pipeline = Convert.ToDouble(dr[4]);
                    Entry.Roads = Convert.ToDouble(dr[5]);
                    Entry.PipesLoss = Convert.ToDouble(dr[6]);
                    Entry.DepotLoss = Convert.ToDouble(dr[7]);
                    Entry.Delivery = Convert.ToDouble(dr[8]);
                    Entry.StorageDist = Convert.ToDouble(dr[9]);

                    Entry.MarginImport = Convert.ToDouble(dr[10]);
                    Entry.MarginDealer = Convert.ToDouble(dr[11]);
                    Entry.MarginSummary = Convert.ToDouble(dr[12]);

                    Entry.ExciseDuty = Convert.ToDouble(dr[13]);
                    Entry.MerchantLevy = Convert.ToDouble(dr[14]);
                    Entry.DeclarationFee = Convert.ToDouble(dr[15]);
                    Entry.RoadsMaintance = Convert.ToDouble(dr[16]);
                    Entry.DevtPetroleum = Convert.ToDouble(dr[17]);
                    Entry.ReguPetroleum = Convert.ToDouble(dr[18]);
                    Entry.DevtRailway = Convert.ToDouble(dr[19]);
                    Entry.ValueAddedTax = Convert.ToDouble(dr[20]);
                    Entry.TaxesAndLevies = Convert.ToDouble(dr[21]);
                    Entry.RetailPrice = Convert.ToDouble(dr[22]);
                    Entry.TaxExempts = Convert.ToDouble(dr[23]);
                    Entry.TaxableAmts = Entry.RetailPrice - Entry.TaxExempts;

                    Entries.Add(Entry);
                }
            }

            return Entries;
        }

        public Double GetFuelPurchasesLedgerOpenning(Int64 stid, DateTime date){
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ISNULL(SUM(total-pdo1-pdo2),0) FROM vPurchasesLedger WHERE st=" + stid + " AND dt < '" + date.Date + "'");
            if (dr.Read())
            {
                return Convert.ToDouble(dr[0]);
            }

            return 0;
        }

        public List<FuelPurchasesLedger> GetFuelPurchasesLedgers(long stid, DateTime start, DateTime stop, string filter){
            var ledgers = new List<FuelPurchasesLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT id, dt, qty, price, descr, invs, total, tax_amount, pdo1, pdo2, pdon, st_name, sb_brand FROM vPurchasesLedger INNER JOIN Stations ON st_idnt=st INNER JOIN StationsBrand ON sb_idnt=st_brand " + conn.GetQueryString(filter, "CAST(qty AS NVARCHAR)+'-'+descr+'-'+invs+'-'+CAST(total AS NVARCHAR)+'-'+CAST(pdo1 AS NVARCHAR)+'-'+CAST(pdo2 AS NVARCHAR)", "st=" + stid + " AND dt BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + " ORDER BY dt, descr, id");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    FuelPurchasesLedger item = new FuelPurchasesLedger();
                    item.Station.Id = stid;

                    item.Id = Convert.ToInt64(dr[0]);
                    item.Date = Convert.ToDateTime(dr[1]).ToString("dd-MMM");
                    item.Ltrs = Convert.ToDouble(dr[2]);
                    item.Price = Convert.ToDouble(dr[3]);

                    item.Description = dr[4].ToString();
                    item.Invoice = dr[5].ToString();

                    item.Total = Convert.ToDouble(dr[6]);
                    item.Vats = Convert.ToDouble(dr[7]);
                    item.PayCard = Convert.ToDouble(dr[8]);
                    item.PayAmts = Convert.ToDouble(dr[9]);

                    if (!String.IsNullOrEmpty(dr[10].ToString()))
                        item.PayDate = Convert.ToDateTime(dr[10].ToString()).ToString("dd-MMM");
                        
                    item.Station.Name = dr[11].ToString().ToUpper();
                    item.Station.Brand.Name = dr[12].ToString().ToUpper();

                    item.Excl = (item.Vats / 0.08);
                    item.Zero = item.Total - item.Vats - item.Excl;

                    ledgers.Add(item);
                }
            }

            return ledgers;
        }

        public List<FuelLedgerSummary> GetFuelPurchasesLedgersSummary(DateTime start, DateTime stop, string filter)
        {
            var ledger = new List<FuelLedgerSummary>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT st_name, SUM(CASE WHEN item_id=1 THEN total ELSE 0 END)DX, SUM(CASE WHEN item_id=2 THEN total ELSE 0 END)uX, SUM(CASE WHEN item_id=3 THEN total ELSE 0 END)vP, SUM(CASE WHEN item_id=4 THEN total ELSE 0 END)IK, SUM(total) TOTAL, SUM(tax_amount) TAX, SUM((tax_amount/0.08))ex_vat, SUM(total-tax_amount-(tax_amount/0.08)) zero_amount FROM vPurchasesLedger INNER JOIN Stations ON st_idnt=st INNER JOIN StationsBrand ON sb_idnt=st_brand " + conn.GetQueryString(filter, "st_name", "dt BETWEEN '" + start.Date + "' AND '" + stop.Date + "'") + " GROUP BY st_name, st_order ORDER BY st_order");
            if (dr.HasRows) {
                while (dr.Read()) {
                    ledger.Add(new FuelLedgerSummary {
                        Name = dr[0].ToString(),
                        Dx = Convert.ToDouble(dr[1]),
                        Ux = Convert.ToDouble(dr[2]),
                        Vp = Convert.ToDouble(dr[3]),
                        Ik = Convert.ToDouble(dr[4]),
                        Total = Convert.ToDouble(dr[5]),
                        Vats = Convert.ToDouble(dr[6]),
                        Excl = Convert.ToDouble(dr[7]),
                        Zero = Convert.ToDouble(dr[8]),
                    });
                }
            }

            return ledger;
        }

        public List<FuelPurchasesLedger> GetStocksPurchasesLedgers(Stations station, DateTime start, DateTime stop, string filter) {
            var ledger = new List<FuelPurchasesLedger>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.", "") + "; DECLARE @start DATE='" + start.Date + "', @stop DATE='" + stop.Date + "';SELECT id, date_, qty, price, tax, Category, Items, SuppInv, Supp, ISNULL(Names,'CASH')Names FROM PurchasesDetails INNER JOIN pProducts ON id_=item_id INNER JOIN Purchases ON PurNum=PurNo LEFT OUTER JOIN Suppliers ON Supp=Suppid " + conn.GetQueryString(filter, "CAST(qty*price AS NVARCHAR)+'-'+Category+'-'+Items+'-'+SuppInv+'-'+ISNULL(Names,'CASH')", "id_>10 AND date_ BETWEEN @start AND @stop") + " ORDER BY date_, SuppInv, Items");
            if (dr.HasRows) {
                while (dr.Read()) {
                    FuelPurchasesLedger item = new FuelPurchasesLedger {
                        Station = station,

                        Id = Convert.ToInt64(dr[0]),
                        Date = Convert.ToDateTime(dr[1]).ToString("dd-MMM"),
                        Ltrs = Convert.ToDouble(dr[2]),
                        Price = Convert.ToDouble(dr[3]),
                        Rate = Convert.ToDouble(dr[4]),
                        Category = dr[5].ToString(),
                        Description = dr[6].ToString(),
                        Invoice = dr[7].ToString(),

                        Supplier = new Suppliers {
                            Id = Convert.ToInt64(dr[8]),
                            Name = dr[9].ToString()
                        }
                    };

                    item.Total = item.Ltrs * item.Price;

                    if (item.Rate.Equals(0))
                        item.Zero = item.Total;
                    else
                        item.Vats = (item.Rate / (item.Rate * 100)) * item.Total;

                    item.Excl = item.Total - item.Vats - item.Zero;
                    ledger.Add(item);
                }
            }

            return ledger;
        }
    }
}
