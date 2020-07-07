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
using ProjectLab.StaticNames;
using System.IO;

namespace ProjectLab.Controllers
{
    public class IdeaController : Controller
    {
        private readonly IdeaService db;

        public IdeaController(IdeaService context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Catalog()
        {
            var ideas = db.GetOpenApprovedIdeas();
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
            ViewData["ListDirections"] = db.GetDirections();
            ViewData["ListComponents"] = ComponentsNames.Get();
            ViewData["ListIdeaTypes"] = IdeaTypesNames.Get();
            ViewData["ListSectionTypes"] = SectionTypesNames.Get();
            var vm = new IdeaEditViewModel();
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(IdeaEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var sections = new List<Section>();
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
                                                      IsNecessary = x.IsNecessary,
                                                      ListSelect = x.ListSelect
                                                  }).ToList()
                        };
                        sections.Add(section);
                    }
                }
                Stream fileStream;
                string fileName, fileType;
                if (vm.FileImage == null)
                {
                    fileStream = null;
                    fileName = "";
                    fileType = "";
                }
                else
                {
                    fileStream = vm.FileImage.OpenReadStream();
                    fileName = vm.FileImage.FileName;
                    fileType = vm.FileImage.ContentType;
                }
                db.CreateIdea(vm.Name, vm.IdeaType, vm.Target, vm.Purpose, vm.Description, vm.Equipment, vm.Safety,
                    User.Identity.Name, vm.DirectionId, vm.Video, fileStream, fileName, fileType, sections);
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
            db.DeleteIdea(IdeaId);
            return RedirectToAction("Menu");
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendToReview(string IdeaId) // отправить идею на проверку
        {
            db.SendIdeaToReview(IdeaId);
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
        [Authorize(Roles = "Эксперт, Админ")]
        public IActionResult Review(ResolutionViewModel vm) // зафиксировть резолюцию эксперта
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole(UserStatusesNames.Admin))
                    db.RegistAdminResolution(User.Identity.Name, vm.IdeaId, vm.Decision, vm.Comment, vm.ValueDegree);
                else 
                    db.RegistExpertResolution(User.Identity.Name, vm.IdeaId, vm.Decision, vm.Comment, vm.ValueDegree);
                return RedirectToAction("Menu");
            }
            else
            {
                ViewData["idea"] = GetIdeaBrowseVM(vm.IdeaId);
                return View(vm);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Эксперт")]
        public IActionResult CancelReview(string IdeaId) // эксперт отказался от выдачи рецензии
        {
            return RedirectToAction("Menu");
        }

        public ActionResult GetFile(string id, string type)
        {
            var image = db.GetFile(id);
            if (image == null)
                return NotFound();
            return File(image, type);
        }

        public IdeaBrowseViewModel GetIdeaBrowseVM (string IdeaId)
        {
            var idea = db.GetIdea(IdeaId);
            var authorId = db.GetUser(idea.AuthorId).Id;
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
                Image = idea.Image,
                Video = idea.Video,
                Sections = new List<SectionBrowseViewModel>(),
                ResolutionCards = new List<ResolutionCardViewModel>()
            };
            if (authorId == User.Identity.Name)
            {
                var resolutions = db.GetResolutions(idea.Id);
                vm.ResolutionCards = resolutions.Select(x => new ResolutionCardViewModel
                {
                    Comment = x.Comment,
                    Decision = x.Decision,
                    ValueDegree = x.ValueDegree
                }).ToList();
            }
            if (authorId == User.Identity.Name || User.IsInRole(UserStatusesNames.Admin) || (User.IsInRole(UserStatusesNames.Expert) 
                        && idea.IdeaStatus.Name == IdeaStatusesNames.OnReview))
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
            var ownideas = db.GetIdeasByAuthor(User.Identity.Name);
            var vm = new IdeaMenuViewModel
            {
                Drafts      = ownideas.FindAll(x => x.IdeaStatus.Name == IdeaStatusesNames.Draft).ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                OnReviews   = ownideas.FindAll(x => x.IdeaStatus.Name == IdeaStatusesNames.OnReview).ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                Approves    = ownideas.FindAll(x => x.IdeaStatus.Name == IdeaStatusesNames.Approved).ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                Rejects     = ownideas.FindAll(x => x.IdeaStatus.Name == IdeaStatusesNames.Rejected).ToList()
                                      .Select(x => GetIdeaCardVM(x)).ToList(),
                MyReviews = new List<IdeaCardViewModel>()
            };
            if (User.IsInRole(UserStatusesNames.Expert))
                vm.MyReviews = db.GetExpert(User.Identity.Name).ReviewIdeas.Select(x => GetIdeaCardVM(x)).ToList();
            return View(vm);
        }
        private IdeaCardViewModel GetIdeaCardVM(Idea idea)
        {
            return new IdeaCardViewModel
            {
                Id = idea.Id,
                Name = idea.Name,
                Direction = idea.Direction.Name,
                AuthorId = idea.AuthorId,
                Image = idea.Image,
                ValueDegree = idea.ValueDegree,
                IdeaType = idea.IdeaType
            };
        }

        [HttpGet]
        [Authorize(Roles = "Админ")]
        public IActionResult AdminReviews()
        {
            var vm = db.GetPrivateIdeasOnReview().Select(x => GetIdeaCardVM(x)).ToList();
            return View(vm);
        }
    }
}