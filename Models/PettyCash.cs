using System;
using Core.Services;

namespace Core.Models
{
    public class PettyCash {
        public long Id { get; set; }
        public Delivery Delivery { get; set; }
        public string Receipt { get; set; }
        public string Voucher { get; set; }
        public string Account { get; set; }
        public string Supplier { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Users AddedBy { get; set; }
        public DateTime AddedOn { get; set; }

        public PettyCash() {
            Id = 0;
            Delivery = new Delivery();
            Account = "";
            Receipt = "";
            Voucher = "";
            Supplier = "";
            Description = "";
            Amount = 0;
        }

        public PettyCash Save() {
            return new CoreService().SavePettyCash(this);
        }
    }
}
