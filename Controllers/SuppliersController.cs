using System;
using Core.Models;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers 
{
    [Authorize]
    public class SuppliersController : Controller 
    {
        private readonly CoreService Core = new CoreService();

        [BindProperty]
        public SuppliersViewModel Input { get; set; }

        [Route("/core/suppliers")]
        public IActionResult Index() {
            Core.UpdateSupplierBalance();
            return View(Core.GetSuppliers());
        }

        [Route("/core/suppliers/{uuid}")]
        public IActionResult Supplier(string uuid, SuppliersViewModel model, AccountsService accounts) {
            model.Supplier = Core.GetSupplier(uuid);
            model.BankAccounts = accounts.GetBankAccountsIEnumerable();
            model.Stations = Core.GetStationsIEnumerable(true);
            model.Category = Core.GetExpenseCategoriesIEnumerable();

            return View(model);
        }

        [Route("/core/suppliers/payments/ledger/{month}/{year}")]
        public IActionResult PaymentLedger(int month, int year, SuppliersPaymentsViewModel model) {
            model.Date = new DateTime(year, month, 1);
            return View(model);
        }

        [Route("/core/suppliers/payments/schedule/{month}/{year}")]
        public IActionResult PaymentSchedule(int month, int year, SuppliersPaymentsViewModel model) {
            model.Date = new DateTime(year, month, 1);
            model.Schedule = Core.GetSuppliersPaymentSchedules(model.Date, model.Date.AddMonths(1).AddDays(-1), null);

            return View(model);
        }

        [HttpPost]
        public IActionResult PostSupplierPayments() {
            DateTime date = DateTime.Parse(Input.Date);
            Suppliers supp = Input.Supplier;

            foreach (var payment in Input.Payments) {
                if (!string.IsNullOrEmpty(payment.Cheque)) {
                    payment.Supplier = supp;
                    payment.Date = date;
                    payment.Save(HttpContext);
                }
            }

            Core.UpdateSupplierBalance(supp);
            return LocalRedirect("/core/suppliers/" + supp.Uuid);
        }

        [HttpPost]
        public IActionResult DeleteSuppliersPayment(int idnt, int supp) {
            new SuppliersPayment(idnt).Delete();
            Core.UpdateSupplierBalance(new Suppliers(supp));
            return Ok("success");
        }

        [HttpPost]
        public IActionResult PostSupplierInvoice() {
            DateTime date = DateTime.Parse(Input.Date);
            Suppliers supp = Input.Supplier;

            foreach (var invoice in Input.Invoices) {
                if (!string.IsNullOrEmpty(invoice.Invoice)) {
                    invoice.Supplier = supp;
                    invoice.Date = date;
                    invoice.Save(HttpContext);
                }
            }

            Core.UpdateSupplierBalance(supp);
            return LocalRedirect("/core/suppliers/" + supp.Uuid);
        }

        public IActionResult DeleteSuppliersInvoice(int idnt, int supp) {
            new StationsExpenses(idnt).Delete();
            Core.UpdateSupplierBalance(new Suppliers(supp));
            return Ok("success");
        }

        [HttpPost]
        public IActionResult PostSupplierCredits() {
            DateTime date = DateTime.Parse(Input.Date);
            Suppliers supp = Input.Supplier;

            foreach (var note in Input.Credits) {
                if (!string.IsNullOrEmpty(note.Receipt)) {
                    note.Supplier = supp;
                    note.Date = date;
                    note.Save(HttpContext);
                }
            }

            Core.UpdateSupplierBalance(supp);
            return LocalRedirect("/core/suppliers/" + supp.Uuid);
        }

        public IActionResult DeleteSuppliersCredit(int idnt, int supp) {
            new SuppliersCredits(idnt).Delete();
            Core.UpdateSupplierBalance(new Suppliers(supp));
            return Ok("success");
        }

        public SuppliersPayment GetSuppliersPayment(int idnt) {
            return Core.GetSuppliersPayment(idnt);
        }

        public StationsExpenses GetSuppliersInvoice(int idnt) {
            return Core.GetStationsExpenses(idnt);
        }

        public SuppliersCredits GetSuppliersCredit(int idnt)         {
            return Core.GetSuppliersCredit(idnt);
        }

        public JsonResult GetSupplierExpenses(long supp, string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(Core.GetCoreExpenses(DateTime.Parse(start), DateTime.Parse(stop), new Suppliers(supp), filter));
        }

        public JsonResult GetSuppliersPayments(long supp, string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(Core.GetSuppliersPayment(DateTime.Parse(start), DateTime.Parse(stop), new Suppliers(supp), filter));
        }

        public JsonResult GetSuppliersCredits(long supp, string start, string stop, string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            return Json(Core.GetSuppliersCredits(DateTime.Parse(start), DateTime.Parse(stop), new Suppliers(supp), filter));
        }
    }
}
