using System;
namespace Core.ViewModel
{
    public class BankingReconcileModel
    {
        public int Station { get; set; }
        public int Source { get; set; }
        public int Action { get; set; }

        public string Date { get; set; }
        public string Description { get; set; }
        public string Customer { get; set; }
        public string Cheque { get; set; }
        public string Invoice { get; set; }

        public double Quantity { get; set; }
        public double Price { get; set; }

        public double Revenue { get; set; }
        public double Expense { get; set; }
        public double Cummulative { get; set; }

        public BankingReconcileModel()
        {
            Station = 0;
            Source = 0;
            Action = 0;

            Date = DateTime.Now.ToString("dd-MMM");
            Description = "X";
            Customer = "X";
            Cheque = "X";
            Invoice = "X";

            Quantity = 0;
            Price = 0;

            Revenue = 0;
            Expense = 0;
            Cummulative = 0;
        }
    }
}
