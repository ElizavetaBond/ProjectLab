using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.StaticNames;

namespace ProjectLab.Models
{
    public class AdminService : ProjectLabDbService
    {
        public AdminService() : base() { }

        public List<User> GetUsersForDirection(string DirectionId)
        {
            return Users.Find(x => x.Direction.Id == DirectionId).ToList();
        }

        public void SetExpert(string UserId)
        {
            var update = new UpdateDefinitionBuilder<User>().Set(x => x.UserStatus,
                        UserStatuses.Find(u => u.Name == UserStatusesNames.Expert).FirstOrDefault());
            Users.FindOneAndUpdate(x => x.Id == UserId, update);
            var user = GetUser(UserId);
            Experts.InsertOne(new Expert
            {
                Id = user.Id,
                Direction = user.Direction,
                EducationalInstitution = user.EducationalInstitution,
                ReviewIdeas = new List<Idea>()
            });
        }

        public bool CancelExpert(string ExpertId)
        {
            var expert = Experts.Find(x => x.Id == ExpertId).FirstOrDefault();
            if (expert.ReviewIdeas.Count > 0) return false;
            var update = new UpdateDefinitionBuilder<User>().Set(x => x.UserStatus,
                        UserStatuses.Find(u => u.Name == UserStatusesNames.Participant).FirstOrDefault());
            Users.FindOneAndUpdate(x => x.Id == ExpertId, update);
            Experts.FindOneAndDelete(x => x.Id == ExpertId);
            return true;
        }
    }
}
