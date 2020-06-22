using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class ExpertDecisionsNames
    {
        public static readonly string Approve;

        public static readonly string Revision;

        public static readonly string Reject;

        static ExpertDecisionsNames()
        {
            Approve = "Утвердить";
            Revision = "Отправить на доработку";
            Reject = "Отклонить";
        }
    }
}
