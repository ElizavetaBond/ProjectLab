﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.ViewModels.Account
{
    public class IdeasOwnViewModel
    {
        // Черновики
        public List<IdeaCardViewModel> Drafts { get; set; }
        // На модерации
        public List<IdeaCardViewModel> Reviews { get; set; }
        // Утвержденные
        public List<IdeaCardViewModel> Approves { get; set; }
        // ОТклоненные
        public List<IdeaCardViewModel> Rejects { get; set; }
    }
}