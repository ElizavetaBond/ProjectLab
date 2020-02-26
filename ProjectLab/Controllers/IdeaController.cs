using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Idea;

namespace ProjectLab.Controllers
{
    public class IdeaController : Controller
    {
        private readonly ProjectLabDbService db;

        public IdeaController(ProjectLabDbService context)
        {
            db = context;
            Session.User = db.Users.Find(x => x.Login=="liza").FirstOrDefault();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var ideas = db.Ideas.Find(x => x.IdeaStatus.Name == "Утверждена" && x.IdeaType == "Открытая").ToList();
            var vm = new List<IdeaCatalogViewModel>();
            foreach (var x in ideas)
            {
                vm.Add(new IdeaCatalogViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    //Author = x.Author.Login,
                    //EducationalInstitution = x.Author.EducationalInstitution.Name
                });
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var filter = new BsonDocument();
            ViewData["ListDirections"] = db.Directions.Find(filter).ToList();
            ViewData["ListComponents"] = new List<string> { "Текст", "Сообщение", "Дата/Время",  "Число", "Флаг",
                                                            "Файл", "Фото", "Место", "Выбор", "Гиперссылка"};
            ViewData["ListIdeaTypes"] = new List<string> { "Открытая", "Приватная" };
            ViewData["ListSectionTypes"] = new List<string> { "Раздел итоговых результатов", 
                                                              "Раздел опроса", "Раздел данных" };
            var vm = new IdeaEditViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(IdeaEditViewModel vm)
        {
            var idea = new Idea
            {
                Name = vm.Name,
                IdeaType = vm.IdeaType, 
                Target = vm.Target,
                Purpose = vm.Purpose,
                Description = vm.Description,
                Equipment = vm.Equipment,
                Safety = vm.Safety,
                Author = Session.User,
                IdeaStatus = db.IdeaStatuses.Find(x=>x.Name=="Утверждена").FirstOrDefault(),
                Direction = db.Directions.Find(x => x.Id == vm.IdDirection).FirstOrDefault(),
                ProjectTemplate = new ProjectTemplate { Sections=new List<Section>()}
            };
            for (var i=0; i<vm.Sections.Count; i++) // перебираем все разделы
            {
                if (!vm.Sections[i].IsDelete) // если раздел не удален
                {
                    var section = new Section
                    {
                        Name = vm.Sections[i].Name,
                        SectionType = vm.Sections[i].SectionType,
                        Components = vm.Components.FindAll(x => x.Section == i && !x.IsDelete)
                                              .Select(x => new Component
                                              {
                                                  Name = x.Name,
                                                  ComponentType = x.Type,
                                                  Description = x.Description,
                                                  IsNecessary = x.IsNecessary
                                              }).ToList()
                    };
                    idea.ProjectTemplate.Sections.Add(section);
                }
            }
            db.Ideas.InsertOne(idea);
            return RedirectToAction("Index");
        }
    }
}