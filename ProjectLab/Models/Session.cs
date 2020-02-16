using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public static class Session
    {
        public static bool isLogin { get; set; }

        public static string Login { get; set; }

        static Session()
        {
            isLogin = true;
            Login = "liza";
        }
    }
}
