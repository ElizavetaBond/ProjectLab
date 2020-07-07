using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class IdeaStatusesNames
    {
        public static readonly string Draft;

        public static readonly string OnReview;

        public static readonly string OnReviewAdmin;

        public static readonly string Approved;

        public static readonly string Rejected;

        static IdeaStatusesNames()
        {
            Draft = "Черновик";
            OnReview = "На модерации";
            OnReviewAdmin = "На модерации Админом";
            Approved = "Утверждена";
            Rejected = "Отклонена";
        }
    }
}
