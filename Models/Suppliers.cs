using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class Suppliers {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public double Balance { get; set; }
        public bool Fuel { get; set; }
        public bool Lube { get; set; }
        public bool Gas { get; set; }
        public bool Soda { get; set; }

        public Suppliers() {
            Id = 0;
            Uuid = "";
            Name = "";
            Pin = "";
            Address = "";
            City = "";
            Telephone = "";
        }

        public Suppliers(long idnt) : this() {
            Id = idnt;
        }

        public Suppliers(long idnt, string name) : this() {
            Id = idnt;
            Name = name;
        }
    }

    public class SuppliersPayment {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Receipt { get; set; }
        public string Cheque { get; set; }
        public string Invoices { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Suppliers Supplier { get; set; }
        public Bank Bank { get; set; }
        public Users User { get; set; }

        public SuppliersPayment() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Receipt = "";
            Cheque = "";
            Invoices = "";
            Description = "";
            Amount = 0;
            Supplier = new Suppliers();
            User = new Users();
        }

        public SuppliersPayment(long idnt) : this() {
            Id = idnt;
        }

        public SuppliersPayment Save(HttpContext Context) {
            return new CoreService(Context).SaveSuppliersPayment(this);
        }

        public void Delete() {
            new CoreService().DeleteSuppliersPayment(this);
        }
    }
}
