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
using System.Threading.Tasks;

namespace ProjectLab.Controllers
{
    [Authorize(Roles = "Админ")]
    public class AdminController: Controller
    {
        private readonly ProjectLabDbService db;

        public AdminController(ProjectLabDbService context)
        {
            db = context;
            //LoadReferences();
        }

        [HttpGet]
        public IActionResult Statistics(int num)
        {
            switch(num)
            {
                case 1:
                    ViewData["Name"] = "учебных заведений";
                    ViewData["ListDirections"] = db.Directions.Find(new BsonDocument()).ToList();
                    ViewData["ListCriterions"] = new List<string>
                    {
                        "Количество утвержденных идей",
                        "Количество проектов, опубликованных обучающимися/педагогами",
                        "Количество проектов, в которых участвуют обучающиеся/педагоги",
                        "Количество активных пользователей"
                    };
                    break;
                case 2:
                    ViewData["Name"] = "Областей специализаций";
                    ViewData["ListFiltrs"] = db.EducationalInstitutions.Find(new BsonDocument()).ToList();
                    break;
                case 3:
                    ViewData["Name"] = "Пользователей";
                    break;
            }
            return View();
        }

        [HttpPost]
        public IActionResult LoadChart(StatisticsSettingsViewModel vm)
        {
            var chart = new ChartViewModel
            {
                NameX = "Учебные заведения",
                NameY = "Количество утвержденных идей",
                Title = "Учебные заведения / Количество утвержденных идей",
                KeyValues = new List<ChartViewModel.KeyValue>()
            };
            var schools = db.EducationalInstitutions.Find(new BsonDocument()).ToList();
            var ideas = db.Ideas.Find(x => x.IdeaStatus.Name == "Утверждена").ToList();
            foreach (var school in schools)
            {
                var value = 0;
                foreach (var idea in ideas)
                {
                    var user = db.Users.Find(u => u.Id == idea.AuthorId).FirstOrDefault();
                    if (user.EducationalInstitution.Id == school.Id)
                    {
                        value++;
                    }
                }
                chart.KeyValues.Add(new ChartViewModel.KeyValue { Key = school.Name, Value = value });
            }
            return PartialView("Chart", chart);
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
