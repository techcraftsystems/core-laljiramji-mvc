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
            List<FuelPurchasesLedger> ledgers = new List<FuelPurchasesLedger>();

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
            List<FuelLedgerSummary> ledger = new List<FuelLedgerSummary>();

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
    }
}
