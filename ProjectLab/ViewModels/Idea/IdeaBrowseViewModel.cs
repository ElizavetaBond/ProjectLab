using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaBrowseViewModel
    {
        public string Name { get; set; }
        public string IdeaType { get; set; }
        public string Target { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public string Equipment { get; set; }
        public string Safety { get; set; }
        public string Direction { get; set; }
        public string Author { get; set; }
        public List<SectionBrowseViewModel> Sections { get; set; } // список разделов шаблона проекта
    }

    public class SectionBrowseViewModel
    {
        public string Name { get; set; } // название раздела
        public string SectionType { get; set; }
        public List<ComponentBrowseViewModel> Components { get; set; }
    }

    public class ComponentBrowseViewModel
    {
        public string Name { get; set; } // название компоненты
        public string Type { get; set; } // тип компоненты
        public string Description { get; set; } // описание компоненты
    }
}

