using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels
{
    public class IdeaViewModel
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Direction { get; set; }
        public string Target { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public string Equipment { get; set; }
        public string Safety { get; set; }
        public List<Section> Sections { get; set; } // список разделов шаблона проекта
        public List<Component> Components { get; set; } // список компонент
    }

    public class Section
    {
        public string Name { get; set; } // название раздела
        public bool IsDelete { get; set; } // признак удаления
    }

    public class Component
    {
        public string Name { get; set; } // название компоненты
        public string Type { get; set; } // тип компоненты
        public string Description { get; set; } // описание компоненты
        public bool IsNecessary { get; set; } // признак необходимости заполнения
        public int Section { get; set; } // номер раздела, к которому относится
        public bool IsDelete { get; set; } // признак удаления
    }
}
