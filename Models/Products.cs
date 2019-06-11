using System;
namespace Core.Models
{
    public class Products
    {
        public long Id { get; set; }
        public long Quantity { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Bp { get; set; }
        public double Sp { get; set; }

        public Products() {
            Id = 0;
            Name = "";
            Category = "";

            Quantity = 0;
            Bp = 0;
            Sp = 0;
        }
    }

    public class ProductsSales {
        public Products Product { get; set; }
        public double Opening { get; set; }
        public double Inns { get; set; }
        public double Sales { get; set; }
        public double Transfer { get; set; }
        public double Closing { get; set; }
        public double Amounts { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public ProductsSales() {
            Opening = 0;
            Inns = 0;
            Sales = 0;
            Transfer = 0;
            Closing = 0;
            Amounts = 0;
        }
    }

    public class ProductsLedger {
        public DateTime Date { get; set; }
        public double Purchase { get; set; }
        public double Delivery { get; set; }
        public double Overpump { get; set; }
        public Products Product { get; set; }

        public ProductsLedger() {
            Date = DateTime.Now;
            Purchase = 0;
            Delivery = 0;
            Overpump = 0;
            Product = new Products();
        }
    }
}
