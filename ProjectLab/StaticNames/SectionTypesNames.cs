using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class SectionTypesNames
    {
        public static readonly string FinalResults;

        public static readonly string Survey;

        public static readonly string Data;

        static SectionTypesNames()
        {
            FinalResults = "Раздел итоговых результатов";
            Survey = "Раздел опроса";
            Data = "Раздел данных";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                FinalResults, Survey, Data
            };
        }

    }
}
