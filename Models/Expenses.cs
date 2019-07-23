using System;
namespace Core.Models
{
    public class Expenses
    {
        public long Id { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public ChartsOfAccount Account { get; set; }
        public Stations Station { get; set; }

        public Expenses() {
            Id = 0;
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Location = "";
            Description = "";
            Amount = 0;

            Account = new ChartsOfAccount();
            Station = new Stations();
        }
    }

    public class ExpensesLedger {
        public long Id { get; set; }
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Invoice { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public double Vat { get; set; }
        public double Zero { get; set; }
        public Suppliers Supplier { get; set; }
        public Stations Station { get; set; }

        public ExpensesLedger() {
            Id = 0;
            Type = 0;

            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");

            Invoice = "";
            Category = "";
            Description = "";

            Amount = 0;
            Vat = 0;
            Zero = 0;

            Supplier = new Suppliers();
            Station = new Stations();
        }

    }
}
