using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectLab.Models.References;
using MongoDB.Bson;

namespace ProjectLab.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }

        public UserStatus UserStatus { get; set; }
        public EducationalInstitution EducationalInstitution { get; set; }
    }
}
