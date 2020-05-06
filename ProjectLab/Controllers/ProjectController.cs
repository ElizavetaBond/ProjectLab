using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.Models.References;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Project;
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
            /*var projects = db.Projects.Find( x => x.ProjectType.Name != "Приватный" && x.ProjectStatus.Name == "Рабочий").ToList();
            var vm = new List<ProjectViewModel>();
            foreach (var p in projects)
            {
                vm.Add(new ProjectViewModel
                {
                    Name = p.Name,
                    Manager = (p.Manager == null) ? "" : p.Manager.Name
                });
            }*/
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(string IdeaId)
        {
            var idea = db.Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
            var vm = new ProjectCreateViewModel { IdeaId = IdeaId, IdeaName = idea.Name };
            ViewData["ListProjectTypes"] = db.ProjectTypes.Find(new BsonDocument()).ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(ProjectCatalogViewModel vm)
        {
            /*var project = new Project
            {
                Name = vm.Name,
                Start = vm.Start,
                Finish = vm.Finish,
                Description = vm.Description
            };
            if (vm.Id == null)
                db.Projects.InsertOne(project);
            else
                db.Projects.ReplaceOne(new BsonDocument("Id", vm.Id), project);*/
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
