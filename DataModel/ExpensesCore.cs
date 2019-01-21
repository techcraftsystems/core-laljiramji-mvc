using System;
namespace Core.DataModel
{
    public class ExpensesCore
    {
        public long Id { get; set; }
        public int Source { get; set; }

        public string Date { get; set; }
        public string Account { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }
        public string Delivery { get; set; }
        public string Invoice { get; set; }

        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }

        public ExpensesCore() {
            Id = 0;
            Source = 0;
            Date = DateTime.Now.ToString("dd/MM/yyyy");

            Account = "";
            Details = "";
            Description = "";
            Delivery = "";
            Invoice = "";

            Quantity = 0;
            Price = 0;
            Amount = 0;
        }
    }
}
