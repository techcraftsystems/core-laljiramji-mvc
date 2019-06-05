using System;
using Core.Models;

namespace Core.Models
{
    public class PurchasesLedger
    {
        public long Id { get; set; }
        public String Type { get; set; }
        public String Date { get; set; }
        public String Invoice { get; set; }
        public Double Quantity { get; set; }
        public Double Purchase { get; set; }
        public Double Delivery { get; set; }
        public Double Price { get; set; }
        public Double Total { get; set; }

        public Stations Station { get; set; }
        public Fuel Fuel { get; set; }
        public Suppliers Supplier { get; set; }


        public PurchasesLedger()
        {
            Id = 0;
            Type = "";
            Date = DateTime.Now.ToString("dd.MM.yyyy");
            Invoice = "";
            Quantity = 0;
            Purchase = 0;
            Delivery = 0;
            Price = 0;
            Total = 0;

            Station = new Stations();
            Fuel = new Fuel();
            Supplier = new Suppliers();
        }
    }

    public class PurchasesOthers {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Lpo { get; set; }
        public string Invoice { get; set; }
        public double Rate { get; set; }
        public double Total { get; set; }
        public double Taxable { get; set; }
        public Stations Station { get; set; }
        public Suppliers Supplier { get; set; }

        public PurchasesOthers() {
            Id = 0;
            Type = "";
            Date = "";
            Lpo = "";
            Invoice = "";
            Rate = 0;
            Total = 0;
            Taxable = 0;
            Station = new Stations();
            Supplier = new Suppliers();
        }
    }
}
