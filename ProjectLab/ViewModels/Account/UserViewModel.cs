using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Account
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }

        public string Surname { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }
        public string Patronymic { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        public string UserStatusId { get; set; }
        public string UserCategoryId { get; set; }
        public string EducationalInstitutionId { get; set; }
        public string EducationId { get; set; }
        public List<string> DirectionsId { get; set; }
        public List<string> RewardsId { get; set; }

        public string Contacts { get; set; }
        public string Photo { get; set; }
        public string AddInform { get; set; }
    }
}
