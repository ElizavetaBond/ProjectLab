using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Admin
{
    public class ChartViewModel
    {
        public string Title { get; set; }
        public string ComparedCategory { get; set; }
        public string MeasuredQuantity { get; set; }
        public class KeyValueViewModel
        {
            public string Key { get; set; }
            public long Value { get; set; }
        }
        public List<KeyValueViewModel> KeyValues { get; set; }
    }
}
