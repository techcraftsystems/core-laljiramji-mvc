using System;
namespace Core.ReportModel
{
    public class EtrSheet
    {
        public DateTime Date { get; set; }
        public double Cash { get; set; }
        public double CashVats { get; set; }
        public double CashZero { get; set; }

        public double Serv { get; set; }
        public double Soda { get; set; }
        public double Lube { get; set; }
        public double Rent { get; set; }
        public double Gas { get; set; }
        public double GasVat { get; set; }

        public double Credit { get; set; }
        public double CreditVats { get; set; }
        public double CreditZero { get; set; }
        public double CreditLube { get; set; }

        public double Total { get; set; }
        public double Check { get; set; }

        public double Vat16 { get; set; }
        public double Vat08 { get; set; }

        public double Total16 { get; set; }
        public double Total08 { get; set; }



        public EtrSheet() {
            Date = DateTime.Now;

            Cash = 0;
            CashVats = 0;
            CashZero = 0;

            Soda = 0;
            Lube = 0;
            Rent = 0;
            Gas = 0;
            GasVat = 0;

            Credit = 0;
            CreditVats = 0;
            CreditZero = 0;
            CreditLube = 0;

            Total = 0;
            Check = 0;

            Vat16 = 0;
            Vat08 = 0;

            Total16 = 0;
            Total08 = 0;
        }
    }
}
