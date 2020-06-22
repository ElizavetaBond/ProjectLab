using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class ComparedCategoriesNames
    {
        public static readonly string EducationalInstitutions;

        public static readonly string Directions;

        public static readonly string UserCategories;

        static ComparedCategoriesNames()
        {
            EducationalInstitutions = "Учебные заведения";
            Directions = "Области специализаций";
            UserCategories = "Категории пользователей";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                EducationalInstitutions, Directions, UserCategories
            };
        }

    }
}
