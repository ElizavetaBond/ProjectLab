using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab
{
    public class ApplicationUser : IUser
    {
        public ApplicationUser(string name)
        {
            Id = Guid.NewGuid().ToString();
            UserName = name;
        }

        public string Id { get; private set; }
        public string UserName { get; set; }
    }
}
