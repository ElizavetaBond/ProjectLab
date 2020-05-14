﻿using System;
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
                vm.Add(GetIdeaCardVM(x));
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit()
        {
            var filter = new BsonDocument();
            ViewData["ListDirections"] = db.Directions.Find(filter).ToList();
            ViewData["ListComponents"] = new List<string> { "Текст", "Сообщение", "Дата", "Время",  "Число", "Флаг",
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
            if (ModelState.IsValid)
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
                    AuthorId = User.Identity.Name,
                    IdeaStatus = db.IdeaStatuses.Find(x => x.Name == "Черновик").FirstOrDefault(),
                    Direction = db.Directions.Find(x => x.Id == vm.DirectionId).FirstOrDefault(),
                    Video = vm.Video,
                    ImageId = vm.FileImage == null ? null : db.LoadImage(vm.FileImage.OpenReadStream(), vm.FileImage.FileName),
                    Resolutions = new List<Resolution>(),
                    Comments = new List<Comment>(),
                    ProjectTemplate = new ProjectTemplate { Sections = new List<Section>() }
                };
                for (var i = 0; i < vm.Sections.Count; i++) // перебираем все разделы
                {
                    if (!vm.Sections[i].IsDelete) // если раздел не удален
                    {
                        var section = new Section
                        {
                            Name = vm.Sections[i].Name,
                            SectionType = vm.Sections[i].SectionType,
                            Answears = new List<Answear>(),
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
                return RedirectToAction("Menu");
            }
            else
                return View(vm);
        }

        [HttpGet]
        public IActionResult Browse(string IdeaId) // вывод информации об идее
        {
            var vm = GetIdeaBrowseVM(IdeaId);
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(string IdeaId)
        {
            var idea = db.Ideas.FindOneAndDelete(x => x.Id == IdeaId);
            if (idea.ImageId != null) db.DeleteImage(idea.ImageId);
            return RedirectToAction("Menu");
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendToReview(string IdeaId) // отправить идею на проверку
        {
            var idea = db.Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
            if (idea.IdeaType == "Приватная") ; // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
            else
            {
                var experts = db.Experts.Find(x => x.Direction.Id == idea.Direction.Id) // выбрали экспертов по направленности
                                      .ToList()
                                      .OrderBy(x => x.ReviewIdeas.Count)
                                      .ToList(); // отсортировали по количеству работы
                if (experts.Count < 3) ;  // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var upd = new UpdateDefinitionBuilder<Expert>().Push(exp => exp.ReviewIdeas, idea);
                        db.Experts.FindOneAndUpdate(exp => exp.Id == experts[i].Id, upd);
                    }
                }
            }

            var update = new UpdateDefinitionBuilder<Idea>().Set(i => i.IdeaStatus, db.IdeaStatuses.Find(x => x.Name == "На модерации").FirstOrDefault());
            db.Ideas.FindOneAndUpdate(i => i.Id == IdeaId, update);
            return RedirectToAction("Menu");
        }

        [HttpGet]
        [Authorize(Roles = "Эксперт, Админ")]
        public IActionResult Review(string IdeaId) // получить вид резолюции
        {
            ViewData["idea"] = GetIdeaBrowseVM(IdeaId); // отдаем вид идеи для ознакомления с ней
            return View(new ResolutionViewModel { IdeaId = IdeaId });
        }

        [HttpPost]
        [Authorize(Roles = "Эксперт")]
        public IActionResult Review(ResolutionViewModel vm) // зафиксировть резолюцию эксперта
        {
            if (ModelState.IsValid)
            {
                var ExpertId = db.Experts.Find(x => x.UserId == User.Identity.Name).FirstOrDefault().Id;
                var resol = new Resolution // резолюция эксперта
                {
                    ExpertId = ExpertId,
                    IsPositive = vm.IsPositive,
                    ValueDegree = vm.IsPositive ? vm.ValueDegree : 0,
                    Remark = vm.Remark
                };

                var updateExp = new UpdateDefinitionBuilder<Expert>().PullFilter(exp => exp.ReviewIdeas, x => x.Id == vm.IdeaId); // удаляем идею у эксперта
                db.Experts.FindOneAndUpdate(x => x.Id == ExpertId, updateExp);

                var updateIdea = new UpdateDefinitionBuilder<Idea>().Push(idea => idea.Resolutions, resol); // добавляем резолюцию в список
                db.Ideas.FindOneAndUpdate(idea => idea.Id == vm.IdeaId, updateIdea);

                var resolutions = db.Ideas.Find(x => x.Id == vm.IdeaId).FirstOrDefault().Resolutions;
                if (resolutions.Count == 3) // если все эксперты оценили, то меняем статус
                {
                    int res = 0, degree = 0;
                    foreach (var x in resolutions)
                    {
                        degree += x.ValueDegree;
                        res += (x.IsPositive ? 1 : (-1));
                    }
                    var status = new IdeaStatus();
                    if (res > 0) status = db.IdeaStatuses.Find(x => x.Name == "Утверждена").FirstOrDefault();
                    else status = db.IdeaStatuses.Find(x => x.Name == "Отклонена").FirstOrDefault();
                    var update = new UpdateDefinitionBuilder<Idea>().Set(idea => idea.ValueDegree, (int)(degree / 3))
                                                                    .Set(idea => idea.IdeaStatus, status);
                    db.Ideas.FindOneAndUpdate(idea => idea.Id == vm.IdeaId, update);
                }
                return RedirectToAction("Menu");
            }
            else
            {
                ViewData["idea"] = GetIdeaBrowseVM(vm.IdeaId);
                return View(vm);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Эксперт, Админ")]
        public IActionResult CancelReview(string IdeaId) // эксперт отказался от выдачи рецензии
        {
            return RedirectToAction("Menu");
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

        public IdeaBrowseViewModel GetIdeaBrowseVM (string IdeaId)
        {
            var idea = db.Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
            var authorId = db.Users.Find(x => x.Id == idea.AuthorId).FirstOrDefault().Id;
            var vm = new IdeaBrowseViewModel
            {
                Name = idea.Name,
                IdeaType = idea.IdeaType,
                IdeaStatus = idea.IdeaStatus.Name,
                Target = idea.Target,
                Purpose = idea.Purpose,
                Safety = idea.Safety,
                Equipment = idea.Equipment,
                Description = idea.Description,
                Direction = idea.Direction.Name,
                AuthorId = authorId,
                ImageId = idea.ImageId,
                Video = idea.Video,
                Sections = new List<SectionBrowseViewModel>()
            };
            if (authorId == User.Identity.Name || (User.IsInRole("Эксперт") && idea.IdeaStatus.Name == "На модерации"))
            {
                vm.Sections = idea.ProjectTemplate.Sections.Select(i => new SectionBrowseViewModel
                {
                    Name = i.Name,
                    SectionType = i.SectionType,
                    Components = i.Components.Select(c => new ComponentBrowseViewModel
                    {
                        Name = c.Name,
                        Type = c.ComponentType,
                        Description = c.Description
                    }).ToList()
                }).ToList();
            }
            return vm;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Menu()
        {
            var ownideas = db.Ideas.Find(x => x.AuthorId == User.Identity.Name).ToList();
            var vm = new IdeaMenuViewModel
            {
                Drafts      = ownideas.FindAll(x => x.IdeaStatus.Name == "Черновик").ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                OnReviews   = ownideas.FindAll(x => x.IdeaStatus.Name == "На модерации").ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                Approves    = ownideas.FindAll(x => x.IdeaStatus.Name == "Утверждена").ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                Rejects     = ownideas.FindAll(x => x.IdeaStatus.Name == "Отклонена").ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                MyReviews = new List<IdeaCardViewModel>()
            };
            if (User.IsInRole("Эксперт"))
            {
                vm.MyReviews = db.Experts.Find(x => x.UserId == User.Identity.Name)
                                        .FirstOrDefault()
                                        .ReviewIdeas
                                        .Select(x => GetIdeaCardVM(x)).ToList();
            }
            return View(vm);
        }

        public IdeaCardViewModel GetIdeaCardVM (Idea idea)
        {
            return new IdeaCardViewModel
            {
                Id = idea.Id,
                Name = idea.Name,
                Direction = idea.Direction.Name,
                AuthorId = idea.AuthorId,
                ImageId = idea.ImageId
            };
        }
    }
}