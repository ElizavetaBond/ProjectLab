using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Idea
{
    public class IdeaMenuViewModel
    {
        // Черновики
        public List<IdeaCardViewModel> Drafts { get; set; }
        // На модерации
        public List<IdeaCardViewModel> OnReviews { get; set; }
        // Утвержденные
        public List<IdeaCardViewModel> Approves { get; set; }
        // ОТклоненные
        public List<IdeaCardViewModel> Rejects { get; set; }
        // На проверке (для эксперта)
        public List<IdeaCardViewModel> MyReviews { get; set; }
    }
}
