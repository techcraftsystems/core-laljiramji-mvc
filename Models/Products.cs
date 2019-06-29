using System;
namespace Core.Models
{
    public class Products
    {
        public long Id { get; set; }
        public long Quantity { get; set; }
        public string Name { get; set; }
        public string Measure { get; set; }
        public string Category { get; set; }
        public double Bp { get; set; }
        public double Sp { get; set; }
        public double Tax { get; set; }
        public double Ltrs { get; set; }
        public Stations Station { get; set; }

        public Products() {
            Id = 0;
            Name = "";
            Measure = "";
            Category = "";

            Quantity = 0;
            Bp = 0;
            Sp = 0;
            Tax = 0;
            Ltrs = 0;
            Station = new Stations();
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

    public class ProductsLinked {
        public long Id { get; set; }
        public Products Product { get; set; }
        public Products Gitimbine { get; set; }
        public Products Kaaga { get; set; }
        public Products Nkubu { get; set; }
        public Products Kirunga { get; set; }

        public ProductsLinked() {
            Id = 0;
            Product = new Products();
            Gitimbine = new Products();
            Kaaga = new Products();
            Nkubu = new Products();
            Kirunga = new Products();
        }
    }

    public class ProductsTransfer {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public Products Product { get; set; }
        public double Gitimbine { get; set; }
        public double Kaaga { get; set; }
        public double Nkubu { get; set; }
        public double Kirunga { get; set; }
        public double Total { get; set; }

        public ProductsTransfer() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Product = new Products();
            Gitimbine = 0;
            Kaaga = 0;
            Nkubu = 0;
            Kirunga = 0;
            Total = 0;
        }
    }
}
