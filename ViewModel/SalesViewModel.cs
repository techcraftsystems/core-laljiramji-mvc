using System;
using System.Collections.Generic;
using Core.DataModel;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;

namespace Core.ViewModel
{
    public class SalesIndexViewModel
    {
        public SalesIndexViewModel()
        {
        }
    }

    public class SalesDeliveriesViewModel
    {
        public IEnumerable<SelectListItem> StationIdnts { get; set; }
        public IEnumerable<SelectListItem> StationCodes { get; set; }
        public IEnumerable<SelectListItem> DeliveryType { get; set; }
        public IEnumerable<SelectListItem> BankAccounts { get; set; }
        public Stations Station { get; set; }
        public List<Delivery> Deliveries { get; set; }
        public List<Delivery> Entries { get; set; }
        public string Code { get; set; }
        public string DateX { get; set; }
        public DateTime Date { get; set; }
        public JObject PettyAccounts;

        public SalesDeliveriesViewModel() {
            StationIdnts = new List<SelectListItem>();
            StationCodes = new List<SelectListItem>();
            DeliveryType = new List<SelectListItem>();
            BankAccounts = new List<SelectListItem>();

            Station = new Stations();
            Code = "";
            Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateX = DateTime.Now.ToString("d MMMM, yyyy");

            Deliveries = new List<Delivery>();
            Entries = new List<Delivery>();
            for (int i = 0; i < 10; i++) {
                Entries.Add(new Delivery());
            }
        }
    }

    public class SalesTransferLedgerViewModel {
        public List<ProductsTransfer> Ledger { get; set; }
        public DateTime Date { get; set; }

        public SalesTransferLedgerViewModel() {
            Ledger = new List<ProductsTransfer>();
            Date = DateTime.Now;
        }
    }

    public class SalesTransferCompareViewModel {
        public List<ProductsTransferCompare> Compare { get; set; }
        public DateTime Date { get; set; }

        public SalesTransferCompareViewModel() {
            Date = DateTime.Now;
            Compare = new List<ProductsTransferCompare>();
        }
    }
}
