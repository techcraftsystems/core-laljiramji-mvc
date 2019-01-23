using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class StationsExpenses
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Invoice { get; set; }
        public string Description { get; set; }

        public Suppliers Supplier { get; set; }
        public Stations Station { get; set; }
        public ExpensesCategory Category { get; set; }
        public Users User { get; set; }

        public double Amount { get; set; }
        public double VatAmount { get; set; }
        public double Zerorated { get; set; }

        public StationsExpenses() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("d MMMM, yyyy");
            Invoice = "";
            Description = "";

            Supplier = new Suppliers();
            Station = new Stations();
            Category = new ExpensesCategory();

            Amount = 0;
            VatAmount = 0;
            Zerorated = 0;
        }

        public StationsExpenses Save(HttpContext context){
            return new CoreService(context).SaveStationsExpenses(this);
        }
    }
}
