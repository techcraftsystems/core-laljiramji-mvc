using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class PurchaseLedgerViewModel
    {
        public IEnumerable<SelectListItem> Stations { get; set; }
        public Stations Station { get; set; }
        public string Code { get; set; }
        public List<FuelPurchasesLedger> Ledger { get; set; }

        public PurchaseLedgerViewModel() {
            Stations = new List<SelectListItem>();
            Code = "";
            Station = new Stations();
            Ledger = new List<FuelPurchasesLedger>();
        }
    }
}
