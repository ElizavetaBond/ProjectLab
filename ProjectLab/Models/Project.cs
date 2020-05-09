using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Project
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }                      // идентификатор
        public Idea Idea { get; set; }                      // идея
        public ProjectStatus ProjectStatus { get; set; }    // статус проекта
        public ProjectType ProjectType { get; set; }        // тип проекта (открытый, закрытый, приватный)
        public string ManagerEmail { get; set; }                   // руководитель
        public DateTime Start { get; set; }                 // дата старта проекта
        public DateTime Finish { get; set; }                // дата окончания проекта
        public List<string> ParticipantsEmail { get; set; }        // участники
        public List<Comment> Comments { get; set; }         // комментарии
        public List<Section> Sections { get; set; }         // разделы из шаблона
    }
}
