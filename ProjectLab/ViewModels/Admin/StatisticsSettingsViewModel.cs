using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Admin
{
    public class StatisticsSettingsViewModel
    {
        [Required]
        [Display(Name = "Выберите критерий сбора статистики:")]
        public string Criterion { get; set; }

        [Required]
        [Display(Name = "Дата с")]
        public DateTime Begin { get; set; }

        [Required]
        [Display(Name = "по")]
        public DateTime End { get; set; }

        [Display(Name = "Учебные заведения")]
        public string EducationalInstitution { get; set; }

        [Display(Name = "Области специализации")]
        public string Direction { get; set; }
    }
}
