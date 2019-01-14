using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        // GET: /<controller>/
        [Route("core/purchases")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("purchases/ledger/{code}")]
        public ActionResult FuelStationPurchaseLedger(String code)
        {
            return RedirectToAction("FuelPurchaseLedger", "Purchases", new { @station = code });
        }

        [Route("purchases/ledger")]
        public IActionResult FuelPurchaseLedger(String station, FuelPurchaseLedgerViewModel model, StationsService svc){
            model.Stations = new List<Stations>(svc.GetStationsByNames());
            if (!string.IsNullOrEmpty(station))
                model.Code = station;

            return View(model);
        }

        [Route("purchases/vat/calculator")]
        public IActionResult VatCalculator(VatCalculatorViewModel model, PurchasesService svc)
        {
            List<PurchasesVat> VatEntries = new List<PurchasesVat>(svc.GetLatestPurchasesVat());
            model.Diesel = VatEntries[0];
            model.Super = VatEntries[1];
            model.Vpower = VatEntries[2];
            model.Kerosene = VatEntries[3];

            return View(model);
        }

        public Double GetFuelPurchasesLedgerOpenning(Int64 stid, string date, PurchasesService svc)
        {
            return svc.GetFuelPurchasesLedgerOpenning(stid, DateTime.Parse(date));
        }

        public JsonResult GetFuelPurchasesLedgers(Int64 stid, string start, string stop, string filter, PurchasesService svc)
        {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<FuelPurchasesLedger> ledgers = svc.GetFuelPurchasesLedgers(stid, DateTime.Parse(start), DateTime.Parse(stop), filter);
            return Json(ledgers);
        }
    }
}
