using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class ResolutionViewModel
    {
        public bool IsPositive { get; set; }    // позитивное ли решение
        public int ValueDegree { get; set; }    // степень ценности
        public string Remark { get; set; }      // замечание эксперта
    }
}
