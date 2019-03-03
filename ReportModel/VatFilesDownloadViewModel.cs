using System;
using System.Collections.Generic;
using Core.DataModel;

namespace Core.ReportModel
{
    public class VatFilesDownloadViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime Date { get; set; }

        public VatFilesDownloadViewModel() {
            Month = 0;
            Year = 0;
        }
    }
}
