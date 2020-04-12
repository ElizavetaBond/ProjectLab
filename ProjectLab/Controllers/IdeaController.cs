using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        }

        [HttpGet]
        public IActionResult Catalog()
        {
            var ideas = db.Ideas.Find(x => x.IdeaType == "Открытая" && x.IdeaStatus.Name == "Утверждена").ToList();
            var vm = new List<IdeaCardViewModel>();
            foreach (var x in ideas)
            {
                vm.Add(new IdeaCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Direction = x.Direction.Name,
                    Author = x.Author.Surname + " " + x.Author.Name,
                    EducationalInstitution = x.Author.EducationalInstitution.Name
                }) ;
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public IActionResult Edit(IdeaEditViewModel vm)
        {
            var author = db.Users.Find(x => x.Email == User.Identity.Name).FirstOrDefault();
            var idea = new Idea
            {
                Name = vm.Name,
                IdeaType = vm.IdeaType, 
                Target = vm.Target,
                Purpose = vm.Purpose,
                Description = vm.Description,
                Equipment = vm.Equipment,
                Safety = vm.Safety,
                Author = author,
                IdeaStatus = db.IdeaStatuses.Find(x=>x.Name=="Черновик").FirstOrDefault(),
                Direction = db.Directions.Find(x => x.Id == vm.DirectionId).FirstOrDefault(),
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
            return RedirectToAction("IdeaMenu", "Account");
        }

        [HttpGet]
        public IActionResult Browse(string IdeaId)
        {
            var idea = db.Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
            return View (new IdeaBrowseViewModel {
                Name = idea.Name,
                IdeaType = idea.IdeaType,
                Target = idea.Target,
                Purpose = idea.Purpose,
                Safety = idea.Safety,
                Equipment = idea.Equipment,
                Description = idea.Description,
                Direction = idea.Direction.Name,
                Author = idea.Author.Surname + " " + idea.Author.Name,
                Sections = idea.ProjectTemplate.Sections.Select(i => new SectionBrowseViewModel
                {
                    Name = i.Name,
                    SectionType = i.SectionType,
                    Components = i.Components.Select(c => new ComponentBrowseViewModel
                    {
                        Name = c.Name,
                        Type = c.ComponentType,
                        Description = c.Description
                    }).ToList()
                }).ToList()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(string IdeaId)
        {
            db.Ideas.DeleteOne(x => x.Id == IdeaId);
            return RedirectToAction("IdeaMenu", "Account");
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendToReview(string IdeaId)
        {
            return RedirectToAction("IdeaMenu", "Account");
        }
    }
}