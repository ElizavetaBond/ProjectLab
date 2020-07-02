using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class SectionTypesNames
    {
        public static readonly string FinalResults;
        public static readonly string FinalResultsDescription;

        public static readonly string Survey;
        public static readonly string SurveyDescription;

        public static readonly string Data;
        public static readonly string DataDescription;

        static SectionTypesNames()
        {
            FinalResults = "Раздел итоговых результатов";
            Survey = "Раздел опроса";
            Data = "Раздел данных";
            FinalResultsDescription = "Тип данного раздела - 'Раздел итоговых результатов'. " +
                "Данный тип раздела предназначен для заполнения руководителем проекта итогов проекта.  " +
                "Данный раздел может быть заполнен однократно. ";
            SurveyDescription = "Тип данного раздела - 'Раздел опроса'. " +
                "Данный тип раздела предназначен для заполнения участниками проекта. " +
                "Данный раздел может быть заполнен одним участником однократно. ";
            DataDescription = "Тип данного раздела - 'Раздел данных'. " +
                "Данный тип раздела предназначен для заполнения участниками проекта. " +
                "Данный раздел может быть заполнен одним участником многократно. ";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                FinalResults, Survey, Data
            };
        }

        public static string GetDescription(string type)
        {
            if (type == FinalResults) return FinalResultsDescription;
            else if (type == Survey) return SurveyDescription;
            else return DataDescription;
        }

    }
}
