using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Comment
    {
        public string UserId { get; set; }  // идентификатор пользователя
        public string Text { get; set; }    // текстовый комеентарий
    }
}
