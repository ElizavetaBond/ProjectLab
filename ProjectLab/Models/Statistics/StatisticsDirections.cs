using MongoDB.Driver;
using ProjectLab.StaticNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public class StatisticsDirections : Statistics
    {
        public StatisticsDirections(ProjectLabDbService serv, StatisticsSettings settings) : base(serv, settings)
        {
            foreach (var x in Directions)
            {
                KeyValues.Add(new KeyValue { Key = x.Name, Value = 0, Id = x.Id });
            }
        }

        protected override void CountApprovedIdeas() // подсчет утвержденных идей для каждого УЗ согласно фильтрам
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var ideas = db.Ideas.Find(x => x.IdeaStatus.Name == IdeaStatusesNames.Approved && x.Date >= Start && x.Date <= Finish && 
                                               x.Direction.Id == KeyValues[i].Id).ToList();
                foreach (var idea in ideas)
                {
                    var us = db.Users.Find(x => x.Id == idea.AuthorId).FirstOrDefault();
                    if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null
                                                               && UserCategories.Find(x => x.Id == us.UserCategory.Id) != null)
                        KeyValues[i].Value++;
                }

            }
        }

        protected override void CountCreatedProjects() // считает количество опубликованных проектов для обл. спец. согласно фильтрам
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var projects = db.Projects.Find(x => (x.ProjectStatus.Name == ProjectStatusesNames.Working 
                                                  || x.ProjectStatus.Name == ProjectStatusesNames.Completed)
                                                  && x.Start >= Start && x.Start <= Finish
                                                  && x.Idea.Direction.Id == KeyValues[i].Id).ToList();
                foreach (var project in projects)
                {
                    var us = db.Users.Find(x => x.Id == project.ManagerId).FirstOrDefault();
                    if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null
                                                         && UserCategories.Find(x => x.Id == us.UserCategory.Id) != null)
                        KeyValues[i].Value++;
                }
            }
        }

        protected override void CountParticipantsInProjects()
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var projects = db.Projects.Find(x => (x.ProjectStatus.Name == ProjectStatusesNames.Working
                        || x.ProjectStatus.Name == ProjectStatusesNames.Completed) && x.Start >= Start && x.Start <= Finish  
                        && x.Idea.Direction.Id == KeyValues[i].Id).ToList();
                foreach (var project in projects)
                {
                    foreach (var participant in project.ParticipantsId)
                    {
                        var us = db.Users.Find(x => x.Id == participant).FirstOrDefault();
                        if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null
                                && UserCategories.Find(x => x.Id == us.UserCategory.Id) != null)
                            KeyValues[i].Value++;
                    }
                }
            }
        }

        protected override void CountArchieveProjects()
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var projects = db.Projects.Find(x => x.ProjectStatus.Name == ProjectStatusesNames.Completed 
                                                    && x.Finish >= Start && x.Finish <= Finish
                                                    && x.Idea.Direction.Id == KeyValues[i].Id).ToList();
                foreach (var project in projects)
                {
                    var us = db.Users.Find(x => x.Id == project.ManagerId).FirstOrDefault();
                    if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null
                                                               && UserCategories.Find(x => x.Id == us.UserCategory.Id) != null)
                        KeyValues[i].Value++;
                }
            }
        }

        protected override void CountRegisteredUsers()
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var users = db.Users.Find(x => x.Direction.Id == KeyValues[i].Id
                                                && x.RegistDate >= Start && x.RegistDate <= Finish).ToList();
                foreach (var user in users)
                {
                    if (EducationalInstitutions.Find(x => x.Id == user.EducationalInstitution.Id) != null
                                                             && UserCategories.Find(x => x.Id == user.UserCategory.Id) != null)
                        KeyValues[i].Value++;
                }
            }
        }
    }
}
