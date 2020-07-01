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
        public string ComparedCategory { get; set; }
        public string MeasuredQuantity { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public List<Direction> Directions { get; set; }
        public List<EducationalInstitution> EducationalInstitutions { get; set; }
        public List<UserCategory> UserCategories { get; set; }
        public StatisticsSettings(List<Direction> directions, List<EducationalInstitution> educInsts, List<UserCategory> usCats, 
                                    string categ, string quant, DateTime start, DateTime finish,
                                    List<string> dirsId, List<string> edsId, List<string> categsId)
        {
            ComparedCategory = categ;
            MeasuredQuantity = quant;
            Start = start;
            Finish = finish;
            if (dirsId != null && dirsId.Any())
            {
                Directions = new List<Direction>();
                foreach (var dir in dirsId)
                    Directions.Add(directions.Find(x => x.Id == dir));
            }
            else
                Directions = directions;

            if (edsId != null && edsId.Any())
            {
                EducationalInstitutions = new List<EducationalInstitution>();
                foreach (var ed in edsId)
                    EducationalInstitutions.Add(educInsts.Find(x => x.Id == ed));
            }
            else
                EducationalInstitutions = educInsts;

            if (categsId != null && categsId.Any())
            {
                UserCategories = new List<UserCategory>();
                foreach (var cat in categsId)
                    UserCategories.Add(usCats.Find(x => x.Id == cat));
            }
            else
                UserCategories = usCats;
        }
    }
}
