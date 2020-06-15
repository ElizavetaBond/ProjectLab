using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public class StatisticsEducationalInstitutions: Statistics
    {
        public StatisticsEducationalInstitutions (ProjectLabDbService serv, Filtrs filtrs) : base(serv, filtrs) 
        { 
            foreach (var x in EducationalInstitutions)
            {
                KeyValues.Add(new KeyValue { Key = x.Name, Value = 0, Id = x.Id });
            }
        }

        public override void Generate() //  вызов нужного метода в зависимости от измеряемой величины
        {
            if (NameY == MeasuredQuantityNames.ApprovedIdeas)
                CountApprovedIdeas();
            else if (NameY == MeasuredQuantityNames.WorkProjectsForParticipants)
                CountWorkProjectsForParticipants();
            else if (NameY == MeasuredQuantityNames.WorkProjectsForManagers)
                CountWorkProjectsForManagers();
            else if (NameY == MeasuredQuantityNames.ArchieveProjectsForParticipants)
                CountArchieveProjectsForParticipants();
            else if (NameY == MeasuredQuantityNames.ArchieveProjectsForManagers)
                CountArchieveProjectsForManagers();
            else if (NameY == MeasuredQuantityNames.ActivityUsers)
                CountActivityUsers();
        }

        protected override void CountApprovedIdeas() // подсчет утвержденных идей для каждого УЗ согласно фильтрам
        {
            var ideas = db.Ideas.Find(x => x.IdeaStatus.Name == "Утверждена" && x.Date >= Start && x.Date <= Finish ).ToList();
            foreach (var idea in ideas)
            {
                var us = db.Users.Find(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (Directions.Find(x => x.Id == idea.Direction.Id) != null && UserCategories.Find( x => x.Id == us.UserCategory.Id)  != null )
                {
                    var ind = KeyValues.FindIndex(x => x.Id == us.EducationalInstitution.Id);
                    if (ind != -1)
                        KeyValues[ind].Value += 1;
                }
            }
        }

        protected override void CountWorkProjectsForParticipants()
        {
            
        }

        protected override void CountWorkProjectsForManagers()
        {
            
        }

        protected override void CountArchieveProjectsForParticipants()
        {
            
        }

        protected override void CountArchieveProjectsForManagers()
        {
            
        }

        protected override void CountActivityUsers()
        {
            
        }
    }
}
