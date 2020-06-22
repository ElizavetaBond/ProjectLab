using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class ProjectStatusesNames
    {
        public static readonly string Working;

        public static readonly string Completed;

        public static readonly string Canceled;

        static ProjectStatusesNames()
        {
            Working = "Рабочий";
            Completed = "Завершенный";
            Canceled = "Отмененный";
        }
    }
}
