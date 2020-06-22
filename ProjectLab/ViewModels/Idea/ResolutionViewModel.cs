using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class ResolutionViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string IdeaId { get; set; }

        [Display(Name = "Ваше решение:")]
        public int Decision { get; set; }    // позитивное ли решение

        [Required(ErrorMessage = "Укажите степень ценности идеи!")]
        [Display(Name = "Степень ценности (1-10):")]
        [Range(1, 10, ErrorMessage = "Недопустимое значение")]
        public int ValueDegree { get; set; }    // степень ценности

        [Required(ErrorMessage = "Оставьте комментарий пользователю!")]
        [Display(Name = "Комментарий:")]
        public string Comment { get; set; }      // замечание эксперта
    }
}
