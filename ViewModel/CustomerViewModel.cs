using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel {
    public class CustomerViewModel {
        public string Date { get; set; }
        public string Note { get; set; }
        public Customers Customer { get; set; }
        public List<CustomersPayments> Payments { get; set; }

        public CustomerViewModel() {
            Date = DateTime.Now.ToString("d MMMM, yyyy");
            Note = "";
            Customer = new Customers();
            Payments = new List<CustomersPayments>();

            for (int ix = 0; ix < 10; ix++) {
                Payments.Add(new CustomersPayments());
            }
        }
    }
}
