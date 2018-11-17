using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Core.ViewModel
{
    public class IndexViewModel
    {
        public List<Stations> Pending { get; set; }
        public List<Stations> Updated { get; set; }

        public IndexViewModel()
        {
            Pending = new List<Stations>();
            Updated = new List<Stations>();
        }
    }
}
