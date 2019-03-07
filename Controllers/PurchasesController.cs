using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        [BindProperty]
        public FuelPriceChangeViewModel PricesUpdate { get; set; }

        [Route("purchases")]
        public IActionResult Index(PurchaseViewModel model, PurchasesService service) {
            model.Purchases = service.GetPurchases(model.Date1x, model.Date2x, null, null, null);

            return View(model);
        }

        [Route("purchases/ledger")]
        public IActionResult FuelLedger(String station, FuelPurchaseLedgerViewModel model, StationsService svc){
            model.Stations = new List<Stations>(svc.GetStationsNames());
            if (!string.IsNullOrEmpty(station))
                model.Code = station;

            return View(model);
        }

        [Route("purchases/ledger/summary")]
        public IActionResult FuelLedgerSummary() {
            return View();
        }

        [Route("purchases/price/change")]
        public IActionResult FuelPriceChange(FuelPriceChangeViewModel model, CoreService service)
        {
            model.Previous = new List<FuelPriceChange>(service.GetLastPriceChange());
            model.StartDate = model.Previous[0].Date.AddDays(1);
            model.StopsDate = model.StartDate.AddMonths(1);
            model.StopsDate = new DateTime(model.StopsDate.Year, model.StopsDate.Month, 14);

            return View(model);
        }

        [Route("purchases/vat/calculator")]
        public IActionResult VatCalculator(VatCalculatorViewModel model, PurchasesService svc) {
            List<PurchasesVat> VatEntries = new List<PurchasesVat>(svc.GetLatestPurchasesVat());
            model.Diesel = VatEntries[0];
            model.Super = VatEntries[1];
            model.Vpower = VatEntries[2];
            model.Kerosene = VatEntries[3];

            return View(model);
        }

        public IActionResult UpdatePriceChange(StationsService service) {
            for (DateTime date = PricesUpdate.StartDate; date.Date <= PricesUpdate.StopsDate.Date; date = date.AddDays(1)) {
                foreach (var update in PricesUpdate.Previous){
                    update.Date = date;
                    update.Save();
                }
            }

            foreach (Stations station in service.GetStationsNames()) {
                foreach (var update in PricesUpdate.Previous){
                    service.UpdateStationsZeroRate(station, update);
                }
            }

            return LocalRedirect("/");
        }

        public JsonResult GetPurchases(string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(new PurchasesService().GetPurchases(DateTime.Parse(start), DateTime.Parse(stop), null, null, null, filter));
        }

        public Double GetFuelPurchasesLedgerOpenning(Int64 stid, string date, PurchasesService svc) {
            return svc.GetFuelPurchasesLedgerOpenning(stid, DateTime.Parse(date));
        }

        public JsonResult GetFuelPurchasesLedgers(Int64 stid, string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(new PurchasesService().GetFuelPurchasesLedgers(stid, DateTime.Parse(start), DateTime.Parse(stop), filter));
        }

        public JsonResult GetFuelPurchasesLedgersSummary(string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(new PurchasesService().GetFuelPurchasesLedgersSummary(DateTime.Parse(start), DateTime.Parse(stop), filter)); ;
        }
    }
}
