using System;
using System.Collections.Generic;
using Core.DataModel;
using Core.Models;

namespace Core.ReportModel
{
    public class VatFilesDownloadViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime Date { get; set; }

        public List<VatDownloadEntries> Purchases00Perc { get; set; }
        public List<VatDownloadEntries> Purchases08Perc { get; set; }
        public List<VatDownloadEntries> Purchases16Perc { get; set; }

        public VatFilesDownloadViewModel() {
            Month = 0;
            Year = 0;

            Purchases00Perc = new List<VatDownloadEntries>();
            Purchases08Perc = new List<VatDownloadEntries>();
            Purchases16Perc = new List<VatDownloadEntries>();
        }
    }

    public class VATMissingBreakdownsViewModel
    {
        public List<LedgerEntries> Ledger { get; set; }

        public VATMissingBreakdownsViewModel() {
            Ledger = new List<LedgerEntries>();
        }
    }
}
