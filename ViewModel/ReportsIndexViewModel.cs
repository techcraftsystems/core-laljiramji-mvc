using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class ReportsIndexViewModel
    {
        public List<Stations> Stations { get; set; }
        public DateTime StartDate { get; set; }

        public ReportsIndexViewModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);
        }
    }
}
