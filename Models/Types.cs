using System;
namespace Core.Models
{
    public class Types
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Types() {
            Id = 0;
            Code = "";
            Name = "";
        }
    }
}
