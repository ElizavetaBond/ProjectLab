using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Admin
{
    public class ChartViewModel
    {
        public string Title { get; set; }
        public string NameX { get; set; }
        public string NameY { get; set; }
        public class KeyValue
        {
            public string Key { get; set; }
            public int Value { get; set; }
        }
        public List<KeyValue> KeyValues { get; set; }
    }
}
