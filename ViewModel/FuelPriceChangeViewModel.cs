using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class FuelPriceChangeViewModel
    {
        public List<FuelPriceChange> Previous { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopsDate { get; set; }

        public FuelPriceChangeViewModel() {
            Previous = new List<FuelPriceChange>();
            StartDate = DateTime.Now;
            StopsDate = DateTime.Now;
        }
    }
}
