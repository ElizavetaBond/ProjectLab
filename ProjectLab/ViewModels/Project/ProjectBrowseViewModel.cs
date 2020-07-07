using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class ProjectBrowseViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Руководитель")]
        public string ManagerId { get; set; }

        public string IdeaType { get; set; }

        [Display(Name = "Уровень доступа")]
        public string ProjectType { get; set; }

        [Display(Name = "Автор идеи")]
        public string AuthorIdeaId { get; set; }

        [Display(Name = "Цель")]
        public string Target { get; set; }          // цель

        [Display(Name = "Назначение")]
        public string Purpose { get; set; }         // назначение

        [Display(Name = "Описание")]
        public string Description { get; set; }     // описание

        [Display(Name = "Техническое оснащение")]
        public string Equipment { get; set; }       // тех оснащение

        [Display(Name = "Техника безопасности")]
        public string Safety { get; set; }          // техника безопасности

        [Display(Name = "Направленность")]
        public string Direction { get; set; }

        [Display(Name = "Видео")]
        public string Video { get; set; }

        [Display(Name = "Изображение")]
        public File Image { get; set; } // ссылка на файл изображения

        [Display(Name = "Дата старта")]
        public DateTime Start { get; set; }                 // дата старта проекта

        [Display(Name = "Дата окончания")]
        public DateTime Finish { get; set; }                // дата окончания проекта
        public List<string> ParticipantsId { get; set; }
        public List<SectionBrowseProjectViewModel> Sections { get; set; }
        public bool IsParticipant { get; set; }
        public bool IsManager { get; set; }
        public bool IsWork { get; set; }
        public bool IsFinish { get; set; }
    }

    public class SectionBrowseProjectViewModel
    {
        public string SectionId { get; set; }
        public string Name { get; set; }                // название раздела
        public string SectionType { get; set; }         // тип раздела
        public string Description { get; set; }
        public bool isFill { get; set; }
        public List<ComponentBrowseProjectViewModel> Components { get; set; } // компоненты в разделе
        public List<AnswearBrowseProjectViewModel> Answears { get; set; }
    }

    public class ComponentBrowseProjectViewModel
    {
        public string Name { get; set; }                // название компоненты
        public string ComponentType { get; set; }       // тип компоненты
        public string Description { get; set; }         // описание компоненты
        public bool IsNecessary { get; set; }           // необходимость заполнения
        public string Value { get; set; }
        public IFormFile File { get; set; }
        public List<string> ListSelect { get; set; }
        public List<bool> ListRes { get; set; }
        public bool Flag { get; set; }
    }

    public class AnswearBrowseProjectViewModel
    {
        public string ProjectId { get; set; }
        public int SectionNum { get; set; }
        public string AuthorId { get; set; }
        public DateTime Date { get; set; }
        public List<ComponentBrowseProjectViewModel> Components { get; set; }
    }
}