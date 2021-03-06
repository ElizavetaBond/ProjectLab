﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Expert
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              // идентификатор
    
        public Direction Direction { get; set; }

        public EducationalInstitution EducationalInstitution { get; set; }

        public List<Idea> ReviewIdeas { get; set; }
    }
}
