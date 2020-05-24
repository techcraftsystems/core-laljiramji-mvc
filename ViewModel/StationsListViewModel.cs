using System;
using System.Collections.Generic;
using Core.Models;
using Core.Services;

namespace Core.ViewModel
{
    public class StationsListViewModel
    {
        public DateTime Date { get; set; }
        public Stations Selected { get; set; }
        public LedgerTotals Totals { get; set; }
        public List<Stations> Stations { get; set; }
        public List<PumpReadings> Readings { get; set; }
        public List<TankSummary> Summaries { get; set; }
        public List<LegderSummary> Ledgers { get; set; }
        public List<Customers> Customers { get; set; }
        public List<MonthsModel> Months { get; set; }
        public Int64 Month { get; set; }

        public StationsListViewModel(){
            Date = DateTime.Now;
            Selected = new Stations();
            Totals = new LedgerTotals();
            Stations = new List<Stations>();
            Readings = new List<PumpReadings>();
            Summaries = new List<TankSummary>();
            Ledgers = new List<LegderSummary>();
            Customers = new List<Customers>();

            Month = DateTime.Now.Month;
            Months = new StationsService().InitializeMonthsModel();
        }
    }
}
