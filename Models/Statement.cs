using System;
namespace Core.Models
{
    public class Statement
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Dates { get; set; }
        public string Transaction { get; set; }
        public string Receipt { get; set; }
        public string Details { get; set; }
        public double Amount { get; set; }

        public Statement() {
            Id = 0;
            Date = DateTime.Now;
            Dates = Date.ToString("dd/MM/yyyy");
            Transaction = "";
            Receipt = "";
            Details = "";
            Amount = 0;
        }
    }
}
