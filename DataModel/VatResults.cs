using System;
using Core.Models;

namespace Core.DataModel
{
    public class VatResults
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public Fuel Fuel { get; set; }
        public double Rate { get; set; }
        public double Zero { get; set; }
        public double Truck { get; set; }

        public VatResults() {
            Id = 0;
            Date = DateTime.Now;
            Fuel = new Fuel(1);
            Rate = 0;
            Zero = 0;
            Truck = 0;
        }
    }
}
