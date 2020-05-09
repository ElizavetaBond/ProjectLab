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
        public IActionResult Catalog()
        {
            var projects = db.Projects.Find( x => x.ProjectType.Name != "Приватный" && x.ProjectStatus.Name == "Рабочий").ToList();
            var vm = new List<ProjectCardViewModel>();
            foreach (var p in projects)
            {
                var manager = db.Users.Find(x => x.Email == p.ManagerEmail).FirstOrDefault();
                vm.Add (new ProjectCardViewModel
                {
                    Id = p.Id,
                    Name = p.Idea.Name,
                    Direction = p.Idea.Direction.Name,
                    EducationalInstitution = manager.EducationalInstitution.Name,
                    Manager = manager.Surname + manager.Name,
                    ProjectType = p.ProjectType.Name,
                    ImageId = p.Idea.ImageId,
                    IsManager = manager.Email == User.Identity.Name,
                    IsParticipant = p.ParticipantsEmail.Find(x => x == User.Identity.Name) != null
                });
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(string IdeaId)
        {
            var idea = db.Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
            var vm = new ProjectCreateViewModel { IdeaId = IdeaId, IdeaName = idea.Name };
            loadReferences(idea.IdeaType);
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ProjectCreateViewModel vm)
        {
            if (vm.Finish <= DateTime.Now)
                ModelState.AddModelError("Finish", "Некорректная дата");
            if (ModelState.IsValid)
            {
                var idea = db.Ideas.Find(x => x.Id == vm.IdeaId).FirstOrDefault();
                db.Projects.InsertOne(new Project
                {
                    Idea = idea,
                    ManagerEmail = User.Identity.Name,
                    ProjectType = db.ProjectTypes.Find(x => x.Id == vm.ProjectTypeId).FirstOrDefault(),
                    ProjectStatus = db.ProjectStatuses.Find(x => x.Name == "Рабочий").FirstOrDefault(),
                    Start = DateTime.Now,
                    Finish = vm.Finish,
                    Comments = new List<Comment>(),
                    ParticipantsEmail = new List<string>(),
                    Sections = idea.ProjectTemplate.Sections
                });
                return RedirectToAction("Catalog");
            }
            loadReferences(db.Ideas.Find(x => x.Id == vm.IdeaId).FirstOrDefault().IdeaType);
            return View(vm);
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

        private void loadReferences (string IdeaType)
        {
            ViewData["ListProjectTypes"] = (IdeaType == "Открытая") ? db.ProjectTypes.Find(new BsonDocument()).ToList() :
                                           db.ProjectTypes.Find(x => x.Name == "Приватный").ToList();
        }

        [Authorize]
        public ActionResult Join(string ProjectId) // стать участником проекта
        {
            var update = new UpdateDefinitionBuilder<Project>().Push(x => x.ParticipantsEmail, User.Identity.Name);
            db.Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
            return RedirectToAction("Catalog");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Browse(string ProjectId)
        {
            return View();
        }
    }
}
