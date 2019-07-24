using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel {
    public class SuppliersViewModel {
        public string Date { get; set; }
        public string Rcpt { get; set; }
        public Suppliers Supplier { get; set; }
        public DateTime Start { get; set; }
        public List<SuppliersPayment> Payments { get; set; }
        public List<StationsExpenses> Invoices { get; set; }

        public IEnumerable<SelectListItem> BankAccounts { get; set; }
        public IEnumerable<SelectListItem> Stations { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; }

        public SuppliersViewModel() {
            Supplier = new Suppliers();
            Rcpt = "";
            Date = DateTime.Now.ToString("d MMMM, yyyy");
            Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            BankAccounts = new List<SelectListItem>();
            Stations = new List<SelectListItem>();
            Category = new List<SelectListItem>();

            Payments = new List<SuppliersPayment>();
            Invoices = new List<StationsExpenses>();

            for (int i = 0; i < 10; i++) {
                Payments.Add(new SuppliersPayment());
                Invoices.Add(new StationsExpenses());
            }
        }
    }
}
