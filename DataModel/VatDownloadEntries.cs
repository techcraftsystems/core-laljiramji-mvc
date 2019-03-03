using System;
using Core.Models;

namespace Core.DataModel
{
    public class VatDownloadEntries
    {
        public long Id { get; set; }
        public string Date { get; set; }
        public string Invoice { get; set; }
        public string Type { get; set; }
        public string Blank { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Suppliers Supplier { get; set; }

        public VatDownloadEntries() {
            Id = 0;
            Date = "";
            Invoice = "";
            Type = "";
            Blank = "";
            Description = "";
            Amount = 0;
            Supplier = new Suppliers();
        }
    }
}
