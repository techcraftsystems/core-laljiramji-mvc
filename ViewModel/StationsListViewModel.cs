using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class StationsListViewModel
    {
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
            Selected = new Stations();
            Totals = new LedgerTotals();
            Stations = new List<Stations>();
            Readings = new List<PumpReadings>();
            Summaries = new List<TankSummary>();
            Ledgers = new List<LegderSummary>();
            Customers = new List<Customers>();

            Month = DateTime.Now.Month;

            Initialize();
        }

        public void Initialize()
        {
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            Months = new List<MonthsModel>();

            for (int i = 1; i < 13; i++)
            {
                MonthsModel month = new MonthsModel();
                month.Value = date.Month;
                month.Name = date.ToString("MMMM");

                month.Select |= DateTime.Now.Month == date.Month;

                Months.Add(month);

                date = date.AddMonths(1);
            }
        }
    }
}
