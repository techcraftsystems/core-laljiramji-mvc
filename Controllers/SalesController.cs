using System;
using System.Collections.Generic;
using Core.Models;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [Authorize]
    public class SalesController : Controller {
        [BindProperty]
        public SalesDeliveriesViewModel Delvs { get; set; }

        [Route("sales")]
        public IActionResult Index() {
            return View();
        }

        [Route("sales/deliveries")]
        public IActionResult Deliveries(SalesDeliveriesViewModel model, StationsService service, AccountsService accounts) {
            model.StationIdnts = service.GetStationIdntsIEnumerable();
            model.StationCodes = service.GetStationCodesIEnumerable();
            model.DeliveryType = service.GetDeliveryTypesIEnumerable();
            model.BankAccounts = accounts.GetBankAccountsIEnumerable();
            model.PettyAccounts = accounts.GetPettyCashAccountsAutocomplete();
            model.Deliveries = service.GetDeliveries(model.Date, DateTime.Now);

            return View(model);
        }

        [Route("sales/transfers/ledger")]
        public IActionResult TransfersLedger(SalesTransferLedgerViewModel model) {
            DateTime Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.Ledger = new StationsService().GetProductsTransfers(Date, DateTime.Now);

            return View(model);
        }

        [HttpPost] 
        public IActionResult PostNewDeliveries() {
            Stations station = Delvs.Station;
            DateTime date = DateTime.Parse(Delvs.DateX);

            foreach (var delv in Delvs.Entries) {
                if (!string.IsNullOrEmpty(delv.Receipt)) { 
                    delv.Station = station;
                    delv.Date = date;
                    delv.Save(HttpContext);                
                }
            }

            return LocalRedirect("/sales/deliveries");
        }

        public IActionResult DeleteDelivery(int idnt) {
            Delivery delivery = new Delivery(idnt);
            delivery.Delete();
            
            return LocalRedirect("/sales/deliveries");
        }

        public Delivery GetDelivery(int idnt, StationsService service) {
            return service.GetDelivery(idnt);
        }

        public JsonResult GetDeliveries(string start, string stop, string filter, StationsService service) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(service.GetDeliveries(DateTime.Parse(start), DateTime.Parse(stop), filter));
        }

        public JsonResult GetProductsTransfers(string start, string stop, string filter, StationsService service) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(service.GetProductsTransfers(DateTime.Parse(start), DateTime.Parse(stop), filter));
        }
    }
}
