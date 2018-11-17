using System;

namespace Core.ViewModel
{
    public class MonthsModel
    {
        public Int64 Value { get; set; }
        public String Name { get; set; }
        public Boolean Select { get; set; }

        public MonthsModel()
        {
            Value = 0;
            Name = "";
            Select = false;
        }
    }
}
