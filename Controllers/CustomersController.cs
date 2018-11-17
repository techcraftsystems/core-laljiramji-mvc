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
        // GET: /<controller>/
        [Route("customers")]
        public IActionResult Index(CustomerServices svc)
        {
            List<Customers> customers = new List<Customers>(svc.GetCustomers(""));
            return View(customers);
        }

        [Route("customers/{station}/{id}")]
        public IActionResult Customers(String station, Int64 id, CustomerServices svc)
        {
            Customers customer = svc.GetCustomer(station, id);
            return View(customer);
        }


        [AllowAnonymous]
        public JsonResult GetLedgerEntries(Int64 custid, Int64 stid, string start, string stop, string filter, StationsService svc)
        {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<LedgerEntries> entries = svc.GetLedgerEntries(stid, DateTime.Parse(start), DateTime.Parse(stop), filter, custid);
            return Json(entries);
        }
    }
}
