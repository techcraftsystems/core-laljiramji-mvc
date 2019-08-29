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
        public List<SuppliersCredits> Credits { get; set; }

        public IEnumerable<SelectListItem> BankAccounts { get; set; }
        public IEnumerable<SelectListItem> CreditsTypes { get; set; }
        public IEnumerable<SelectListItem> Stations { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; }

        public SuppliersViewModel() {
            Supplier = new Suppliers();
            Rcpt = "";
            Date = DateTime.Now.ToString("d MMMM, yyyy");
            Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            BankAccounts = new List<SelectListItem>();
            CreditsTypes = new List<SelectListItem>{
                new SelectListItem {Value = "0", Text = "Credit Note"},
                new SelectListItem {Value = "1", Text = "Debit Note"},
            };
            Stations = new List<SelectListItem>();
            Category = new List<SelectListItem>();

            Payments = new List<SuppliersPayment>();
            Invoices = new List<StationsExpenses>();
            Credits = new List<SuppliersCredits>();

            for (int i = 0; i < 10; i++) {
                Credits.Add(new SuppliersCredits());
                Payments.Add(new SuppliersPayment());
                Invoices.Add(new StationsExpenses());
            }
        }
    }

    public class SuppliersIndexViewModel {
        public Suppliers Supplier { get; set; }
        public List<Suppliers> Suppliers { get; set; }

        public SuppliersIndexViewModel() {
            Supplier = new Suppliers();
            Suppliers = new List<Suppliers>();
        }
    }

    public class SuppliersPaymentsViewModel {
        public DateTime Date { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<PaymentLedger> Ledger { get; set; }
        public List<PaymentSchedule> Schedule { get; set; }

        public SuppliersPaymentsViewModel() {
            Date = DateTime.Now;
            Months = new List<SelectListItem>();
            Ledger = new List<PaymentLedger>();
            Schedule = new List<PaymentSchedule>();

            var start = new DateTime(DateTime.Now.Year, 1, 1);
            for (int ix = 0; ix < 12; ix++) {
                Months.Add(new SelectListItem {
                    Value = ix.ToString(),
                    Text = start.AddMonths(ix).ToString("MMMM")
                });
            }
        }
    }
}
