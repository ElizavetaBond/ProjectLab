using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это обязательное поле!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это обязательное поле!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Это обязательное поле!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Это обязательное поле!")]
        public string Name { get; set; }
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Это обязательное поле!")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public string UserStatusId { get; set; }
        public string UserCategoryId { get; set; }
        public string EducationalInstitutionId { get; set; }
        public string EducationId { get; set; }
        public List<string> DirectionsId { get; set; }

        public string Contacts { get; set; }
        public string Photo { get; set; }
        public string AddInform { get; set; }
    }
}
