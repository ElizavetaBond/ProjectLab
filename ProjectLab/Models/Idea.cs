using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLab.Models
{
    public class Idea
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; } // название
        public string Target { get; set; } // цель
        public string Purpose { get; set; } // назначение
        public string Description { get; set; } // описание
        public string Equipment { get; set; } // тех оснащение
        public string Safety { get; set; } // техника безопасности

        public IdeaStatus IdeaStatus { get; set; }
        public IdeaType IdeaType { get; set; }
        public Direction Direction { get; set; }
        public User Author { get; set; }

        public string ImageId { get; set; } // ссылка на изображение

        public bool HasImage()
        {
            return !String.IsNullOrWhiteSpace(ImageId);
        }
    }
}
