using System;
namespace Core.Models
{
    public class Suppliers
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }

        public Suppliers() {
            Id = 0;
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
