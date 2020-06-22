using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }

        public void CancelExpert(string ExpertId)
        {
            var update = new UpdateDefinitionBuilder<User>().Set(x => x.UserStatus,
                        UserStatuses.Find(u => u.Name == UserStatusesNames.Participant).FirstOrDefault());
            Users.FindOneAndUpdate(x => x.Id == ExpertId, update);
            // НАДО ЧТО ТО СДЕЛАТЬ С ИДЕЯМИ КОТОРЫЕ БЫЛИ У НЕГО НА ПРОВЕРКЕ
        }
    }
}
