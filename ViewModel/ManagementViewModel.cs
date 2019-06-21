using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class FinanceStationwiseViewModel
    {
        public List<Ledger> Income { get; set; }
        public List<Ledger> Costs { get; set; }
        public List<Ledger> Expense { get; set; }
        public DateTime Date { get; set; }

        public FinanceStationwiseViewModel() {
            Date = DateTime.Now;

            Income = new List<Ledger>();
            Costs = new List<Ledger>();
            Expense = new List<Ledger>();
        }
    }
}
