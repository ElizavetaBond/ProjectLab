using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models;
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
                vm.Add (new ProjectCardViewModel
                {
                    Id = p.Id,
                    Name = p.Idea.Name,
                    Direction = p.Idea.Direction.Name,
                    ManagerId = managerId,
                    ProjectType = p.ProjectType.Name,
                    Image = p.Idea.Image,
                    IsManager = managerId == User.Identity.Name,
                    IsParticipant = p.ParticipantsId.Find(x => x == User.Identity.Name) != null
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
                    ManagerId = User.Identity.Name,
                    ProjectType = db.ProjectTypes.Find(x => x.Id == vm.ProjectTypeId).FirstOrDefault(),
                    ProjectStatus = db.ProjectStatuses.Find(x => x.Name == "Рабочий").FirstOrDefault(),
                    Start = DateTime.Now,
                    Finish = vm.Finish,
                    Comments = new List<Comment>(),
                    ParticipantsId = new List<string>(),
                    Sections = idea.ProjectTemplate.Sections
                });
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
                Sections = new List<SectionBrowseProjectViewModel>()
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
                                    Value = "",
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
                return RedirectToAction("Catalog");
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
    }
}
