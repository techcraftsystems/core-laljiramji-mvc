using System;
using Core.Models;

namespace Core.ViewModel {
    public class SuppliersViewModel {
        public Suppliers Supplier { get; set; }
        public DateTime Start { get; set; }

        public SuppliersViewModel() {
            Supplier = new Suppliers();
            Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
    }
}
