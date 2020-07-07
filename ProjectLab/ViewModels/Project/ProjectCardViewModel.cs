using ProjectLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class ProjectCardViewModel
    {
        public string Id { get; set; }                      // идентификатор
        public string Name { get; set; }                    // название идеи
        public string Direction { get; set; }               // направленность идеи
        public string ManagerId { get; set; }                  // автор идеи
        public string ProjectType { get; set; }
        public File Image { get; set; }
        public bool IsManager { get; set; }
        public bool IsParticipant { get; set; }
        public bool IsFinish { get; set; }
    }
}
