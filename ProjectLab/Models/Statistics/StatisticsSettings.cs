using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public class StatisticsSettings
    {
        private ProjectLabDbService db;
        public string ComparedCategory { get; set; }
        public string MeasuredQuantity { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public List<Direction> Directions { get; set; }
        public List<EducationalInstitution> EducationalInstitutions { get; set; }
        public List<UserCategory> UserCategories { get; set; }
        public StatisticsSettings(ProjectLabDbService _db, string categ, string quant, DateTime start, DateTime finish,
                          List<string> dirsId, List<string> edsId, List<string> categsId)
        {
            db = _db;
            ComparedCategory = categ;
            MeasuredQuantity = quant;
            Start = start;
            Finish = finish;
            if (dirsId != null && dirsId.Any())
            {
                Directions = new List<Direction>();
                foreach (var dir in dirsId)
                    Directions.Add(db.Directions.Find(d => d.Id == dir).FirstOrDefault());
            }
            else
                Directions = db.Directions.Find(new BsonDocument()).ToList();

            if (edsId != null && edsId.Any())
            {
                EducationalInstitutions = new List<EducationalInstitution>();
                foreach (var ed in edsId)
                    EducationalInstitutions.Add(db.EducationalInstitutions.Find(e => e.Id == ed).FirstOrDefault());
            }
            else
                EducationalInstitutions = db.EducationalInstitutions.Find(new BsonDocument()).ToList();

            if (categsId != null && categsId.Any())
            {
                UserCategories = new List<UserCategory>();
                foreach (var cat in categsId)
                    UserCategories.Add(db.UserCategories.Find(u => u.Id == cat).FirstOrDefault());
            }
            else
                UserCategories = db.UserCategories.Find(new BsonDocument()).ToList();
        }
    }
}
