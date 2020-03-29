﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class ProjectTemplate
    {
        public List<Section> Sections { get; set; }     // список разделов
    }

    public class Section
    {
        public string Name { get; set; }                // название раздела
        public string SectionType { get; set; }         // тип раздела
        public List<Component> Components { get; set; } // компоненты в разделе

    }
    public class Component
    {
        public string Name { get; set; }                // название компоненты
        public string ComponentType { get; set; }       // тип компоненты
        public string Description { get; set; }         // описание компоненты
        public bool IsNecessary { get; set; }           // необходимость заполнения
    }
}