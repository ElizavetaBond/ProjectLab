﻿using MongoDB.Bson;
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
        public DateTime Date { get; set; }


        public string AuthorId { get; set; }
        public IdeaStatus IdeaStatus { get; set; }  // ссылка на статус идеи
        public Direction Direction { get; set; }    // ссылка на направленность идеи

        public string ReviewId { get; set; }

        
        public ProjectTemplate ProjectTemplate { get; set; }    // шаблон проекта
        public List<Comment> Comments { get; set; }             // комментарии


        public string Video { get; set; }
        public File Image { get; set; } // ссылка на файл изображения

    }
}
