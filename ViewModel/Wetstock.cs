using System.Collections.Generic;
using Core.Models;

namespace Core.ViewModel
{
    public class WetstockModel
    {
        public List<Wetstock> Model { get; set; }
        public WetstockModel() {
            Model = new List<Wetstock>();
        }
    }
}
