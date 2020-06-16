using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class MeasuredQuantitiesNames
    {
        public static readonly string ApprovedIdeas;

        public static readonly string CreatedProjects;

        public static readonly string ProjectsWithParticipants;

        public static readonly string ArchieveProjects;

        public static readonly string RegisteredUsers;

        static MeasuredQuantitiesNames()
        {
            ApprovedIdeas = "Утвержденные идеи";
            CreatedProjects = "Созданные проекты";
            ProjectsWithParticipants = "Проекты с участием";
            ArchieveProjects= "Завершенные проекты";
            RegisteredUsers = "Зарегистированные пользователи";
        }

    }
}
