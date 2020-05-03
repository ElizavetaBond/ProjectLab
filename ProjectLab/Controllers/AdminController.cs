using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using ProjectLab.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers
{
    [Authorize(Roles = "Админ")]
    public class AdminController: Controller
    {
        private readonly ProjectLabDbService db;

        public AdminController(ProjectLabDbService context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Menu()
        {
            var menu = new List<MenuItem>()
            {
                new MenuItem {Name="Личные сведения", Controller="Account", Action="Account"},
                new MenuItem {Name="Мои идеи", Controller="Idea", Action="Menu"},
                new MenuItem {Name="Справочники", Controller="Admin", Action=""}
            };
            return PartialView();
        }
    }
}
