using System;
using System.Collections.Generic;
using Core.Models;
using Core.ReportModel;

namespace Core.ViewModel
{
    public class ReportsStationsSummaryViewModel
    {
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public List<StationsMonthlySummary> Report { get; set; }

        public ReportsStationsSummaryViewModel() {
            Date = DateTime.Now;
            Station = new Stations();
            Report = new List<StationsMonthlySummary>();
        }
    }
}
