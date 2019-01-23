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
        public IActionResult AddTruckFuelExpense()
        {
            CoreService core = new CoreService(HttpContext);
            TrucksFuelExpense expense = ExpenseModel.TrucksExpenses;

            DateTime date = DateTime.Parse(expense.DateString);
            VatResults vat = core.GetVatResults(date, new Fuel(1));

            expense.Date = date;
            expense.Amount = expense.Quantity * expense.Price;
            expense.Zerorated = vat.Zero * expense.Quantity;
            expense.VatAmount = (expense.Amount - expense.Zerorated)*(vat.Rate / (vat.Rate+100));
            expense.Description = "N/A";
            expense.Save(HttpContext);


            return LocalRedirect("/expenses/");
        }

        [HttpPost]
        public IActionResult AddStationExpense()
        {
            StationsExpenses expense = ExpenseModel.StationExpense;
            expense.Date = DateTime.Parse(expense.DateString);
            expense.Save(HttpContext);

            return LocalRedirect("/expenses/");
        }

        public JsonResult GetExpensesCore(string start, string stop, string filter, CoreService core) {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            return Json(core.GetExpensesCore(DateTime.Parse(start), DateTime.Parse(stop), filter));
        }

        public JsonResult GetStationsExpenses(string start, string stop, string filter, CoreService core)
        {
            if (string.IsNullOrWhiteSpace(filter))
                filter = "";

            return Json(core.GetStationsExpenses(DateTime.Parse(start), DateTime.Parse(stop), filter));
        }
    }
}
