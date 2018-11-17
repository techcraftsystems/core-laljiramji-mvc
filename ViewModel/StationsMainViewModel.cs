using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class StationsMainViewModel
    {
        public List<Stations> Stations { get; set; }


        public StationsMainViewModel()
        {
            Stations = new List<Stations>();
        }
    }
}
