using MongoDB.Driver;
using ProjectLab.StaticNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public class StatisticsUserCategories : Statistics
    {
        public StatisticsUserCategories(AdminService serv, StatisticsSettings settings) : base(serv, settings)
        {
            foreach (var x in UserCategories)
            {
                KeyValues.Add(new KeyValue { Key = x.Name, Value = 0, Id = x.Id });
            }
        }

        protected override void CountApprovedIdeas() // подсчет утвержденных идей для каждого УЗ согласно фильтрам
        {
            var ideas = db.GetIdeas().FindAll(x => x.IdeaStatus.Name == IdeaStatusesNames.Approved && x.Date >= Start && x.Date <= Finish);
            foreach (var idea in ideas)
            {
                var us = db.GetUser(idea.AuthorId);
                if (Directions.Find(x => x.Id == idea.Direction.Id) != null 
                        && EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null)
                {
                    var ind = KeyValues.FindIndex(x => x.Id == us.UserCategory.Id);
                    if (ind != -1)
                        KeyValues[ind].Value ++;
                }
            }
        }

        protected override void CountCreatedProjects()
        {
            var projects = db.GetProjects().FindAll(x => (x.ProjectStatus.Name == ProjectStatusesNames.Working
                             || x.ProjectStatus.Name == ProjectStatusesNames.Completed) && x.Start >= Start && x.Start <= Finish);
            foreach (var project in projects)
            {
                var us = db.GetUser(project.ManagerId);
                if (Directions.Find(x => x.Id == project.Idea.Direction.Id) != null 
                        && EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null)
                {
                    var ind = KeyValues.FindIndex(x => x.Id == us.UserCategory.Id);
                    if (ind != -1)
                        KeyValues[ind].Value += 1;
                }
            }
        }

        protected override void CountParticipantsInProjects()
        {
            var projects = db.GetProjects().FindAll(x => (x.ProjectStatus.Name == ProjectStatusesNames.Working
                || x.ProjectStatus.Name == ProjectStatusesNames.Completed) && x.Start >= Start && x.Start <= Finish);
            foreach (var project in projects)
            {
                if (Directions.Find(x => x.Id == project.Idea.Direction.Id) != null)
                {
                    foreach (var participant in project.ParticipantsId)
                    {
                        var us = db.GetUser(participant);
                        if (EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id)  != null)
                        {
                            var ind = KeyValues.FindIndex(x => x.Id == us.UserCategory.Id);
                            if (ind != -1)
                                KeyValues[ind].Value++;
                        }
                    }
                }
            }
        }

        protected override void CountArchieveProjects()
        {
            var projects = db.GetProjects().FindAll(x => x.ProjectStatus.Name == ProjectStatusesNames.Completed
                                    && x.Finish >= Start && x.Finish <= Finish);
            foreach (var project in projects)
            {
                var us = db.GetUser(project.ManagerId);
                if (Directions.Find(x => x.Id == project.Idea.Direction.Id) != null
                            && EducationalInstitutions.Find(x => x.Id == us.EducationalInstitution.Id) != null)
                {
                    var ind = KeyValues.FindIndex(x => x.Id == us.UserCategory.Id);
                    if (ind != -1)
                        KeyValues[ind].Value++;
                }
            }
        }

        protected override void CountRegisteredUsers()
        {
            for (int i = 0; i < KeyValues.Count; i++)
            {
                var users = db.GetUsers().FindAll(x => x.UserCategory.Id == KeyValues[i].Id
                                        && x.RegistDate >= Start && x.RegistDate <= Finish);
                foreach (var user in users)
                {
                    if (Directions.Find(x => x.Id == user.Direction.Id) != null
                            && EducationalInstitutions.Find(x => x.Id == user.EducationalInstitution.Id) != null)
                        KeyValues[i].Value++;
                }
            }
        }
    }
}
