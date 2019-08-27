using System;
namespace Core.Models
{
    public class Wetstock
    {
        public Wetstock()
        {
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
