using System;
namespace Core.Models
{
    public class ExpensesCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ExpensesCategory() {
            Id = 0;
            Name = "";
        }

        public ExpensesCategory(long idnt, string name) : this() {
            Id = idnt;
            Name = name;
        }
    }
}
