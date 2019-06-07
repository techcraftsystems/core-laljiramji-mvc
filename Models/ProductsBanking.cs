using System;
namespace Core.Models
{
    public class ProductsBanking
    {
        public Stations Station { get; set; }
        public double Banked { get; set; }
        public double Discount { get; set; }
        public double VAT { get; set; }
        public double Exempt { get; set; }

        public ProductsBanking() {
            Station = new Stations();
            Discount = 0;
            Banked = 0;
            VAT = 0;
            Exempt = 0;
        }
    }
}
