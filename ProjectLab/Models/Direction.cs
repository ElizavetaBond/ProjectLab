using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    [BsonIgnoreExtraElements]
    public class Direction
    {
        public string Name { get; set; }
    }
}
