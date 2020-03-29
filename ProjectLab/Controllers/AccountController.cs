using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Find(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefaultAsync();
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Find(u => u.Email == model.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    var newUser = new User 
                    { 
                        Email = model.Email, 
                        Password = model.Password, 
                        UserStatus = db.UserStatuses.Find(x => x.Name == "Участник сообщества").FirstOrDefault() 
                    };
                    db.Users.InsertOne(newUser);

                    await Authenticate(newUser); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserStatus.Name)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "Участник сообщества, Эксперт")]
        public IActionResult IdeaMenu()
        {
            var ownideas = db.Ideas.Find(x => x.Author == User.Identity.Name).ToList();
            return View(new IdeasOwnViewModel
            {
                Drafts = ownideas.FindAll(x => x.IdeaStatus.Name == "Черновик").ToList()
                .Select(x => new IdeaCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    Author = x.Author,
                    EducationalInstitution = db.Users.Find(u => u.Email == x.Author).FirstOrDefault().EducationalInstitution.Name
                }).ToList(),
                Reviews = ownideas.FindAll(x=>x.IdeaStatus.Name == "На модерации").ToList()
                .Select(x => new IdeaCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    Author = x.Author,
                    EducationalInstitution = db.Users.Find(u => u.Email == x.Author).FirstOrDefault().EducationalInstitution.Name
                }).ToList(),
                Approves = ownideas.FindAll(x => x.IdeaStatus.Name == "Утверждена").ToList()
                .Select(x => new IdeaCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    Author = x.Author,
                    EducationalInstitution = db.Users.Find(u => u.Email == x.Author).FirstOrDefault().EducationalInstitution.Name
                }).ToList(),
                Rejects= ownideas.FindAll(x => x.IdeaStatus.Name == "Отклонена").ToList()
                .Select(x => new IdeaCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    Author = x.Author,
                    EducationalInstitution = db.Users.Find(u => u.Email == x.Author).FirstOrDefault().EducationalInstitution.Name
                }).ToList()
            });
        }
    }
}
