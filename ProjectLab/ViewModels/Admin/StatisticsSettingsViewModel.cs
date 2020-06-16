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
        [Display(Name = "Выберите сравниваемую категорию:")]
        public string ComparedCategory { get; set; }

        [Required]
        [Display(Name = "Выберите измеряемую величину:")]
        public string MeasuredQuantity { get; set; }

        [Required]
        [Display(Name = "Фильтр по дате:")]
        public DateTime Begin { get; set; }

        [Required]
        [Display(Name = "------")]
        public DateTime End { get; set; }

        [Display(Name = "Фильтр по учебным заведениям:")]
        public List<string> EducationalInstitutionsId { get; set; }

        [Display(Name = "Фильтр по областям специализации:")]
        public List<string> DirectionsId { get; set; }

        [Display(Name = "Фильтр по категориям пользователей:")]
        public List<string> UserCategoriesId { get; set; }
    }
}
