using System;
using System.Collections.Generic;
using Core.Models;
using Core.Services;

namespace Core.ViewModel
{
    public class BankingMainViewModel
    {
        public List<Bank> Banks { get; set; }
        public Bank Bank { get; set; }

        public List<MonthsModel> Months { get; set; }
        public Int64 Month { get; set; }

        public BankingMainViewModel()
        {
            Banks = new List<Bank>();
            Bank = new Bank();

            Month = DateTime.Now.Month;
            Months = new StationsService().InitializeMonthsModel();
        }
    }
}
