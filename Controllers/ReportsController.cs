using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.ViewModel;
using Core.Services;
using Core.DataModel;
using Core.Models;
using Core.ReportModel;

namespace Core.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: /<controller>/
        [Route("reports")]
        public IActionResult Index(ReportIndexViewModel model, StationsService service) {
            model.Stations = new List<Stations>(service.GetStationsNames());
            model.StationIdnts = service.GetStationIdntsIEnumerable();
            model.StationCodes = service.GetStationCodesIEnumerable();

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
        public IActionResult VATStationBreakdown(string code, int month, int year, VatStationBreakdownViewModel model, StationsService svc)
        {
            DateTime date = new DateTime(year, month, 1);

            model.station = svc.GetStation(code);
            model.year = year;
            model.month = month;
            model.report = svc.GetReportVatBreakdown(model.station, date, date.AddMonths(1).AddDays(-1));

            return View(model);
        }

        [Route("reports/vat/downloads/{month}/{year}")]
        public IActionResult VATFilesDownloads(int month, int year, VatFilesDownloadViewModel model, PurchasesService Service) {
            model.Month = month;
            model.Year = year;
            model.Date = new DateTime(year, month, 1);

            model.Purchases00Perc = Service.GetPurchases00PercEntries(month, year);
            model.Purchases08Perc = Service.GetPurchases08PercEntries(month, year);
            model.Purchases16Perc = Service.GetPurchases16PercEntries(month, year);

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

        [Route("reports/etr/{code}/{month}/{year}")]
        public IActionResult EtrSheet(string code, int month, int year, ReportsEtrSheetViewModel model, StationsService svc)
        {
            model.Date = new DateTime(year, month, 1);
            model.Station = svc.GetStation(code);
            model.Report = svc.GetEtrSheet(model.Station, month, year);

            return View(model);
        }

        [Route("reports/trucks/fuel/{year}")]
        public IActionResult TrucksFuelMonthly(int year) {
            List<TrucksMonthlySummary> model = new CoreService().GetTrucksMonthlySummary(year);
            return View(model);
        }

        [Route("reports/trucks/fuel-vat/{month}/{year}")]
        public IActionResult TrucksFuelVat(int month, int year) {
            List<TrucksFuelExpense> model = new List<TrucksFuelExpense>(new CoreService().GetTrucksFuelExpense(month, year));
            return View(model);
        }

        [Route("reports/stocks/quantity/{code}/{catg}/{month}/{year}")]
        public IActionResult StocksQuantity(string code, string catg, int month, int year, ReportProductSales model, StationsService service) {
            model.Date = new DateTime(year, month, 1);
            model.Station = service.GetStation(code);
            model.Banking = service.GetProductsBanking(model.Station, model.Date, model.Date.AddMonths(1).AddDays(-1), catg);
            model.Sales = service.GetProductsSales(model.Station, model.Date, model.Date.AddMonths(1).AddDays(-1), catg);
            model.StationCodes = service.GetStationCodesIEnumerable();

            return View(model);
        }

        [Route("reports/stocks/amount/{code}/{catg}/{month}/{year}")]
        public IActionResult StocksAmount(string code, string catg, int month, int year, ReportProductSales model, StationsService service) {
            model.Date = new DateTime(year, month, 1);
            model.Station = service.GetStation(code);
            model.Banking = service.GetProductsBanking(model.Station, model.Date, model.Date.AddMonths(1).AddDays(-1), catg);
            model.Sales = service.GetProductsSales(model.Station, model.Date, model.Date.AddMonths(1).AddDays(-1), catg);
            model.StationCodes = service.GetStationCodesIEnumerable();

            return View(model);
        }
    }
}
