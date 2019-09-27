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
                if (int.Parse(payment.Receipt) > 0) {
                    payment.Customer = Customer;
                    payment.PostDate = date;
                    payment.Save(HttpContext);
                }
            }

            Customer.UpdateBalance();
            return LocalRedirect("/core/customers/" + Customer.Station.Code + "/" + Customer.Id);
        }

        [HttpPost]
        public IActionResult PostWithholdingTax() {
            DateTime date = DateTime.Parse(Input.Date);
            Customers cust = Input.Customer;

            foreach (var wht in Input.Withhold) {
                if (!string.IsNullOrEmpty(wht.Receipt) && wht.Amount > 0) {
                    wht.Customer = cust;
                    wht.Date = date;
                    wht.Save(HttpContext);
                }
            }

            cust.UpdateBalance();
            return LocalRedirect("/core/customers/" + cust.Station.Code + "/" + cust.Id);
        }

        [HttpPost]
        public IActionResult DeleteCustomersPayment(int idnt, string code, StationsService service) {
            CustomersPayments payment =  service.GetCustomerPayment(idnt, service.GetStation(code));
            payment.Delete();
            payment.Customer.UpdateBalance();

            return Ok("success");
        }

        [HttpPost]
        public IActionResult DeleteCustomersWithholding(int idnt, string code, StationsService service) {
            CustomersWithholding wht = service.GetCustomersWithholding(idnt, service.GetStation(code));
            wht.Delete();
            wht.Customer.UpdateBalance();

            return Ok("success");
        }

        public JsonResult GetCustomerPayments(string code, int idnt, string start, string stop, string filter = "") {
            StationsService Service = new StationsService();
            if (string.IsNullOrEmpty(filter))
                filter = "";

            return Json(Service.GetCustomerPayments(Service.GetStation(code), DateTime.Parse(start), DateTime.Parse(stop), new Customers(idnt), filter));
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

        public CustomersWithholding GetCustomersWithholding(long idnt, string code, StationsService service) {
            return service.GetCustomersWithholding(idnt, service.GetStation(code));
        }
    }
}
