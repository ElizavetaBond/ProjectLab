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
            /*var id1 = db.Users.Find(x => x.Email == "exp1@mail.ru").FirstOrDefault().Id;
            SetExpert(id1);
            var id2 = db.Users.Find(x => x.Email == "exp2@mail.ru").FirstOrDefault().Id;
            SetExpert(id2);
            var id3 = db.Users.Find(x => x.Email == "exp3@mail.ru").FirstOrDefault().Id;
            SetExpert(id3);*/
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
            else Console.WriteLine("===========================" + ModelState.Values);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var filter = new BsonDocument();
            ViewData["ListUserCategories"] = db.UserCategories.Find(filter).ToList();
            ViewData["ListEducationalInstitutions"] = db.EducationalInstitutions.Find(filter).ToList();
            ViewData["ListEducations"] = db.Educations.Find(filter).ToList();
            ViewData["ListDirections"] = db.Directions.Find(filter).ToList();
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
                        Surname = model.Surname,
                        Name = model.Name,
                        Patronymic = model.Patronymic,
                        BirthDate = model.BirthDate,
                        UserStatus = db.UserStatuses.Find(x => x.Name == "Участник сообщества").FirstOrDefault(),
                        UserCategory = db.UserCategories.Find(x => x.Id == model.UserCategoryId).FirstOrDefault(),
                        EducationalInstitution = db.EducationalInstitutions.Find(x => x.Id == model.EducationalInstitutionId).FirstOrDefault(),
                        Education = db.Educations.Find(x => x.Id == model.EducationId).FirstOrDefault(),
                        AddInform = model.AddInform,
                        Contacts = model.Contacts,
                        Rewards = new List<Reward>(),
                        Direction = db.Directions.Find(x => x.Id == model.DirectionId).FirstOrDefault(),
                        Photo = model.Photo == null ? null :
                            new File
                            {
                                Id = db.SaveFile(model.Photo.OpenReadStream(), model.Photo.FileName),
                                Type = model.Photo.ContentType
                            }
                    };
                    db.Users.InsertOne(newUser);

                    await Authenticate(db.Users.Find(x=>x.Email==newUser.Email).FirstOrDefault()); // аутентификация

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

        public void SetExpert(string UserId)
        {
            var update = new UpdateDefinitionBuilder<User>().Set(us => us.UserStatus, db.UserStatuses.Find(x => x.Name == "Эксперт").FirstOrDefault());
            db.Users.FindOneAndUpdate(us => us.Id == UserId, update);

            var user = db.Users.Find(x => x.Id == UserId).FirstOrDefault();
            db.Experts.InsertOne(new Expert { UserId = user.Id, Direction=user.Direction, ReviewIdeas = new List<Idea>() });
        }

        public void SetAdmin()
        {
            var update = new UpdateDefinitionBuilder<User>().Set(us => us.UserStatus, db.UserStatuses.Find(x => x.Name == "Админ").FirstOrDefault());
            db.Users.FindOneAndUpdate(us => us.Email == "admin@ya.ru", update);
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
