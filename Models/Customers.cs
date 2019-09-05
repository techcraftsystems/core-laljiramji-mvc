using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class Customers {
        public long Id { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime LastInvoice { get; set; }
        public string Name { get; set; }
        public string Contacts { get; set; }
        public string Telephone { get; set; }
        public string KraPin { get; set; }
        public double Balance { get; set; }
        public double CreditLimit { get; set; }
        public Stations Station { get; set; }

        public Customers() {
            Id = 0;
            DateJoined = DateTime.Now;
            LastInvoice = DateTime.Now;
            Name = "";
            Contacts = "";
            Telephone = "";
            KraPin = "";
            Balance = 0;
            CreditLimit = 0;
            Station = new Stations();
        }

        public Customers(long idnt) : this(){
            Id = idnt;
        }

        public void UpdateBalance() {
            var Service = new CoreService();

            Service.UpdateCustomerBalance(this);
            Balance = Service.GetCustomerBalance(this);
        }
    }

    public class CustomersPayments {
        public long Id { get; set; }
        public Types Type { get; set; }
        public DateTime PostDate { get; set; }
        public string Date { get; set; }
        public string Receipt { get; set; }
        public string Cheque { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; }
        public Stations Station { get; set; }
        public Customers Customer { get; set; }

        public CustomersPayments() {
            Id = 0;
            Type = new Types();
            PostDate = DateTime.Now;
            Date = PostDate.ToString("dd/MM/yyyy");
            Receipt = "";
            Cheque = "";
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

    public class CustomersWithholding {
        public long Id { get; set; }
        public Customers Customer { get; set; }
        public Types Type { get; set; }
        public Users User { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Receipt { get; set; }
        public string Invoice { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }

        public CustomersWithholding() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Receipt = "";
            Invoice = "";
            Description = "";
            Amount = 0;
            Customer = new Customers();
            User = new Users();
            Type = new Types();
        }

        public CustomersWithholding(long idnt) : this() {
            Id = idnt;
        }

        public CustomersWithholding Save(HttpContext Context) {
            return new CoreService(Context).SaveCustomersWithholding(this);
        }

        public void Delete() {
            new CoreService().DeleteCustomersWithholding(this);
        }
    }
}
