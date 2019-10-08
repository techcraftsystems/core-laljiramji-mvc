using System;
using System.Collections.Generic;
using Core.Models;
using Core.ReportModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class ReportIndexViewModel
    {
        public List<Stations> Stations { get; set; }
        public IEnumerable<SelectListItem> StationCodes { get; set; }
        public IEnumerable<SelectListItem> StationIdnts { get; set; }
        public DateTime StartDate { get; set; }
        public Stations Station { get; set; }

        public ReportIndexViewModel() {
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);

            Station = new Stations();
            Stations = new List<Stations>();
            StationCodes = new List<SelectListItem>();
            StationIdnts = new List<SelectListItem>();
        }
    }

    public class ReportsStationsSummaryViewModel {
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public List<StationsMonthlySummary> Report { get; set; }

        public ReportsStationsSummaryViewModel() {
            Date = DateTime.Now;
            Station = new Stations();
            Report = new List<StationsMonthlySummary>();
        }
    }

    public class ReportsEtrSheetViewModel {
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public List<EtrSheet> Report { get; set; }

        public ReportsEtrSheetViewModel() {
            Date = DateTime.Now;
            Station = new Stations();
            Report = new List<EtrSheet>();
        }
    }

    public class ReportsCustomerYearlyViewModel {
        public Stations station { get; set; }
        public long year { get; set; }
        public string type { get; set; }
        public List<ReportCustomerYearly> report { get; set; }
        public IEnumerable<SelectListItem> Codes { get; set; }

        public ReportsCustomerYearlyViewModel()
        {
            station = new Stations();
            year = DateTime.Now.Year;
            Codes = new List<SelectListItem>();
            report = new List<ReportCustomerYearly>();
        }
    }

    public class ReportsCustomerPeriodicViewModel
    {
        public Stations Station { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<ReportCustomerPeriodic> Report { get; set; }
        public IEnumerable<SelectListItem> Codes { get; set; }

        public ReportsCustomerPeriodicViewModel() {
            Station = new Stations();
            Start = DateTime.Now;
            Stop = DateTime.Now;
            Codes = new List<SelectListItem>();
            Report = new List<ReportCustomerPeriodic>();
        }
    }

    public class ReportProductsViewModel {
        public IEnumerable<SelectListItem> Stations { get; set; }
        public List<Products> Products { get; set; }
        public string Station { get; set; }

        public ReportProductsViewModel() {
            Station = "";
            Stations = new List<SelectListItem>();
            Products = new List<Products>();
        }
    }

    public class ReportProductsLinkedViewModel {
        public List<ProductsLinked> Products { get; set; }

        public ReportProductsLinkedViewModel() {
            Products = new List<ProductsLinked>();
        }
    }


    public class ReportProductSales {
        public List<ProductsSales> Sales { get; set; }
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public ProductsBanking Banking { get; set; }
        public IEnumerable<SelectListItem> StationCodes { get; set; }

        public ReportProductSales() {
            Date = DateTime.Now;
            Sales = new List<ProductsSales>();
            Station = new Stations();
            Banking = new ProductsBanking();
            StationCodes = new List<SelectListItem>();
        }
    }

    public class ReportPurchaseVariance {
        public List<ProductsLedger> Diesel { get; set; }
        public List<ProductsLedger> Super { get; set; }
        public List<ProductsLedger> Vpower { get; set; }
        public List<ProductsLedger> Kerosene { get; set; }

        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public IEnumerable<SelectListItem> Stations { get; set; }

        public ReportPurchaseVariance() {
            Date = DateTime.Now;
            Station = new Stations();
            Stations = new List<SelectListItem>();
        }
    }

    public class ReportWetstock {
        public List<WetstockSummary> Summary { get; set; }
        public List<Wetstock> Wetstock { get; set; }
        public Stations Station { get; set; }
        public DateTime Date { get; set; }
        public DateTime Stop { get; set; }
        public IEnumerable<SelectListItem> Stations { get; set; }

        public ReportWetstock() {
            Summary = new List<WetstockSummary>();
            Wetstock = new List<Wetstock>();
            Station = new Stations();
            Date = DateTime.Now;
            Stop = DateTime.Now;
            Stations = new List<SelectListItem>();
        }
    }
}
