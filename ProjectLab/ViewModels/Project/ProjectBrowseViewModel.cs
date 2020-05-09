using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class ProjectBrowseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string ProjectType { get; set; }
        public string AuthorIdea { get; set; }
        public string Target { get; set; }          // цель
        public string Purpose { get; set; }         // назначение
        public string Description { get; set; }     // описание
        public string Equipment { get; set; }       // тех оснащение
        public string Safety { get; set; }          // техника безопасности
        public string Direction { get; set; }
        public string Video { get; set; }
        public string ImageId { get; set; } // ссылка на файл изображения
        public DateTime Start { get; set; }                 // дата старта проекта
        public DateTime Finish { get; set; }                // дата окончания проекта
        public List<string> Participants { get; set; }
        //public List<Section> Template

    }
}
