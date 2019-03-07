using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class PurchaseViewModel
    {
        public DateTime Date1x { get; set; }
        public DateTime Date2x { get; set; }

        public List<Purchases> Purchases { get; set; }

        public PurchaseViewModel() {
            Date1x = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Date2x = Date1x.AddMonths(1).AddDays(-1);

            Purchases = new List<Purchases>();
        }
    }
}
