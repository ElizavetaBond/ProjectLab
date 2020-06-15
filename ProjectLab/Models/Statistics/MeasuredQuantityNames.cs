using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public static class MeasuredQuantityNames
    {
        public static readonly string ApprovedIdeas;

        public static readonly string WorkProjectsForParticipants;

        public static readonly string WorkProjectsForManagers;

        public static readonly string ArchieveProjectsForParticipants;

        public static readonly string ArchieveProjectsForManagers;

        public static readonly string ActivityUsers;

        static MeasuredQuantityNames()
        {
            ApprovedIdeas = "Утвержденные идеи";
            WorkProjectsForParticipants = "Рабочие проекты (участники)";
            WorkProjectsForManagers = "Рабочие проекты (руководители)";
            ArchieveProjectsForParticipants = "Завершенные проекты (участники)";
            ArchieveProjectsForManagers = "Завершенные проекты (руководители)";
            ActivityUsers = "Активные пользователи";
        }

    }
}
