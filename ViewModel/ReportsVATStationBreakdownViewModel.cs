using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class ReportsVATStationBreakdownViewModel
    {
        public Stations station { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public List<ReportVatBreakdown> report { get; set; }

        public ReportsVATStationBreakdownViewModel() {
            station = new Stations();
            month = 0;
            year = 0;
            report = new List<ReportVatBreakdown>();
        }
    }
}
