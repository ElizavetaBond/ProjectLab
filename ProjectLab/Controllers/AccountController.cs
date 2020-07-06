using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.Models.References;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectLab.Controllers
{
    public class AccountController: Controller
    {
        private readonly AccountService db;

        public AccountController(AccountService context)
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
                User user = db.GetUser(model.Email, model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            else Console.WriteLine("===========================" + ModelState.Values);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["ListUserCategories"] = db.GetUserCategories();
            ViewData["ListEducationalInstitutions"] = db.GetEducationalInstitutions().OrderBy(x => x.Name);
            ViewData["ListEducations"] = db.GetEducations();
            ViewData["ListDirections"] = db.GetDirections();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = db.GetUserByEmail(model.Email);
                if (user == null)
                {
                    string photoType = "", photoName = "";
                    Stream photoStream = null;
                    if (model.Photo != null)
                    {
                        photoStream = model.Photo.OpenReadStream();
                        photoType = model.Photo.ContentType;
                        photoName = model.Photo.Name;
                    }
                    db.CreateUser(model.Email, model.Password, model.Surname, model.Name, model.Patronymic,
                        model.BirthDate, model.UserCategoryId, model.EducationalInstitutionId, model.EducationalInstitutionId,
                        model.AddInform, model.Contacts, model.DirectionId, photoStream, photoType, photoName);

                    await Authenticate(db.GetUserByEmail(model.Email)); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("Email", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserStatus.Name)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("ApplicationCookie");
            return RedirectToAction("Login", "Account");
        }        

        [HttpGet]
        public IActionResult Account()
        {
            return View();
        }

        public ActionResult GetFile(string id, string type)
        {
            var image = db.GetFile(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, type);
        }
    }
}
