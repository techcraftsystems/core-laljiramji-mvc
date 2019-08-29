using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Core.ViewModel;

namespace Core.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        [BindProperty]
        public CustomerViewModel Input { get; set; }

        [Route("/core/customers")]
        public IActionResult Index(CoreService svc) {
            List<Customers> customers = new List<Customers>(svc.GetCustomers());
            return View(customers);
        }

        [Route("/core/customers/{code}")]
        public IActionResult Station(string code, CoreService svc) {
            List<Customers> customers = new List<Customers>(svc.GetCustomers("", code));
            return View(customers);
        }

        [Route("/core/customers/{station}/{idnt}")]
        public IActionResult Customers(string station, long idnt, CustomerViewModel model, CoreService svc) {
            model.Customer = svc.GetCustomer(station, idnt);
            model.Customer.UpdateBalance();

            return View(model);
        }

        [HttpPost]
        public IActionResult PostCustomerPayments() {
            DateTime date = DateTime.Parse(Input.Date);
            Customers Customer = Input.Customer;

            foreach (var payment in Input.Payments) {
                if (payment.Receipt > 0) {
                    payment.Customer = Customer;
                    payment.PostDate = date;
                    payment.Save(HttpContext);
                }
            }

            Customer.UpdateBalance();
            return LocalRedirect("/core/customers/" + Customer.Station.Code + "/" + Customer.Id);
        }

        public IActionResult DeleteCustomersPayment(int idnt, string code, StationsService service) {
            CustomersPayments payment =  service.GetCustomerPayment(idnt, service.GetStation(code));
            payment.Delete();
            payment.Customer.UpdateBalance();

            return Ok("success");
        }

        public JsonResult GetLedgerEntries(long custid, long stid, string start, string stop, string filter, StationsService svc) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            List<LedgerEntries> entries = svc.GetLedgerEntries(stid, DateTime.Parse(start), DateTime.Parse(stop), filter, custid);
            return Json(entries);
        }

        public CustomersPayments GetCustomersPayment(long idnt, string code, StationsService service) {
            return service.GetCustomerPayment(idnt, service.GetStation(code));
        }
    }
}
