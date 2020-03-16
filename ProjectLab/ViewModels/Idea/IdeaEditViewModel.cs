using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectLab.Models;
using ProjectLab.Models.References;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaEditViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdeaType { get; set; }
        public string Target { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public string Equipment { get; set; }
        public string Safety { get; set; }
        public string IdDirection { get; set; }
        public List<SectionEditViewModel> Sections { get; set; } // список разделов шаблона проекта
        public List<ComponentEditViewModel> Components { get; set; } // список компонент
    }

    public class SectionEditViewModel
    {
        public string Name { get; set; } // название раздела
        public string SectionType { get; set; }
        public bool IsDelete { get; set; } // признак удаления
    }

    public class ComponentEditViewModel
    {
        public string Name { get; set; } // название компоненты
        public string Type { get; set; } // тип компоненты
        public string Description { get; set; } // описание компоненты
        public bool IsNecessary { get; set; } // признак необходимости заполнения
        public int Section { get; set; } // номер раздела, к которому относится
        public bool IsDelete { get; set; } // признак удаления
    }
}
