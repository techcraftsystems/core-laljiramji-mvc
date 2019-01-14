using System;
namespace Core.Models
{
    public class Bank
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int StationCount { get; set; }
        public DateTime LastTransaction { get; set; }

        public Bank() {
            Id = 0;
            Code = "";
            Name = "";
            Branch = "";
            AccountName = "";
            AccountNumber = "";
            Description = "";
            Amount = 0;

            StationCount = 0;
            LastTransaction = DateTime.Now;
        }
    }
}
