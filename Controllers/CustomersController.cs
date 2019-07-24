using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace Core.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        [Route("customers")]
        public IActionResult Index(CoreService svc) {
            List<Customers> customers = new List<Customers>(svc.GetCustomers());
            return View(customers);
        }

        [Route("customers/{station}/{id}")]
        public IActionResult Customers(String station, Int64 id, CoreService svc) {
            Customers customer = svc.GetCustomer(station, id);
            return View(customer);
        }

        public JsonResult GetLedgerEntries(long custid, long stid, string start, string stop, string filter, StationsService svc) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<LedgerEntries> entries = svc.GetLedgerEntries(stid, DateTime.Parse(start), DateTime.Parse(stop), filter, custid);
            return Json(entries);
        }
    }
}
