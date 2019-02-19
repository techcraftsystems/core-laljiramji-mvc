using System;
namespace Core.Models
{
    public class FuelPurchasesLedger
    {
        public Stations Station { get; set; }
        public long Id { get; set; }
        public string Date { get; set; }
        public string Invoice { get; set; }
        public string Description { get; set; }

        public double Ltrs { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public double Excl { get; set; }
        public double Vats { get; set; }
        public double Zero { get; set; }

        public string PayDate { get; set; }
        public double PayCard { get; set; }
        public double PayAmts { get; set; }

        public FuelPurchasesLedger()
        {
            Station = new Stations();
            Id = 0;

            Date = DateTime.Now.ToString("dd-MMM");
            Invoice = "";
            Description = "";

            Ltrs = 0;
            Price = 0;
            Total = 0;

            Excl = 0;
            Vats = 0;
            Zero = 0;

            PayDate = "";
            PayCard = 0;
            PayAmts = 0;
        }
    }
}
