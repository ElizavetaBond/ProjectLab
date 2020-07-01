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
                var managerId = db.Users.Find(x => x.Id == p.ManagerId).FirstOrDefault().Id;
                vm.Add (CreateProjectCard(p));
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Archive()
        {
            var projects = db.Projects.Find(x => x.ProjectType.Name != "Приватный" && x.ProjectStatus.Name == "Завершенный").ToList();
            var vm = new List<ProjectCardViewModel>();
            foreach (var p in projects)
            {
                var managerId = db.Users.Find(x => x.Id == p.ManagerId).FirstOrDefault().Id;
                vm.Add(CreateProjectCard(p));
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
                    ManagerId = User.Identity.Name,
                    ProjectType = db.ProjectTypes.Find(x => x.Id == vm.ProjectTypeId).FirstOrDefault(),
                    ProjectStatus = db.ProjectStatuses.Find(x => x.Name == "Рабочий").FirstOrDefault(),
                    Start = DateTime.Now,
                    Finish = vm.Finish,
                    Comments = new List<Comment>(),
                    ParticipantsId = new List<string> { User.Identity.Name },
                    Sections = idea.ProjectTemplate.Sections
                }) ;
                return RedirectToAction("Catalog");
            }
            loadReferences(db.Ideas.Find(x => x.Id == vm.IdeaId).FirstOrDefault().IdeaType);
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
            ViewData["ListProjectTypes"] = (IdeaType == "Открытая") ? db.ProjectTypes.Find(new BsonDocument()).ToList() :
                                           db.ProjectTypes.Find(x => x.Name == "Приватный").ToList();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddParticipant(string ProjectId, string UserId) // стать участником проекта
        {
            var update = new UpdateDefinitionBuilder<Project>().Push(x => x.ParticipantsId, UserId);
            db.Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
            return RedirectToAction("Browse", "Project", new { ProjectId = ProjectId });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Join(string ProjectId) // стать участником проекта
        {
            var update = new UpdateDefinitionBuilder<Project>().Push(x => x.ParticipantsId, User.Identity.Name);
            db.Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
            return RedirectToAction("Browse", "Project", new { ProjectId = ProjectId });
        }

        [HttpGet]
        public IActionResult Browse(string ProjectId)
        {
            var project = db.Projects.Find(x => x.Id == ProjectId).FirstOrDefault();
            var vm =  new ProjectBrowseViewModel 
            { 
                Id = project.Id,
                Name = project.Idea.Name,
                AuthorIdeaId  = project.Idea.AuthorId,
                Description = project.Idea.Description,
                Direction = project.Idea.Direction.Name,
                Equipment = project.Idea.Equipment,
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
                IsWork = project.ProjectStatus.Name == "Рабочий"
            };
            var i = 0;
            foreach (var s in project.Sections)
            {
                vm.Sections.Add(new SectionBrowseProjectViewModel
                {
                    SectionId = "areaSection" + i,
                    Name=s.Name,
                    SectionType=s.SectionType,
                    Answears = s.Answears.Select(x => new AnswearBrowseProjectViewModel
                    {
                        AuthorId = x.AuthorId,
                        Date = x.Date
                    }).ToList()
                });
                i++;
            }
            if (vm.IsManager)
            {
                var listUsers = new List<ParticipantViewModel>();
                foreach (var x in db.Users.Find(new BsonDocument()).ToList())
                {
                    if (!project.ParticipantsId.Exists(p => p == x.Id))
                        listUsers.Add(new ParticipantViewModel { Name = x.Surname + " " + x.Name, UserId = x.Id });
                }
                ViewData["ListUsers"] = listUsers;
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Fill(string ProjectId, int SectionNum) // получить форму для заполнения результатов
        {
            /*var update = new UpdateDefinitionBuilder<Project>().Push(x => x.Sections[0].Components, new Component 
            { 
                ComponentType = "Выбор", Name = "Множ. выбор", IsNecessary = false,
                ListSelect = new List<string> { "Значение1", "Значение2", "Значение3", "Значение4", "Значение5", "Значение6" }
            });
            db.Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);*/
            return View(new AnswearBrowseProjectViewModel
            {
                ProjectId = ProjectId,
                SectionNum = SectionNum,
                Components =
                db.Projects.Find(x => x.Id == ProjectId).FirstOrDefault()
                                .Sections[SectionNum].Components.Select(x => new ComponentBrowseProjectViewModel
                                {
                                    ComponentType = x.ComponentType,
                                    Description = x.Description,
                                    IsNecessary = x.IsNecessary,
                                    Name = x.Name,
                                    Value = x.ComponentType == "Текст" ? x.Description : "",
                                    ListSelect = x.ListSelect
                                }).ToList()
            });
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
                    if (Model.Components[i].ComponentType == "Файл" || Model.Components[i].ComponentType == "Фото")
                        isValid = Model.Components[i].File != null;
                    else if (Model.Components[i].ComponentType == "Флаг") 
                        isValid = true;
                    else if (Model.Components[i].ComponentType == "Множественный выбор")
                        isValid = Model.Components[i].ListRes.Contains(true);
                    else
                        isValid = !(Model.Components[i].Value == null || Model.Components[i].Value.Trim().Length == 0);
                    if (!isValid)
                        ModelState.AddModelError("Components[" + i + "]", "Заполните данное поле");
                }
            }

            if (ModelState.IsValid)
            {
                var answear = new Answear
                {
                    AuthorId = User.Identity.Name,
                    Date = DateTime.Now,
                    Components = new List<Component>()
                }; 
                foreach (var x in Model.Components)
                {
                    var value = "";
                    File file = null;
                    if (x.ComponentType == "Флаг")
                    {
                        value = x.Flag.ToString();
                    }
                    else if (x.ComponentType == "Файл" || x.ComponentType == "Фото")
                    {
                        file = x.File == null ? null :
                            new File
                            {
                                Id = db.SaveFile(x.File.OpenReadStream(), x.File.FileName),
                                Type = x.File.ContentType
                            };
                    }
                    else if (x.ComponentType == "Множественный выбор")
                    {
                        for (int i=0;  i<x.ListSelect.Count; i++)
                            if (x.ListRes[i]) 
                                value += x.ListSelect[i] + "; ";
                    }
                    else
                    {
                        value = x.Value;
                    }
                    answear.Components.Add(new Component
                    {
                        Name = x.Name,
                        ComponentType = x.ComponentType,
                        Description = x.Description,
                        IsNecessary = x.IsNecessary,
                        ListSelect = x.ListSelect,
                        Value = value,
                        File = file
                    });
                }
                var update = new UpdateDefinitionBuilder<Project>().Push(x => x.Sections[Model.SectionNum].Answears, answear);
                db.Projects.FindOneAndUpdate(x => x.Id == Model.ProjectId, update);
                return RedirectToAction("Browse", "Project", new { ProjectId = Model.ProjectId });
            }
            return View(Model);
        }
    
        [HttpGet]
        public IActionResult BrowseAnswear (string ProjectId, int SectionNum, int AnswearNum)
        {
            var answear = db.Projects.Find(x => x.Id == ProjectId).FirstOrDefault().Sections[SectionNum].Answears[AnswearNum];
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
        public IActionResult Finish (string ProjectId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Set(x => x.ProjectStatus, db.ProjectStatuses.Find(s => s.Name == "Завершенный").FirstOrDefault());
            db.Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
            return RedirectToAction("Archive");
        }

        private ProjectCardViewModel CreateProjectCard(Project project)
        {
            var managerId = db.Users.Find(x => x.Id == project.ManagerId).FirstOrDefault().Id;
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
            var vm = new ProjectMenuViewModel
            {
                Managment = db.Projects.Find(x => x.ManagerId == User.Identity.Name 
                                            && x.ProjectStatus.Name == ProjectStatusesNames.Working).ToList()
                                        .Select(x => CreateProjectCard(x)).ToList(),
                Canceled = db.Projects.Find(x => x.ManagerId == User.Identity.Name
                                            && x.ProjectStatus.Name == ProjectStatusesNames.Canceled).ToList()
                                        .Select(x => CreateProjectCard(x)).ToList(),
                Participation = new List<ProjectCardViewModel>(),
                Archive = new List<ProjectCardViewModel>()
            };
            var workProjects = db.Projects.Find(x => x.ProjectStatus.Name == ProjectStatusesNames.Working).ToList();
            foreach ( var p in workProjects)
            {
                if (p.ParticipantsId.FindIndex(x => x == User.Identity.Name) != -1 && p.ManagerId != User.Identity.Name)
                    vm.Participation.Add(CreateProjectCard(p));
            }
            var completeProjects = db.Projects.Find(x => x.ProjectStatus.Name == ProjectStatusesNames.Completed).ToList();
            foreach (var p in completeProjects)
            {
                if (p.ParticipantsId.FindIndex(x => x == User.Identity.Name) != -1)
                    vm.Archive.Add(CreateProjectCard(p));
            }
            return View(vm);
        }
    }
}
