using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers
{
    public class AccountController: Controller
    {
        private readonly ProjectLabDbService db;

        public AccountController(ProjectLabDbService context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult IdeaMenu()
        {
            return View(new IdeasOwnViewModel
            {
                Drafts = db.Ideas.Find(x => x.IdeaStatus.Name == "Черновик")
                .ToList()
                .Select(x => new IdeaCardViewModel
                {
                    Id = x.Id,
                    //Author = x.Author.Name,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    //EducationalInstitution = x.Author.EducationalInstitution.Name
                })
                .ToList()
            });
        }
    }
}
