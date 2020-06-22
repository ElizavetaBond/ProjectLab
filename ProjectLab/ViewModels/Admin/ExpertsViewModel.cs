using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Admin
{
    public class ExpertsViewModel
    {
        public string SectionId { get; set; }
        public string DirectionName { get; set; }
        public List<string> ExpertsId { get; set; }
        public List<UserViewModel> Users { get; set; }
    }
}
