using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.Models.References;
using ProjectLab.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectLab.StaticNames;
using System.Threading.Tasks;
using ProjectLab.Models.Statistics;

namespace ProjectLab.Controllers
{
    [Authorize(Roles = "Админ")]
    public class AdminController: Controller
    {
        private readonly AdminService db;

        public AdminController(AdminService context)
        {
            db = context;
            //LoadReferences();
        }

        [HttpGet]
        public IActionResult Statistics()
        {
            ViewData["ListComparedCategories"] = new List<string> { ComparedCategoriesNames.EducationalInstitutions,
                                                                    ComparedCategoriesNames.Directions,
                                                                    ComparedCategoriesNames.UserCategories };
            ViewData["ListMeasuredQuantities"] = new List<string> { MeasuredQuantitiesNames.ApprovedIdeas,
                                                                  MeasuredQuantitiesNames.CreatedProjects,
                                                                  MeasuredQuantitiesNames.ParticipantsInProjects,
                                                                  MeasuredQuantitiesNames.ArchieveProjects,
                                                                  MeasuredQuantitiesNames.RegisteredUsers };
            ViewData["ListEducationalInstitutions"] = db.EducationalInstitutions.Find(new BsonDocument()).ToList();
            ViewData["ListDirections"] = db.Directions.Find(new BsonDocument()).ToList();
            ViewData["ListUserCategories"] = db.UserCategories.Find(new BsonDocument()).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult LoadChart(StatisticsSettingsViewModel vm)
        {
            var chart = new ChartViewModel();
            var settings = new StatisticsSettings(db, vm.ComparedCategory, vm.MeasuredQuantity, vm.Begin, vm.End,
                                                  vm.DirectionsId, vm.EducationalInstitutionsId, vm.UserCategoriesId);

            if (settings.ComparedCategory == ComparedCategoriesNames.EducationalInstitutions)
            {
                var statistics = new StatisticsEducationalInstitutions(db, settings);
                statistics.Generate();
                chart = new ChartViewModel
                {
                    Title = statistics.Title,
                    ComparedCategory = statistics.ComparedCategory,
                    MeasuredQuantity = statistics.MeasuredQuantity,
                    KeyValues = statistics.KeyValues.Select(x => new ChartViewModel.KeyValueViewModel
                    {
                        Key = x.Key,
                        Value = x.Value
                    }).ToList()
                };
            }
            else if (settings.ComparedCategory == ComparedCategoriesNames.Directions)
            {
                var statistics = new StatisticsDirections(db, settings);
                statistics.Generate();
                chart = new ChartViewModel
                {
                    Title = statistics.Title,
                    ComparedCategory = statistics.ComparedCategory,
                    MeasuredQuantity = statistics.MeasuredQuantity,
                    KeyValues = statistics.KeyValues.Select(x => new ChartViewModel.KeyValueViewModel
                    {
                        Key = x.Key,
                        Value = x.Value
                    }).ToList()
                };
            }
            else if (settings.ComparedCategory == ComparedCategoriesNames.UserCategories)
            {
                var statistics = new StatisticsUserCategories(db, settings);
                statistics.Generate();
                chart = new ChartViewModel
                {
                    Title = statistics.Title,
                    ComparedCategory = statistics.ComparedCategory,
                    MeasuredQuantity = statistics.MeasuredQuantity,
                    KeyValues = statistics.KeyValues.Select(x => new ChartViewModel.KeyValueViewModel
                    {
                        Key = x.Key,
                        Value = x.Value
                    }).ToList()
                };
            }
            return PartialView("Chart", chart);
        }

        [HttpGet]
        public IActionResult Experts()
        {
            var vm = new List<ExpertsViewModel>();
            var directions = db.GetDirections();
            for (int i = 0; i < directions.Count; i++)
            {
                var users = db.GetUsersForDirection(directions[i].Id);
                vm.Add(new ExpertsViewModel
                {
                    SectionId = "section" + i,
                    DirectionName = directions[i].Name,
                    ExpertsId = users.FindAll(x => x.UserStatus.Name == UserStatusesNames.Expert).Select(x => x.Id).ToList(),
                    Users = users.FindAll(x => x.UserStatus.Name == UserStatusesNames.Participant).Select(x => new UserViewModel
                    {
                        UserId = x.Id,
                        UserName = x.Surname + " " + x.Name + " " + x.Patronymic
                    }).ToList()
                });
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult SetExpert(string UserId)
        {
            db.SetExpert(UserId);
            return RedirectToAction("Experts");
        }

        [HttpPost]
        public IActionResult CancelExpert(string ExpertId)
        {
            db.CancelExpert(ExpertId);
            return RedirectToAction("Experts");
        }

        public void LoadReferences()
        {
            /*db.Subjects.InsertOne(new Subject { Name = "Астраханская область" });
            db.Subjects.InsertOne(new Subject { Name = "Республика Адыгея" });
            db.Subjects.InsertOne(new Subject { Name = "Республика Алтай" });
            db.Subjects.InsertOne(new Subject { Name = "Московская область" });
            var sbj = db.Subjects.Find(x => x.Name == "Астраханская область").FirstOrDefault();
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "город Астрахань" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "город Знаменск" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Ахтубинский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Володарский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Икрянинский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Енотаевский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Камызякский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Красноярский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Лиманский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Наримановский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Приволжский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Харабалинский район" });
            db.Municipalities.InsertOne(new Municipality { Subject = sbj, Name = "Черноярский район" });
            var mun = db.Municipalities.Find(x => x.Name == "город Астрахань").FirstOrDefault();
            var update = new UpdateDefinitionBuilder<EducationalInstitution>().Set(x => x.Municipality, mun);
            db.EducationalInstitutions.UpdateMany(x => x.Name != "", update);
            var mun = db.Municipalities.Find(x => x.Name == "город Астрахань").FirstOrDefault();
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №32" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №74" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №51" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №12" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №4" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Сош №56" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Гимназия №2" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "Гимназия №1" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "АГТУ" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "АГУ" });
            db.EducationalInstitutions.InsertOne(new EducationalInstitution { Municipality = mun, Name = "АГМУ" });*/
        }
    }
}
