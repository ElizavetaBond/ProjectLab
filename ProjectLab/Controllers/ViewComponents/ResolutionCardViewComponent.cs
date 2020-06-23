using Microsoft.AspNetCore.Mvc;
using ProjectLab.ViewModels.Idea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.Controllers.ViewComponents
{
    public class ResolutionCardViewComponent : ViewComponent
    {

        public ResolutionCardViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(ResolutionCardViewModel vm)
        {
            return View(vm);
        }
    }
}
