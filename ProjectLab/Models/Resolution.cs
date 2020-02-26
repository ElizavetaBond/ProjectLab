using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class Resolution
    {
        public string IdExpert { get; set; }    // идентификатор эксперта
        public bool IsPositive { get; set; }    // позитивное ли решение
        public int ValueDegree { get; set; }    // степень ценности
        public string Remark { get; set; }      // замечание эксперта
    }
}
