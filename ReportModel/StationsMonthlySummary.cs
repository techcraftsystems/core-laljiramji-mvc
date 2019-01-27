using System;
namespace Core.ReportModel
{
    public class StationsMonthlySummary
    {
        public DateTime Date { get; set; }
        public double DxSale { get; set; }
        public double UxSale { get; set; }
        public double VpSale { get; set; }
        public double IkSale { get; set; }
        public double Sale { get; set; }

        public double DxCredit { get; set; }
        public double UxCredit { get; set; }
        public double VpCredit { get; set; }
        public double IkCredit { get; set; }
        public double Credit { get; set; }
        public double OxCredit { get; set; }

        public double Cash { get; set; }
        public double Discount { get; set; }
        public double Transport { get; set; }
        public double Lesses { get; set; }

        public double Lube { get; set; }
        public double LubeCredit { get; set; }
        public double Gas { get; set; }
        public double GasVat { get; set; }
        public double Soda { get; set; }

        public double Rent { get; set; }

        public double Carwash { get; set; }
        public double CarwashCredit { get; set; }
        public double Service { get; set; }
        public double ServiceCredit { get; set; }
        public double Tyre { get; set; }
        public double TyreCredit { get; set; }

        public StationsMonthlySummary() {
            Date = DateTime.Now;

            DxSale = 0;
            UxSale = 0;
            VpSale = 0;
            IkSale = 0;
            Sale = 0;

            DxCredit = 0;
            UxCredit = 0;
            VpCredit = 0;
            IkCredit = 0;
            Credit = 0;
            OxCredit = 0;

            Cash = 0;
            Discount = 0;
            Transport = 0;
            Lesses = 0;

            Lube = 0;
            LubeCredit = 0;
            Gas = 0;
            GasVat = 0;
            Soda = 0;

            Rent = 0;

            Carwash = 0;
            CarwashCredit = 0;
            Service = 0;
            ServiceCredit = 0;
            Tyre = 0;
            TyreCredit = 0;
        }
    }
}
