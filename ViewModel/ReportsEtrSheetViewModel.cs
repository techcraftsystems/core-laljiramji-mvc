using System;
using System.Collections.Generic;
using Core.Models;
using Core.ReportModel;

namespace Core.ViewModel
{
    public class ReportsEtrSheetViewModel
    {
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public List<EtrSheet> Report { get; set; }

        public ReportsEtrSheetViewModel() {
            Date = DateTime.Now;
            Station = new Stations();
            Report = new List<EtrSheet>();
        }
    }
}
