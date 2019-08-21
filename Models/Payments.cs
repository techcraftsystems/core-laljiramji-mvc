using System;
namespace Core.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }

        public Payment() {
            Id = 0;
            Date = DateTime.Now;
        }
    }

    public class PaymentSchedule {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Invoice { get; set; }
        public Suppliers Supplier { get; set; }
        public double Amount { get; set; }
        public double ExVats { get; set; }
        public double PercSix { get; set; }
        public double PercTen { get; set; }
        public double Credits { get; set; }
        public double Cheques { get; set; }

        public PaymentSchedule() {
            Id = 0;
            Date = DateTime.Now;
            Invoice = "";
            Supplier = new Suppliers();
            Amount = 0;
            ExVats = 0;
            PercSix = 0;
            PercTen = 0;
            Credits = 0;
            Cheques = 0;
        }
    }

    public class PaymentLedger {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Invoice { get; set; }
        public Suppliers Supplier { get; set; }
        public double Amount { get; set; }
        public double Paid { get; set; }
        public double Balance { get; set; }

        public PaymentLedger() {
            Id = 0;
            Date = DateTime.Now;
            Invoice = "";
            Supplier = new Suppliers();
            Amount = 0;
            Paid = 0;
            Balance = 0;
        }
    }
}
