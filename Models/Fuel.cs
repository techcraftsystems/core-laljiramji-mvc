using System;
namespace Core.Models
{
    public class Fuel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public Fuel() {
            Id = 0;
            Name = "";
        }

        public Fuel(long idnt) : this() {
            Id = idnt;
        }
    }
}
