using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers
{
    public class ProjectController: Controller
    {
        private readonly ProjectLabDbService db;

        public ProjectController(ProjectLabDbService context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var filter = new BsonDocument();
            var projects = db.Projects.Find(filter).ToList();
            var vm = new List<ProjectViewModel>();
            foreach (var p in projects)
            {
                vm.Add(new ProjectViewModel
                {
                    Name = p.Name,
                    Manager = (p.Manager == null) ? "" : p.Manager.Name
                });
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(string IdeaId)
        {
            var vm = new ProjectViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(ProjectViewModel vm)
        {
            var project = new Project
            {
                Name = vm.Name,
                Start = vm.Start,
                Finish = vm.Finish,
                Description = vm.Description
            };
            if (vm.Id == null)
                db.Projects.InsertOne(project);
            else
                db.Projects.ReplaceOne(new BsonDocument("Id", vm.Id), project);
            return RedirectToAction("Index");
        }

        public ActionResult GetImage(string id)
        {
            var image = db.GetImage(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, "image/jpg");
        }
    }
}
