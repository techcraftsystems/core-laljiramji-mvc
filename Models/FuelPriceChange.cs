using System;
using Core.Services;

namespace Core.Models
{
    public class FuelPriceChange
    {
        public long Id { get; set; }
        public Fuel Fuel { get; set; }
        public DateTime Date { get; set; }
        public double Taxx { get; set; }
        public double Zero { get; set; }
        public double Neww { get; set; }
        public double Trucks { get; set; }

        public FuelPriceChange() {
            Id = 0;
            Fuel = new Fuel();

            Date = DateTime.Now;
            Taxx = 0;
            Zero = 0;
            Neww = 0;
            Trucks = 0;
        }

        public FuelPriceChange Save()
        {
            return new CoreService().SavePriceChange(this);
        }
    }
}
