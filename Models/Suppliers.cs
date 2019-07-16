using System;
namespace Core.Models
{
    public class Suppliers {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public double Balance { get; set; }
        public bool Fuel { get; set; }
        public bool Lube { get; set; }
        public bool Gas { get; set; }
        public bool Soda { get; set; }

        public Suppliers() {
            Id = 0;
            Uuid = "";
            Name = "";
            Pin = "";
            Address = "";
            City = "";
            Telephone = "";
        }

        public Suppliers(long idnt) : this() {
            Id = idnt;
        }

        public Suppliers(long idnt, string name) : this() {
            Id = idnt;
            Name = name;
        }
    }
}
