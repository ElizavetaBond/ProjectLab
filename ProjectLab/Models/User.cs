using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectLab.Models.References;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;

namespace ProjectLab.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              // идентификатор
        public string Email { get; set; }
        public string Password { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }

        public UserStatus UserStatus { get; set; }
        public UserCategory UserCategory { get; set; }
        public EducationalInstitution EducationalInstitution { get; set; }
        public Education Education { get; set; }
        //public List<Direction> Directions { get; set; }
        public Direction Direction { get; set; }
        public List<Reward> Rewards { get; set; }

        public string Contacts { get; set; }
        public File Photo { get; set; }
        public string AddInform { get; set; }
    }
}
