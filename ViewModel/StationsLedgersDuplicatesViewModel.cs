using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class StationsLedgersDuplicatesViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Stations> Stations { get; set; }

        public StationsLedgersDuplicatesViewModel() {
            EndDate = DateTime.Now;
            StartDate = new DateTime(EndDate.Year, EndDate.Month, 1);
        }
    }
}
