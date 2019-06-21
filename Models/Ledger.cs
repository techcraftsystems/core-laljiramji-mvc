using System;
namespace Core.Models
{
    public class Ledger {
        public LedgerType Type { get; set; }
        public LedgerItem Item { get; set; }
        public double Bypass { get; set; }
        public double Gitimbine { get; set; }
        public double Kaaga { get; set; }
        public double Kenol { get; set; }
        public double Kinoru { get; set; }
        public double Kirunga { get; set; }
        public double Kobil { get; set; }
        public double Maua { get; set; }
        public double Nkubu { get; set; }
        public double Ojijo { get; set; }
        public double Oryx { get; set; }
        public double Uhuru { get; set; }
        public double Viewpt { get; set; }
        public double Total { get; set; }

        public Ledger(){
            Type = new LedgerType();
            Item = new LedgerItem();

            Bypass = 0;
            Gitimbine = 0;
            Kaaga = 0;
            Kenol = 0;
            Kinoru = 0;
            Kirunga = 0;
            Kobil = 0;
            Maua = 0;
            Nkubu = 0;
            Ojijo = 0;
            Oryx = 0;
            Uhuru = 0;
            Viewpt = 0;
            Total = 0;
        }
    }

    public class LedgerType {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }

        public LedgerType() {
            Id = 0;
            Name = "";
            Prefix = "";
        }
    }

    public class LedgerItem {
        public long Id { get; set; }
        public string Name { get; set; }

        public LedgerItem() {
            Id = 0;
            Name = "";
        }
    }
}
