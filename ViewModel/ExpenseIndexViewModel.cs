using System;
using System.Collections.Generic;
using Core.DataModel;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel
{
    public class ExpenseIndexViewModel {
        public StationsExpenses StationExpense { get; set; }

        public List<SelectListItem> Trucks { get; set; }
        public List<SelectListItem> Stations { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
        public List<SelectListItem> Categories { get; set; }

        public List<ExpensesCore> Expenses { get; set; }
        public List<TrucksFuelExpense> TrucksFuel { get; set; }

        public DateTime Date1x { get; set; }
        public DateTime Date2x { get; set; }
        public string DateX { get; set; }
        public string Notes { get; set; }

        public ExpenseIndexViewModel() {
            StationExpense = new StationsExpenses();
            Expenses = new List<ExpensesCore>();
            TrucksFuel = new List<TrucksFuelExpense>();

            Date1x = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Date2x = Date1x.AddMonths(1).AddDays(-1);
            DateX = DateTime.Now.ToString("d MMMM, yyyy");
            Notes = "N/A";

            double price = new CoreService().GetLatestPurchasePrice();
            for (int i = 0; i < 10; i++) {
                TrucksFuel.Add(new TrucksFuelExpense { Price = price });
            }
        }
    }
}
