using System;
using System.Collections.Generic;
using Core.DataModel;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class ExpenseIndexViewModel
    {
        public TrucksFuelExpense TrucksExpenses { get; set; }
        public List<SelectListItem> Trucks { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
        public List<ExpensesCore> Expenses { get; set; }

        public DateTime Date1x { get; set; }
        public DateTime Date2x { get; set; }

        public ExpenseIndexViewModel() {
            TrucksExpenses = new TrucksFuelExpense();
            TrucksExpenses.UpdateLatestPurchasePrice();

            Date1x = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Date2x = Date1x.AddMonths(1).AddDays(-1);

            Expenses = new List<ExpensesCore>();
        }
    }
}
