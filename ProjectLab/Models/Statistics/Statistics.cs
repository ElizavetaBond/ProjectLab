using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ProjectLab.Models.Statistics
{
    public abstract class Statistics
    {
        protected ProjectLabDbService db;
        public string Title { get; set; }
        public string ComparedCategory { get; set; }
        public string MeasuredQuantity { get; set; }
        public List<KeyValue> KeyValues { get; set; }
        protected DateTime Start { get; set; }
        protected DateTime Finish { get; set; }
        protected List<Direction> Directions { get; set; }
        protected List<EducationalInstitution> EducationalInstitutions { get; set; }
        protected List<UserCategory> UserCategories { get; set; }

        public Statistics(ProjectLabDbService serv, StatisticsSettings settings)
        {
            db = serv;
            Title = settings.ComparedCategory + " / " + settings.MeasuredQuantity;
            ComparedCategory = settings.ComparedCategory;
            MeasuredQuantity = settings.MeasuredQuantity;
            KeyValues = new List<KeyValue>();
            Start = settings.Start;
            Finish = settings.Finish;
            Directions = settings.Directions;
            EducationalInstitutions = settings.EducationalInstitutions;
            UserCategories = settings.UserCategories;
        }

        public abstract void Generate();
        protected abstract void CountApprovedIdeas();
        protected abstract void CountCreatedProjects();
        protected abstract void CountProjectsWithParticipants();
        protected abstract void CountArchieveProjects();
        protected abstract void CountRegisteredUsers();
    }
}
