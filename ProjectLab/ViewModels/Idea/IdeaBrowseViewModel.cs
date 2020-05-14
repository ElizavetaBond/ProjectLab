using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaBrowseViewModel
    {
        [Display(Name = "Название идеи")]
        public string Name { get; set; }

        [Display(Name = "Уровень доступа")]
        public string IdeaType { get; set; }

        [Display(Name = "Цель")]
        public string Target { get; set; }

        [Display(Name = "Назначение")]
        public string Purpose { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Перечень оборудования и материалов")]
        public string Equipment { get; set; }

        [Display(Name = "Техника безопасности")]
        public string Safety { get; set; }

        [Display(Name = "Направленность")]
        public string Direction { get; set; }

        [Display(Name = "Автор")]
        public string AuthorId { get; set; }

        [Display(Name="Ссылка на видео")]
        public string Video { get; set; }

        [Display(Name="Изображение")]
        public string ImageId { get; set; }

        public string IdeaStatus { get; set; }

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

