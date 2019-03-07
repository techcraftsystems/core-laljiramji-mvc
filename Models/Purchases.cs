using System;
namespace Core.Models
{
    public class Purchases
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Lpo { get; set; }
        public string Invoice { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public double VatsAmount { get; set; }
        public double ZeroAmount { get; set; }

        public Stations Station { get; set; }
        public Suppliers Supplier { get; set; }

        public Purchases() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Lpo = "";
            Invoice = "";
            Category = "";
            Description = "";
            Amount = 0;
            Station = new Stations();
            Supplier = new Suppliers();
        }
    }
}
