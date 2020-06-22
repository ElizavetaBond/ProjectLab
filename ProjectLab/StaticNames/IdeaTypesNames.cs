using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class IdeaTypesNames
    {
        public static readonly string Open;

        public static readonly string Private;

        static IdeaTypesNames()
        {
            Open = "Открытая";
            Private = "Приватная";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                Open, Private
            };
        }
    }
}
