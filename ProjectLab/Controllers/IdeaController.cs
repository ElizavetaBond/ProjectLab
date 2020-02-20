using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels;

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
        public IActionResult Index()
        {
            var filter = new BsonDocument();
            var ideas = db.Ideas.Find(filter).ToList();
            var vm = new List<IdeaViewModel>();
            foreach (var i in ideas)
            {
                vm.Add(new IdeaViewModel
                {
                    Name = i.Name,
                    Direction = (i.Direction == null) ? "" : i.Direction.Name,
                    Author = (i.Author==null)?"":i.Author.Name
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
            var vm = new IdeaViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(IdeaViewModel vm)
        {
            var idea = new Idea
            {
                Name = vm.Name,
                Direction = new Direction { Name = vm.Direction },
                Target = vm.Target,
                Purpose = vm.Purpose,
                Description = vm.Description,
                Equipment = vm.Equipment,
                Safety = vm.Safety
            };
            if (vm.Id == null) 
                db.Ideas.InsertOne(idea);
            else
                db.Ideas.ReplaceOne(new BsonDocument("Id", vm.Id), idea);
            return RedirectToAction("Index");
        }
    }
}