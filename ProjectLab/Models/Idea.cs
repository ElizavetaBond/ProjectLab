using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectLab.Models
{
    public class Idea
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              // идентификатор
        public string Name { get; set; }            // название
        public string IdeaType { get; set; }        // тип идеи (открытая, приватная)
        public string Target { get; set; }          // цель
        public string Purpose { get; set; }         // назначение
        public string Description { get; set; }     // описание
        public string Equipment { get; set; }       // тех оснащение
        public string Safety { get; set; }          // техника безопасности
        public int ValueDegree { get; set; }        // степень ценности

        public User Author { get; set; }            // ссылка на автора
        public IdeaStatus IdeaStatus { get; set; }  // ссылка на статус идеи
        public Direction Direction { get; set; }    // ссылка на направленность идеи

        
        public ProjectTemplate ProjectTemplate { get; set; }    // шаблон проекта
        public List<Resolution> Resolutions { get; set; }       // резолюции 
        public List<Comment> Comments { get; set; }             // комментарии

        // фото
        // видео
    }
}
