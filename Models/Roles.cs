using System;
namespace Core.Models
{
    public class Roles
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public Roles() {
            Id = 0;
            Name = "";
        }
    }
}
