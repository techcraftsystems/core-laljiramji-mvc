using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class ReportsCustomerYearlyViewModel
    {
        public Stations station { get; set; }
        public Int64 year { get; set; }
        public String type { get; set; }
        public List<CustomerYearly> report { get; set; }

        public ReportsCustomerYearlyViewModel()
        {
            station = new Stations();
            year = DateTime.Now.Year;
            report = new List<CustomerYearly>();
        }
    }
}
