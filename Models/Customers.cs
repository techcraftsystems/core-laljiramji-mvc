using System;
using Core.Services;

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
}
