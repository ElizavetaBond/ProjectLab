using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using System.Web;
using ProjectLab.Models.References;
using Microsoft.AspNetCore.Http;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaEditViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Название идеи")]
        public string Name { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Уровень доступа")]
        public string IdeaType { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Цель")]
        public string Target { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Назначение")]
        public string Purpose { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Перечень оборудования и материалов")]
        public string Equipment { get; set; }

        [Display(Name = "Техника безопасности")]
        public string Safety { get; set; }

        [Required (ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Направленность")]
        public string DirectionId { get; set; }

        [Url(ErrorMessage = "Некорректная ссылка!")]
        [Display(Name = "Ссылка на видео")]
        public string Video { get; set; }

        //[FileExtensions(Extensions = ".jpg", ErrorMessage = "Некорректный формат")]
        public IFormFile FileImage { get; set; }

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
        public List<string> ListSelect { get; set; }
        public bool IsNecessary { get; set; } // признак необходимости заполнения
        public int Section { get; set; } // номер раздела, к которому относится
        public bool IsDelete { get; set; } // признак удаления
    }
}
