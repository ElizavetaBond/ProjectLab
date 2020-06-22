using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public class UserStatusesNames
    {
        public static readonly string Participant;

        public static readonly string Expert;

        public static readonly string Admin;

        static UserStatusesNames()
        {
            Participant = "Участник сообщества";
            Expert = "Эксперт";
            Admin = "Админ";
        }
    }
}
