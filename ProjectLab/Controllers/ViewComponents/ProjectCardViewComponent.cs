using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using ProjectLab.ViewModels.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers.ViewComponents
{
    public class ProjectCardViewComponent : ViewComponent
    {
        private readonly ProjectLabDbService db;

        public ProjectCardViewComponent (ProjectLabDbService context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(ProjectCardViewModel vm)
        {
            return View(vm);
        }
    }
}
