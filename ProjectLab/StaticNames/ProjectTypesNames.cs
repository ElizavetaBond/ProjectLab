using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class ProjectTypesNames
    {
        public static readonly string Opened;

        public static readonly string Closed;

        public static readonly string Private;

        static ProjectTypesNames()
        {
            Opened = "Открытый";
            Closed = "Закрытый";
            Private = "Приватный";
        }

        public static List<string> Get()
        {
            return new List<string> { Opened, Closed, Private };
        }
    }
}
