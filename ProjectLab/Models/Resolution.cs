using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Resolution
    {
        public string ExpertId { get; set; }
        public int Decision { get; set; }       // решение Эксперта (1-утвердить, -1-отклонить, 0-на доработку
        public int ValueDegree { get; set; }    // степень ценности
        public string Comment { get; set; }     // замечание эксперта
        public DateTime DateReview { get; set; }
    }
}
