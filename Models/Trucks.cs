using System;
namespace Core.Models
{
    public class Trucks
    {
        public long Id { get; set; }
        public string Registration { get; set; }

        public Trucks() {
            Id = 0;
            Registration = "";
        }

        public Trucks(long idnt) : this() {
            Id = idnt;
        }

        public Trucks(long idnt, string registration) : this() {
            Id = idnt;
            Registration = registration;
        }
    }
}
