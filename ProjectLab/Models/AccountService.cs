using MongoDB.Driver;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ProjectLab.StaticNames;
using System.Threading.Tasks;

namespace ProjectLab.Models
{
    public class AccountService: ProjectLabDbService
    {
        public AccountService(): base() { }

        public User GetUser (string email, string password)
        {
            return Users.Find(u => u.Email == email && u.Password == password).FirstOrDefault();
        }

        public User GetUserByEmail (string email)
        {
            return Users.Find(x => x.Email == email).FirstOrDefault();
        }

        public void CreateUser(string email, string password, string surname, string name, string patron, 
            DateTime birthdate, string categoryId, string institutionId, string educationId, string addInform,
            string contacts, string directionId, Stream photoStream, String photoType, string photoName)
        {
            Users.InsertOne( new User
            {
                Email = email,
                Password = password,
                Surname = surname,
                Name = name,
                Patronymic = patron,
                BirthDate = birthdate,
                UserStatus = UserStatuses.Find(x => x.Name == UserStatusesNames.Participant).FirstOrDefault(),
                UserCategory = UserCategories.Find(x => x.Id == categoryId).FirstOrDefault(),
                EducationalInstitution = EducationalInstitutions.Find(x => x.Id == institutionId).FirstOrDefault(),
                Education = Educations.Find(x => x.Id == educationId).FirstOrDefault(),
                AddInform = addInform,
                Contacts = contacts,
                Direction = Directions.Find(x => x.Id == directionId).FirstOrDefault(),
                RegistDate = DateTime.Now,
                Rewards = new List<References.Reward>(),
                Photo = photoStream == null ? null :
                            new File
                            {
                                Id = SaveFile(photoStream, photoName),
                                Type = photoType
                            }
            });
        }
    }
}
