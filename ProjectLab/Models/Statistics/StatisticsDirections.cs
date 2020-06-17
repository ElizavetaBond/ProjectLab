using MongoDB.Driver;
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
                var ideas = db.Ideas.Find(x => x.IdeaStatus.Name == "Утверждена" && x.Date >= Start && x.Date <= Finish && 
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
                var projects = db.Projects.Find(x => (x.ProjectStatus.Name == "Рабочий" || x.ProjectStatus.Name == "Завершенный")
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

        /*protected override void CountProjectsWithParticipants()
        {
            var projects = db.Projects.Find(x => (x.ProjectStatus.Name == "Рабочий" || x.ProjectStatus.Name == "Завершенный") && x.Start >= Start && x.Start <= Finish).ToList();
            foreach (var project in projects)
            {
                var ind = Directions.FindIndex(x => x.Id == project.Idea.Direction.Id);
                if ( ind != -1 )
                {
                    foreach (var participant in project.ParticipantsId)
                    {
                        var us = db.Users.Find(x => x.Id == participant).FirstOrDefault();
                        if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null 
                                        && UserCategories.Find(x => x.Id == us.UserCategory.Id) != null)
                        {
                            var ind = KeyValues.FindIndex(x => x.Id == us.EducationalInstitution.Id);
                            if (ind != -1)
                                KeyValues[ind].Value += 1;
                        }
                    }
                }
            }
        }*/

        protected override void CountArchieveProjects()
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var projects = db.Projects.Find(x => x.ProjectStatus.Name == "Завершенный" && x.Finish >= Start && x.Finish <= Finish
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
