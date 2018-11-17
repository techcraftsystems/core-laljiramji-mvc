using System;
namespace Core.Models
{
    public class FuelPurchasesLedger
    {
        public Stations Station { get; set; }
        public Int64 Id { get; set; }
        public String Date { get; set; }
        public String Invoice { get; set; }
        public String Description { get; set; }

        public Double Ltrs { get; set; }
        public Double Price { get; set; }
        public Double Total { get; set; }

        public String PayDate { get; set; }
        public Double PayCard { get; set; }
        public Double PayAmts { get; set; }

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

            PayDate = "";
            PayCard = 0;
            PayAmts = 0;
        }
    }
}
