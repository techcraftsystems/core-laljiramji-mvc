using System;
namespace Core.Models
{
    public class Wetstock
    {
        public long Id { get; set; }
        public Tank Tank { get; set; }
        public DateTime Date { get; set; }
        public double Opening { get; set; }
        public double Deliveries { get; set; }
        public double Sales { get; set; }
        public double Tests { get; set; }
        public double Books { get; set; }
        public double Actuals { get; set; }
        public double Water { get; set; }
        public double Variance { get; set; }
        public double VariancePerc { get; set; }
        public double CummSales { get; set; }
        public double CummVariance { get; set; }
        public double CummVariancePerc { get; set; }

        public Wetstock() {
            Id = 0;
            Tank = new Tank();
            Date = DateTime.Now;
            Opening = 0;
            Deliveries = 0;
            Sales = 0;
            Tests = 0;
            Books = 0;
            Actuals = 0;
            Water = 0;
            Variance = 0;
            VariancePerc = 0;
        }
    }

    public class WetstockSummary
    {
        public long Id { get; set; }
        public Tank Tank { get; set; }
        public Fuel Fuel { get; set; }
        public double Opening { get; set; }
        public double Sale { get; set; }
        public double Returns { get; set; }
        public double Delivery { get; set; }
        public double Closing { get; set; }
        public double Dips { get; set; }

        public WetstockSummary() {
            Id = 0;
            Tank = new Tank();
            Fuel = new Fuel();
            Opening = 0;
            Sale = 0;
            Returns = 0;
            Delivery = 0;
            Closing = 0;
            Dips = 0;
        }
    }
}
