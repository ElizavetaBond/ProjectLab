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
        public string Manager { get; set; }                  // автор идеи
        public string EducationalInstitution { get; set; }  // учебное заведение
        public string ProjectType { get; set; }
        public string ImageId { get; set; }
        public bool IsManager { get; set; }
        public bool IsParticipant { get; set; }
    }
}
