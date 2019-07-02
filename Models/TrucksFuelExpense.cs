using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class TrucksFuelExpense
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Invoice { get; set; }
        public string Description { get; set; }

        public Trucks Truck { get; set; }
        public Suppliers Supplier { get; set; }

        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double VatAmount { get; set; }
        public double Zerorated { get; set; }
        public double Exclussive { get; set; }

        public TrucksFuelExpense() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("d MMMM, yyyy");
            Invoice = "";
            Description = "N/A";

            Truck = new Trucks();
            Supplier = new Suppliers(2);

            Quantity = 0;
            Price = 0;
            Amount = 0;
            VatAmount = 0;
            Zerorated = 0;
			Exclussive = 0;
        }

        public TrucksFuelExpense(long idnt) : this() {
            Id = idnt;
        }

        public TrucksFuelExpense Save(HttpContext context) {
            return new CoreService(context).SaveTrucksFuelExpense(this);
        }

        public void Delete() {
            new CoreService().DeleteTrucksFuelExpense(this);
        }

        public void UpdateLatestPurchasePrice() {
            Price = new CoreService().GetLatestPurchasePrice();
        }
    }
}
