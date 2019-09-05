using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModel {
    public class CustomerViewModel {
        public string Date { get; set; }
        public string Note { get; set; }
        public Customers Customer { get; set; }
        public List<CustomersPayments> Payments { get; set; }
        public List<CustomersWithholding> Withhold { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        public CustomerViewModel() {
            Date = DateTime.Now.ToString("d MMMM, yyyy");
            Note = "";
            Customer = new Customers();
            Payments = new List<CustomersPayments>();
            Withhold = new List<CustomersWithholding>();

            for (int ix = 0; ix < 10; ix++) {
                Payments.Add(new CustomersPayments());
                Withhold.Add(new CustomersWithholding());
            }

            Types = new List<SelectListItem> {
                new SelectListItem { Value = "1", Text = "WHT VAT"},
                new SelectListItem { Value = "2", Text = "WHT TAX"}
            };
        }
    }
}
