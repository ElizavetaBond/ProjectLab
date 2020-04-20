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
 
        public bool IsPositive { get; set; }    // позитивное ли решение

        [Required(ErrorMessage = "При утверждении идеи, ее необходимо оценить!")]
        [Display(Name = "Степень ценности (1-10):")]
        [Range(1, 10, ErrorMessage = "Недопустимое значение")]
        public int ValueDegree { get; set; }    // степень ценности

        [Required(ErrorMessage = "Укажите причину отказа!")]
        [Display(Name = "Комментарий:")]
        public string Remark { get; set; }      // замечание эксперта
    }
}
