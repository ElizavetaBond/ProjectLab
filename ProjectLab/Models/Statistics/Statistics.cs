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
        public string NameX { get; set; }
        public string NameY { get; set; }
        public List<KeyValue> KeyValues { get; set; }
        protected DateTime Start { get; set; }
        protected DateTime Finish { get; set; }
        protected List<Direction> Directions { get; set; }
        protected List<EducationalInstitution> EducationalInstitutions { get; set; }
        protected List<UserCategory> UserCategories { get; set; }

        public Statistics(ProjectLabDbService serv, Filtrs filtrs)
        {
            db = serv;
            Title = filtrs.NameX + " / " + filtrs.NameY;
            NameX = filtrs.NameX;
            NameY = filtrs.NameY;
            KeyValues = new List<KeyValue>();
            Start = filtrs.Start;
            Finish = filtrs.Finish;
            Directions = filtrs.Directions;
            EducationalInstitutions = filtrs.EducationalInstitutions;
            UserCategories = filtrs.UserCategories;
        }

        public abstract void Generate();
        protected abstract void CountApprovedIdeas();
        protected abstract void CountWorkProjectsForParticipants();
        protected abstract void CountWorkProjectsForManagers();
        protected abstract void CountArchieveProjectsForParticipants();
        protected abstract void CountArchieveProjectsForManagers();
        protected abstract void CountActivityUsers();
    }
}
