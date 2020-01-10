using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.ViewModel;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace Core.Controllers
{
    [Authorize]
    public class StationsController : Controller
    {
        // Index should redirect
        [Route("core/stations")]
        public ActionResult Index(StationsService svc)
        {
            StationsMainViewModel model = new StationsMainViewModel();
            model.Stations = svc.GetStations();

            return View(model);
        }

        [Route("core/stations/{name}")]
        public IActionResult Main(String name, StationsService svc, CoreService csv)
        {
            StationsListViewModel model = new StationsListViewModel();
            model.Selected = svc.GetStation(name);
            model.Stations = svc.GetStations();
            model.Totals = svc.GetLedgerTotals(model.Selected, model.Selected.Push);
            model.Readings = svc.GetMetreReadings(model.Selected, model.Selected.Push);
            model.Summaries = svc.GetSummaries(model.Selected, model.Selected.Push);
            model.Ledgers = svc.GetCustomersSummaries(model.Selected, model.Selected.Push);
            model.Customers = csv.GetCustomers(model.Selected.Name);

            return View(model);
        }

        [Route("core/stations/ledgers/duplicates")]
        public ActionResult LedgersDuplicates(StationsLedgersDuplicatesViewModel model, StationsService svc)
        {
            model.Stations = new List<Stations>(svc.GetStations());
            return View(model);
        }

        [AllowAnonymous]
        public JsonResult GetStationsSummary(String date1, String date2, String stats, StationsService svc)
        {
            DateTime xdate1 = DateTime.Now.AddDays(-1);
            DateTime xdate2 = DateTime.Now.AddDays(-1);

            //DateTime.ParseExact(date1, "d MMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(date1))
                xdate1 = DateTime.Parse(date1);

            if (!string.IsNullOrEmpty(date2))
                xdate2 = DateTime.Parse(date2);

            if (string.Equals(stats, "Select Stations"))
                stats = "";
            else {
                string[] stations = stats.Split(',', StringSplitOptions.RemoveEmptyEntries);
                stats = "";

                for (int i = 0; i < stations.Length; i++)
                {
                    if (string.IsNullOrEmpty(stats))
                    {
                        stats = "'" + stations[i].Trim() + "'";
                    }
                    else
                    {
                        stats += ",'" + stations[i].Trim() + "'";
                    }
                }
            }

            List<LedgerTotals> totals = new List<LedgerTotals>(svc.GetLedgerTotals(stats, xdate1, xdate2));
            return Json(totals);
        }

        public JsonResult GetStationsReconciles(Int64 stid, Int64 year, Int64 mnth, StationsService svc)
        {
            List<StationsReconcile> reconciles = svc.GetStationsReconciles(stid, year, mnth);
            return Json(reconciles);
        }

        public JsonResult GetLedgerEntries(Int64 stid, string start, string stop, string filter, StationsService svc){
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<LedgerEntries> entries = svc.GetLedgerEntries(stid, DateTime.Parse(start), DateTime.Parse(stop), filter);
            return Json(entries);
        }

        public JsonResult GetLedgerDuplicates(string start, string stop, StationsService service)
        {
            List<LedgerEntries> entries = service.GetLedgerDuplicates(DateTime.Parse(start), DateTime.Parse(stop));
            return Json(entries);
        }

        public JsonResult GetExpenditure(Int64 stid, string start, string stop, string filter, StationsService svc)
        {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<Expenses> expenses = svc.GetExpenditure(stid, DateTime.Parse(start), DateTime.Parse(stop), filter);
            return Json(expenses);
        }

        public JsonResult GetPurchasesOthers(long stid, string start, string stop, string filter, StationsService svc) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            return Json(svc.GetPurchasesOthers(stid, DateTime.Parse(start), DateTime.Parse(stop), filter));
        }

        public JsonResult GetPurchasesLedger(long stid, string start, string stop, string filter, StationsService svc) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<PurchasesLedger> ledgers = svc.GetPurchasesLedger(stid, DateTime.Parse(start), DateTime.Parse(stop), filter);
            return Json(ledgers);
        }

        public JsonResult GetCustomerPayments(string date1, string date2, string stations, string customers, string filter, StationsService service)
        {
            DateTime xdate1 = DateTime.Parse(date1);
            DateTime xdate2 = DateTime.Parse(date2);

            if (string.IsNullOrEmpty(customers))
                customers = "";

            if (string.IsNullOrEmpty(filter))
                filter = "";

            List<CustomersPayments> payments = new List<CustomersPayments>(service.GetCustomerPayments(xdate1, xdate2, stations, customers, filter));
            return Json(payments);
        }
    }
}
