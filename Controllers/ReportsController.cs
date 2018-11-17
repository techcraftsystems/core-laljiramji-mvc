using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.ViewModel;
using Core.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: /<controller>/
        [Route("reports")]
        public IActionResult Index(ReportsIndexViewModel model, StationsService svc)
        {
            model.Stations = new List<Models.Stations>(svc.GetStationsByNames());
            return View(model);
        }

        [Route("reports/customers/summary/{code}/{year}/{type}")]
        public IActionResult CustomerYearly(String code, Int64 year, String type, ReportsCustomerYearlyViewModel model, StationsService svc)
        {
            model.station = svc.GetStation(code);
            model.year = year;
            model.type = type;
            model.report = svc.GetCustomerYearlyReport(year, model.station.Id, type);

            return View(model);
        }

        [Route("reports/customers/balances/{code}/{year}")]
        public IActionResult CustomerBalances(String code, Int64 year, String type, ReportsCustomerYearlyViewModel model, StationsService svc)
        {
            model.station = svc.GetStation(code);
            model.year = year;
            model.type = type;
            model.report = svc.GetCustomerYearlyReport(year, model.station.Id, "all");

            return View(model);
        }
    }
}
