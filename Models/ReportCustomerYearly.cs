using System;
namespace Core.Models
{
    public class ReportCustomerYearly
    {
        public Customers Customer { get; set; }
        public double Opening { get; set; }
        public double Jan { get; set; }
        public double Feb { get; set; }
        public double Mar { get; set; }
        public double Apr { get; set; }
        public double May { get; set; }
        public double Jun { get; set; }
        public double Jul { get; set; }
        public double Aug { get; set; }
        public double Sep { get; set; }
        public double Oct { get; set; }
        public double Nov { get; set; }
        public double Dec { get; set; }
        public double Total { get; set; }
        public double Closing { get; set; }

        public ReportCustomerYearly() {
            Customer = new Customers();
            Opening = 0;
            Jan = 0;
            Feb = 0;
            Mar = 0;
            Apr = 0;
            May = 0;
            Jun = 0;
            Jul = 0;
            Aug = 0;
            Sep = 0;
            Oct = 0;
            Nov = 0;
            Dec = 0;
            Total = 0;
            Closing = 0;
        }
    }

    public class ReportCustomerPeriodic {
        public Customers Customer { get; set; }
        public double Opening { get; set; }
        public double Invoice { get; set; }
        public double Credits { get; set; }
        public double Payment { get; set; }
        public double Closing { get; set; }

        public ReportCustomerPeriodic() {
            Customer = new Customers();
            Opening = 0;
            Invoice = 0;
            Credits = 0;
            Payment = 0;
            Closing = 0;
        }
    }
}
