using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Stations
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Synonym { get; set; }
        public string Prefix { get; set; }
        public DateTime Push { get; set; }
        public Boolean Updated { get; set; }

        public StationsBrand Brand { get; set; }

        public Double FuelLtrs { get; set; }
        public Double FuelSales { get; set; }
        public Double LubeSales { get; set; }

        public Stations() {
            Id = 0;
            Code = "";
            Name = "";
            Synonym = "";
            Prefix = "";
            Updated = false;
            Push = new DateTime();
            FuelLtrs = 0;
            FuelSales = 0;
            LubeSales = 0;

            Brand = new StationsBrand();
        }

        public Stations(long idnt) : this() {
            Id = idnt;
        }

        public Stations(long idnt, string name) : this() {
            Id = idnt;
            Name = name;
        }

        public Stations(long idnt, string code, string name) : this() {
            Id = idnt;
            Code = code;
            Name = name;
        }
    }
}
