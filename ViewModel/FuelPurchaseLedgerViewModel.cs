using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class FuelPurchaseLedgerViewModel
    {
        public List<Stations> Stations { get; set; }
        public String Code { get; set; }

        public FuelPurchaseLedgerViewModel()
        {
            Stations = new List<Stations>();
            Code = "";
        }
    }
}
