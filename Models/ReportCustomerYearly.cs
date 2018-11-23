using System;
namespace Core.Models
{
    public class ReportCustomerYearly
    {
        public Customers Customer { get; set; }
        public Double Opening { get; set; }
        public Double Jan { get; set; }
        public Double Feb { get; set; }
        public Double Mar { get; set; }
        public Double Apr { get; set; }
        public Double May { get; set; }
        public Double Jun { get; set; }
        public Double Jul { get; set; }
        public Double Aug { get; set; }
        public Double Sep { get; set; }
        public Double Oct { get; set; }
        public Double Nov { get; set; }
        public Double Dec { get; set; }
        public Double Total { get; set; }
        public Double Closing { get; set; }

        public ReportCustomerYearly()
        {
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
}
