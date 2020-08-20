using System;
using System.Collections.Generic;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models {
    public class Delivery {
        public long Id { get; set; }
        public DeliveryType Type { get; set; }
        public Stations Station { get; set; }
        public Bank Bank { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Receipt { get; set; }
        public double Amount { get; set; }
        public double Expense { get; set; }
        public double Banking { get; set; }
        public double Discount { get; set; }
        public Users AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string JSonExpense { get; set; }
        public string Description { get; set; }

        public Delivery() {
            Id = 0;

            Bank = new Bank();
            Type = new DeliveryType();
            Station = new Stations();

            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");

            Receipt = "";
            Amount = 0;
            Expense = 0;
            Banking = 0;
            Discount = 0;

            AddedBy = new Users();
            AddedOn = DateTime.Now;

            JSonExpense = "{}";
            Description = "N/A";
        }

        public Delivery (long idnt) : this() {
            Id = idnt;
        }

        public List<PettyCash> GetPettyCash() {
            return new List<PettyCash>();
        }

        public Delivery Save(HttpContext Context) {
            return new CoreService(Context).SaveDelivery(this);
        }

        public void Delete() {
            new CoreService().DeleteDelivery(this);
        }
    }

    public class DeliveryType {
        public long Id { get; set; }
        public string Name { get; set; }

        public DeliveryType() {
            Id = 0;
            Name = "";
        }
    }

    public class DeliveryVariance {
        public bool HasVariance { get; set; }
        public Delivery Delivery { get; set; }
        public Delivery Variance { get; set; }
        public Delivery Recorded { get; set; }

        public DeliveryVariance()
        {
            HasVariance = false;
            Delivery = new Delivery();
            Variance = new Delivery();
            Recorded = new Delivery();
        }
    }
}
