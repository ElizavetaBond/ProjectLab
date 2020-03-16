using Microsoft.AspNetCore.Mvc;
using ProjectLab.Models;
using ProjectLab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers.ViewComponents
{
    public class IdeaCardViewComponent : ViewComponent
    {
        private readonly ProjectLabDbService db;

        public IdeaCardViewComponent (ProjectLabDbService context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(IdeaCardViewModel vm)
        {
            return View(vm);
        }
    }
}
