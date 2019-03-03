using System;
namespace Core.Models
{
    public class Suppliers
    {
        public Int64 Id { get; set; }
        public String Name { get; set; }
        public String Pin { get; set; }

        public Suppliers() {
            Id = 0;
            Name = "";
            Pin = "";
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
