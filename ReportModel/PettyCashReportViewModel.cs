using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ReportModel
{
    public class PettyCashReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Selected { get; set; }
        public List<PettyCash> PettyCash { get; set; }
        public IEnumerable<SelectListItem> Codes { get; set; }

        public PettyCashReportViewModel() {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            Selected = "";
            PettyCash = new List<PettyCash>();
            Codes = new List<SelectListItem>();
        }
    }
}
