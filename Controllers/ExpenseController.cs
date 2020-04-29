using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DataModel;
using Core.Models;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        [BindProperty]
        public ExpenseIndexViewModel ExpenseModel { get; set; }

        [Route("expenses")]
        public IActionResult Index(ExpenseIndexViewModel model, CoreService core) {
            model.Suppliers = core.GetSuppliersIEnumerable();
            model.Trucks = core.GetTrucksIEnumerable();
            model.Stations = core.GetStationsIEnumerable(true);
            model.Categories = core.GetExpenseCategoriesIEnumerable();
            model.Expenses = new List<ExpensesCore>(core.GetExpensesCore(model.Date1x, model.Date2x));

            return View(model);
        }

        [Route("expenses/ledger")]
        public IActionResult Ledger(ExpensesLedgerViewModel model, CoreService core)
        {
            model.Stations = core.GetStationsIEnumerable();
            model.Expenses = core.GetStationsExpenses(model.StartDate, model.StopsDate);

            return View(model);
        }

        [HttpPost]
        public IActionResult AddTruckFuelExpense() {
            CoreService core = new CoreService(HttpContext);

            DateTime date = DateTime.Parse(ExpenseModel.DateX);
            VatResults vat = core.GetVatResults(date, new Fuel(1));

            foreach(var expense in ExpenseModel.TrucksFuel) {
                if (string.IsNullOrEmpty(expense.Invoice))
                    continue;

                expense.Date = date;
                expense.Amount = expense.Quantity * expense.Price;
                expense.Zerorated = expense.Amount - expense.VatAmount - (expense.VatAmount/0.08);
                expense.Description = "N/A";
                expense.Save(HttpContext);
            }

            return LocalRedirect("/expenses/");
        }

        [HttpPost]
        public IActionResult AddStationExpense() {
            StationsExpenses expense = ExpenseModel.StationExpense;
            expense.Date = DateTime.Parse(expense.DateString);
            expense.Save(HttpContext);

            return LocalRedirect("/expenses/");
        }

        public JsonResult GetExpensesCore(string start, string stops, string filter, int source, CoreService core) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            return Json(core.GetExpensesCore(DateTime.Parse(start), DateTime.Parse(stops), filter, source));
        }

        public JsonResult GetStationsExpenses(string start, string stop, CoreService core, string stations = "", string filter = "") {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";
            if (string.IsNullOrWhiteSpace(stations))
                stations = "";

            return Json(core.GetStationsExpenses(DateTime.Parse(start), DateTime.Parse(stop), stations, filter));
        }

        public JsonResult GetTrucksFuelExpense(int idnt, CoreService core) {
            return Json(core.GetTrucksFuelExpense(idnt));
        }

        [Authorize(Roles = "Super User")]
        public IActionResult DeleteFuelExpense(int idnt, CoreService core) {
            new TrucksFuelExpense(idnt).Delete();

            return Ok("success");
        }
    }
}
