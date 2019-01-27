using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.ViewModel;
using Core.Services;
using Core.DataModel;
using Core.Models;

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

        [Route("reports/vat/{code}/{month}/{year}")]
        public IActionResult VATStationBreakdown(string code, int month, int year, ReportsVATStationBreakdownViewModel model, StationsService svc)
        {
            DateTime date = new DateTime(year, month, 1);

            model.station = svc.GetStation(code);
            model.year = year;
            model.month = month;
            model.report = svc.GetReportVatBreakdown(model.station, date, date.AddMonths(1).AddDays(-1));

            return View(model);
        }

        [Route("reports/stations/summary/{code}/{month}/{year}")]
        public IActionResult StationsSummary(string code, int month, int year, ReportsStationsSummaryViewModel model, StationsService svc)
        {
            model.Date = new DateTime(year, month, 1);
            model.Station = svc.GetStation(code);
            model.Report = svc.GetStationsMonthlySummary(model.Station, month, year);

            return View(model);
        }

        [Route("reports/trucks/fuel/{year}")]
        public IActionResult TrucksFuelMonthly(int year) {
            List<TrucksMonthlySummary> model = new CoreService().GetTrucksMonthlySummary(year);
            return View(model);
        }

        [Route("reports/trucks/fuel-vat/{month}/{year}")]
        public IActionResult TrucksFuelVat(int month, int year) {
            List<TrucksFuelExpense> model = new List<TrucksFuelExpense>(new CoreService().GetTrucksFuelExpense(month, year));//new CoreService().GetTrucksMonthlySummary(year);
            return View(model);
        }
    }
}
