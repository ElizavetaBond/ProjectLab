using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models.Statistics
{
    public class Filtrs
    {
        private ProjectLabDbService db;
        public string NameX { get; set; }
        public string NameY { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public List<Direction> Directions { get; set; }
        public List<EducationalInstitution> EducationalInstitutions { get; set; }
        public List<UserCategory> UserCategories { get; set; }
        public Filtrs(ProjectLabDbService _db, string nameX, string nameY, DateTime start, DateTime finish,
                          List<string> dirsId, List<string> edsId, List<string> categsId)
        {
            db = _db;
            NameX = nameX;
            NameY = nameY;
            Start = start;
            Finish = finish;
            if (dirsId.Any())
            {
                Directions = new List<Direction>();
                foreach (var dir in dirsId)
                    Directions.Add(db.Directions.Find(d => d.Id == dir).FirstOrDefault());
            }
            else
                Directions = db.Directions.Find(new BsonDocument()).ToList();

            if (edsId.Any())
            {
                EducationalInstitutions = new List<EducationalInstitution>();
                foreach (var ed in edsId)
                    EducationalInstitutions.Add(db.EducationalInstitutions.Find(e => e.Id == ed).FirstOrDefault());
            }
            else
                EducationalInstitutions = db.EducationalInstitutions.Find(new BsonDocument()).ToList();

            if (categsId.Any())
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
