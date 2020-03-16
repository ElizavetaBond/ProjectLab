using ProjectLab.Models;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels
{
    public class IdeaCardViewModel // представление идеи в каталоге идей
    {
        public string Id { get; set; }                      // идентификатор
        public string Name { get; set; }                    // название идеи
        public string Direction { get; set; }               // направленность идеи
        public string Author { get; set; }                  // автор идеи
        public string EducationalInstitution { get; set; }  // учебное заведение
    }
}
