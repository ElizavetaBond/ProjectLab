using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Project
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }        // название
        public Idea Idea { get; set; }          // идея
        public User Manager { get; set; }       // руководитель
        public DateTime Start { get; set; }     // дата старта проекта
        public DateTime Finish { get; set; }    // дата окончания проекта
        public string Description { get; set; } // описание проекта
    }
}
