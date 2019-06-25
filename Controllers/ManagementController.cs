using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.ViewModel;
using System;
using Core.Services;

namespace Core.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManagementController : Controller
    {
        [Route("management")]
        public IActionResult Index() {
            return View();
        }

        [Route("management/financials")]
        public IActionResult Finance() {
            return View();
        }

        [Route("management/financials/station-wise/{month}/{year}")]
        public IActionResult FinanceStationWise(int month, int year, FinanceStationwiseViewModel model, CoreService service) {
            model.Date = new DateTime(year, month, 1);
            model.Income = service.GetManagementIncomePerStation(model.Date, model.Date.AddMonths(1).AddDays(-1));
            model.Costs = service.GetManagementCostsPerStation(model.Date, model.Date.AddMonths(1).AddDays(-1));

            return View(model);
        }
    }
}

