using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ReportModel
{
    public class DeliveryVarianceViewModel
    {
        public DateTime Date { get; set; }
        public Stations Station { get; set; }
        public List<DeliveryVariance> Ledger { get; set; }
        public IEnumerable<SelectListItem> Codes { get; set; }

        public DeliveryVarianceViewModel() {
            Date = DateTime.Now;
            Station = new Stations();
            Codes = new List<SelectListItem>();
            Ledger = new List<DeliveryVariance>();
        }
    }
}
