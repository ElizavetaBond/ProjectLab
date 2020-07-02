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
        private readonly AccountService db;

        public AccountLinkViewComponent(AccountService context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string UserId)
        {
            var user = db.GetUser(UserId);
            return View(new AccountLinkViewModel
            {
                UserId = user.Id,
                Image = user.Photo,
                Name = user.Name,
                Surname = user.Surname
            });
        }
    }
}
