using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels
{
    public class ProjectCatalogViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public string ImageId { get; set; }
    }
}
