using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class ExpensesLedgerViewModel
    {
        public List<SelectListItem> Stations { get; set; }
        public List<StationsExpenses> Expenses { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime StopsDate { get; set; }

        public ExpensesLedgerViewModel() {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            StopsDate = StartDate.AddMonths(1).AddDays(-1);

            Expenses = new List<StationsExpenses>();
        }
    }
}
