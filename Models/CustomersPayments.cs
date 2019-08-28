using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class CustomersPayments {
        public long Id { get; set; }
        public long Type { get; set; }
        public DateTime PostDate { get; set; }
        public string Date { get; set; }
        public int Receipt { get; set; }
        public int Cheque { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; }
        public Stations Station { get; set; }
        public Customers Customer { get; set; }

        public CustomersPayments() {
            Id = 0;
            Type = 0;
            PostDate = DateTime.Now;
            Date = PostDate.ToString("dd/MM/yyyy");
            Receipt = 0;
            Cheque = 0;
            Notes = "";
            Amount = 0;

            Station = new Stations();  
            Customer = new Customers();
        }

        public CustomersPayments Save(HttpContext Context) {
            return new CoreService(Context).SaveCustomersPayments(this);
        }

        public void Delete() {
            new CoreService().DeleteCustomersPayment(this);
        }
    }
}
