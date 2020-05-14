using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers.ViewComponents
{
    public class AccountLinkViewComponent: ViewComponent
    {
        private readonly ProjectLabDbService db;

        public AccountLinkViewComponent(ProjectLabDbService context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string UserId)
        {
            var user = db.Users.Find(x => x.Id == UserId).FirstOrDefault();
            return View(new AccountLinkViewModel
            {
                UserId = user.Id,
                ImageId = user.ImageId,
                Name = user.Name,
                Surname = user.Surname
            });
        }
    }
}
