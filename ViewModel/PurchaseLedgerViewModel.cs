using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class PurchaseLedgerViewModel
    {
        public IEnumerable<SelectListItem> Stations { get; set; }
        public string Code { get; set; }

        public PurchaseLedgerViewModel() {
            Stations = new List<SelectListItem>();
            Code = "";
        }
    }
}
