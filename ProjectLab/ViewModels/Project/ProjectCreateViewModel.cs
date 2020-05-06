using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Project
{
    public class ProjectCreateViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string IdeaId { get; set; }

        [Display(Name = "Идея")]
        public string IdeaName { get; set; }

        [Required(ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Уровень доступа")]
        public string ProjectTypeId { get; set; }       

        [Required(ErrorMessage = "Заполните данное поле")]
        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        public DateTime Finish { get; set; }      
    }
}
