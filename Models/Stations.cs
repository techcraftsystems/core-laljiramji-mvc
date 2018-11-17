using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Stations
    {
        [Display(Name = "id")]
        [Required]
        public Int64 Id { get; set; }

        [Required]
        [Display(Name = "name")]
        public String Name { get; set; }

        [Required]
        [Display(Name = "code")]
        public String Code { get; set; }

        [Display(Name = "push date")]
        public DateTime Push { get; set; }

        [Display(Name = "updated")]
        public Boolean Updated { get; set; }

        public StationsBrand Brand { get; set; }

        public Double FuelLtrs { get; set; }
        public Double FuelSales { get; set; }
        public Double LubeSales { get; set; }



        public Stations()
        {
            Id = 0;
            Code = "";
            Name = "";
            Updated = false;
            Push = new DateTime();
            FuelLtrs = 0;
            FuelSales = 0;
            LubeSales = 0;

            Brand = new StationsBrand();
        }
    }
}
