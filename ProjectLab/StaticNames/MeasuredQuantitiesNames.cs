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

        public static readonly string ParticipantsInProjects;

        public static readonly string ArchieveProjects;

        public static readonly string RegisteredUsers;

        static MeasuredQuantitiesNames()
        {
            ApprovedIdeas = "Количество утвержденных идей";
            CreatedProjects = "Количество созданных проектов";
            ParticipantsInProjects = "Общее количество участников в проектах";
            ArchieveProjects= "Количество завершенных проектов";
            RegisteredUsers = "Количество зарегистированных пользователей";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                ApprovedIdeas, CreatedProjects, ParticipantsInProjects, ArchieveProjects, RegisteredUsers
            };
        }
    }
}
