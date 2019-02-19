using System;
namespace Core.DataModel
{
    public class FuelLedgerSummary
    {
        public string Name { get; set; }
        public double Dx { get; set; }
        public double Ux { get; set; }
        public double Vp { get; set; }
        public double Ik { get; set; }
        public double Total { get; set; }
        public double Vats { get; set; }
        public double Excl { get; set; }
        public double Zero { get; set; }

        public FuelLedgerSummary() {
            Name = "";
            Dx = 0;
            Ux = 0;
            Vp = 0;
            Ik = 0;
            Total = 0;
            Vats = 0;
            Excl = 0;
            Zero = 0;
        }
    }
}
