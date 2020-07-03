using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaCardViewModel // представление идеи в каталоге идей
    {
        public string Id { get; set; }                      // идентификатор
        public string Name { get; set; }                    // название идеи
        public string Direction { get; set; }               // направленность идеи
        public string AuthorId { get; set; }                  // автор идеи
        public File Image { get; set; }
        public string IdeaType { get; set; }
        public int ValueDegree { get; set; }
    }
}
