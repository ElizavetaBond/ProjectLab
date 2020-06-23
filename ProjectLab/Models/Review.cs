using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Review
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              // идентификатор
        public Idea Idea { get; set; }
        public List<string> ExpertsId { get; set; }
        public List<Resolution> Resolutions { get; set; }
        public DateTime DateSending{ get; set; }
    }
}
