using System;
namespace Core.Models
{
    public class CustomersPayments
    {
        public long Id { get; set; }
        public long Type { get; set; }
        public string Date { get; set; }
        public string Receipt { get; set; }
        public string Cheque { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; }
        public Stations Station { get; set; }
        public Customers Customer { get; set; }

        public CustomersPayments()
        {
            Id = 0;
            Type = 0;
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Receipt = "";
            Cheque = "";
            Notes = "";
            Amount = 0;

            Station = new Stations();  
            Customer = new Customers();
        }
    }
}
