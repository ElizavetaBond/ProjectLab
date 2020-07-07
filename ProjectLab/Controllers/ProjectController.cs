using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
using ProjectLab.StaticNames;
using ProjectLab.ViewModels.Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectLab.Controllers
{
    public class ProjectController: Controller
    {
        private readonly ProjectService db;

        public ProjectController(ProjectService context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Catalog()
        {
            var projects = db.GetWorkAvailableProjects();
            var vm = new List<ProjectCardViewModel>();
            foreach (var p in projects)
                vm.Add (CreateProjectCard(p));
            return View(vm);
        }

        [HttpGet]
        public IActionResult Archive()
        {
            var projects = db.GetCompetedkAvailableProjects();
            var vm = new List<ProjectCardViewModel>();
            foreach (var p in projects)
                vm.Add(CreateProjectCard(p));
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(string IdeaId)
        {
            var idea = db.GetIdea(IdeaId);
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
                db.CreateProject(vm.IdeaId, User.Identity.Name, vm.ProjectTypeId, vm.Finish);
                return RedirectToAction("Catalog");
            }
            loadReferences(db.GetIdea(vm.IdeaId).IdeaType);
            return View(vm);
        }

        public ActionResult GetFile(string id, string type)
        {
            var file = db.GetFile(id);
            if (file == null)
            {
                return NotFound();
            }
            return File(file, type);
        }

        private void loadReferences (string IdeaType)
        {
            ViewData["ListProjectTypes"] = (IdeaType == IdeaTypesNames.Open) ? db.GetProjectTypes() :
                                           db.GetProjectTypes().FindAll(x => x.Name == ProjectTypesNames.Private);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddParticipant(string ProjectId, string UserId) // стать участником проекта
        {
            db.AddParticipant(ProjectId, UserId);
            return RedirectToAction("Browse", "Project", new { ProjectId = ProjectId });
        }


        [HttpGet]
        public IActionResult Browse(string ProjectId)
        {
            var project = db.GetProject(ProjectId);
            var vm =  new ProjectBrowseViewModel 
            { 
                Id = project.Id,
                Name = project.Idea.Name,
                AuthorIdeaId  = project.Idea.AuthorId,
                Description = project.Idea.Description,
                Direction = project.Idea.Direction.Name,
                Equipment = project.Idea.Equipment,
                IdeaType = project.Idea.IdeaType,
                Finish = project.Finish,
                Image = project.Idea.Image,
                ManagerId = project.ManagerId,
                ProjectType = project.ProjectType.Name,
                Purpose = project.Idea.Purpose,
                Safety = project.Idea.Safety,
                Start = project.Start,
                Target = project.Idea.Target,
                Video = project.Idea.Video,
                ParticipantsId = project.ParticipantsId,
                Sections = new List<SectionBrowseProjectViewModel>(),
                IsParticipant = project.ParticipantsId.Find(x => x == User.Identity.Name) != null,
                IsManager = project.ManagerId == User.Identity.Name,
                IsWork = project.ProjectStatus.Name == ProjectStatusesNames.Working,
                IsFinish = true
            };
            var i = 0;
            foreach (var s in project.Sections)
            {
                var sectionVm = new SectionBrowseProjectViewModel
                {
                    SectionId = "areaSection" + i,
                    Name = s.Name,
                    SectionType = s.SectionType,
                    Description = SectionTypesNames.GetDescription(s.SectionType),
                    isFill = false,
                    Answears = s.Answears.Select(x => new AnswearBrowseProjectViewModel
                    {
                        AuthorId = x.AuthorId,
                        Date = x.Date
                    }).ToList()
                } ;
                if (vm.IsParticipant)
                {
                    if (s.SectionType == SectionTypesNames.FinalResults)
                    {
                        if (vm.IsManager && !s.Answears.Any()) sectionVm.isFill = true;
                        else sectionVm.isFill = false;
                    }
                    else if (s.SectionType == SectionTypesNames.Survey)
                    {
                        if (s.Answears.FindIndex(x => x.AuthorId == User.Identity.Name) == -1) sectionVm.isFill = true;
                        else sectionVm.isFill = false;
                    }
                    else sectionVm.isFill = true;
                }
                i++;
                vm.Sections.Add(sectionVm);
            }
            if (vm.IsManager)
            {
                var listUsers = new List<ParticipantViewModel>();
                foreach (var x in db.GetUsers())
                {
                    if (!project.ParticipantsId.Exists(p => p == x.Id))
                        listUsers.Add(new ParticipantViewModel { Name = x.Surname + " " + x.Name, UserId = x.Id });
                }
                ViewData["ListUsers"] = listUsers;
                ViewData["ListProjectTypes"] = db.GetProjectTypes();
            }
            var finalresult = project.Sections.FindAll(x => x.SectionType == SectionTypesNames.FinalResults);
            foreach (var f in finalresult)
            {
                if (!f.Answears.Any())
                {
                    vm.IsFinish = false;
                    break;
                }
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Fill(string ProjectId, int SectionNum) // получить форму для заполнения результатов
        {
            var vm =new AnswearBrowseProjectViewModel
            {
                ProjectId = ProjectId,
                SectionNum = SectionNum,
                Components =
                db.GetProject(ProjectId)
                                .Sections[SectionNum].Components.Select(x => new ComponentBrowseProjectViewModel
                                {
                                    ComponentType = x.ComponentType,
                                    Description = x.Description,
                                    IsNecessary = x.IsNecessary,
                                    Name = x.Name,
                                    Value = x.ComponentType == "Текст" ? x.Description : "",
                                    ListSelect = x.ListSelect
                                }).ToList()
            };
            return PartialView("Fill", vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Fill(AnswearBrowseProjectViewModel Model) // заполнить раздел
        {
            for (int i = 0; i < Model.Components.Count; i++) // проверяем заполнены ли все обязательные поля
            {
                var isValid = true;
                if (Model.Components[i].IsNecessary)
                {
                    if (Model.Components[i].ComponentType == ComponentsNames.File || Model.Components[i].ComponentType == ComponentsNames.Photo)
                        isValid = Model.Components[i].File != null;
                    else if (Model.Components[i].ComponentType == ComponentsNames.Flag) 
                        isValid = true;
                    else if (Model.Components[i].ComponentType == ComponentsNames.MultipleChoice)
                        isValid = Model.Components[i].ListRes.Contains(true);
                    else
                        isValid = !(Model.Components[i].Value == null || Model.Components[i].Value.Trim().Length == 0);
                    if (!isValid)
                        ModelState.AddModelError("Components[" + i + "]", "Заполните данное поле");
                }
            }

            if (ModelState.IsValid)
            {
                db.AddAnswear(User.Identity.Name, Model.Components, Model.ProjectId, Model.SectionNum);
                return RedirectToAction("Browse", "Project", new { ProjectId = Model.ProjectId });
            }
            return View(Model);
        }
    
        [HttpGet]
        public IActionResult BrowseAnswear (string ProjectId, int SectionNum, int AnswearNum)
        {
            var answear = db.GetProject(ProjectId).Sections[SectionNum].Answears[AnswearNum];
            return View(new BrowseAnswearViewModel
            {
                AuthorId = answear.AuthorId,
                Date = answear.Date.ToString(),
                Components = answear.Components.Select(x => new BrowseAnswearComponentViewModel
                {
                    ComponentType = x.ComponentType,
                    Description = x.Description,
                    File = x.File,
                    Name = x.Name,
                    Value = x.Value
                }).ToList()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Finish (string ProjectId)
        {
            var project = db.GetProject(ProjectId);
            if (project.ManagerId == User.Identity.Name)
            {
                db.CompleteProject(ProjectId);
                return RedirectToAction("Archive");
            }
            return RedirectToAction("Browse", new { ProjectId = ProjectId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Cancel (string ProjectId)
        {
            var project = db.GetProject(ProjectId);
            if (project.ManagerId == User.Identity.Name)
            {
                db.CancelProject(ProjectId);
                return RedirectToAction("Menu", "Project");
            }
            return RedirectToAction("Browse", new { ProjectId = ProjectId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult LeaveProject (string ProjectId)
        {
            db.LeaveProject(ProjectId, User.Identity.Name);
            return RedirectToAction("Catalog");
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangeType(string ProjectId, string ProjectTypeId)
        {
            var project = db.GetProject(ProjectId);
            if (project.ManagerId == User.Identity.Name)
                db.ChangeProjectType(ProjectId, ProjectTypeId);
            return RedirectToAction("Browse", new { ProjectId = ProjectId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangeDateFinish(string ProjectId, DateTime Finish)
        {
            var project = db.GetProject(ProjectId);
            if (project.ManagerId == User.Identity.Name)
                db.ChangeDateFinish(ProjectId, Finish);
            return RedirectToAction("Browse", new { ProjectId = ProjectId });
        }

        private ProjectCardViewModel CreateProjectCard(Project project)
        {
            var managerId = db.GetUser(project.ManagerId).Id;
            return new ProjectCardViewModel
            {
                Id = project.Id,
                Name = project.Idea.Name,
                Direction = project.Idea.Direction.Name,
                ManagerId = managerId,
                ProjectType = project.ProjectType.Name,
                Image = project.Idea.Image,
                IsManager = managerId == User.Identity.Name,
                IsParticipant = project.ParticipantsId.Find(x => x == User.Identity.Name) != null
            };
        }

        [HttpGet]
        [Authorize]
        public IActionResult Menu()
        {
            var managmentProjects = db.GetManagmentProjects(User.Identity.Name);
            var participantProjects = db.GetParticipantProjects(User.Identity.Name);
            var vm = new ProjectMenuViewModel
            {
                Managment = managmentProjects.FindAll(x => x.ProjectStatus.Name == ProjectStatusesNames.Working)
                                        .Select(x => CreateProjectCard(x)).ToList(),
                Canceled = participantProjects.FindAll(x => x.ProjectStatus.Name == ProjectStatusesNames.Canceled)
                                        .Select(x => CreateProjectCard(x)).ToList(),
                Participation = participantProjects.FindAll(x => x.ProjectStatus.Name == ProjectStatusesNames.Working)
                                        .Select(x => CreateProjectCard(x)).ToList(),
                Archive = participantProjects.FindAll(x => x.ProjectStatus.Name == ProjectStatusesNames.Completed)
                                        .Select(x => CreateProjectCard(x)).ToList(),
            };
            return View(vm);
        }
    }
}
