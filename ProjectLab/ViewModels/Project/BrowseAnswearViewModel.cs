using ProjectLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class BrowseAnswearViewModel
    {
        public string AuthorId { get; set; }
        public string Date { get; set; }
        public List<BrowseAnswearComponentViewModel> Components { get; set; }
    }
    public class BrowseAnswearComponentViewModel
    {
        public string Name { get; set; }                // название компоненты
        public string ComponentType { get; set; }       // тип компоненты
        public string Description { get; set; }         // описание компоненты
        public string Value { get; set; }
        public File File { get; set; }
    }
}
