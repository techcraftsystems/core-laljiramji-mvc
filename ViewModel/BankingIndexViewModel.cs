using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class BankingIndexViewModel
    {
        public List<Bank> Banks { get; set; }

        public BankingIndexViewModel() {
            Banks = new List<Bank>();
        }
    }
}
