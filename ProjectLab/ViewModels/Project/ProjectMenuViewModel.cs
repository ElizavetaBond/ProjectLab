using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class ProjectMenuViewModel
    {
        public List<ProjectCardViewModel> Participation { get; set; }
        public List<ProjectCardViewModel> Managment { get; set; }
        public List<ProjectCardViewModel> Archive { get; set; }
        public List<ProjectCardViewModel> Canceled { get; set; }

    }
}
