using MongoDB.Driver;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ProjectLab.StaticNames;
using System.Threading.Tasks;
using System.Text;

namespace ProjectLab.Models
{
    public class AccountService: ProjectLabDbService
    {
        public AccountService(): base() { }

        byte[] GenSalt(int length)
        {
            RNGCryptoServiceProvider p = new RNGCryptoServiceProvider();
            var salt = new byte[length];
            p.GetBytes(salt);
            return salt;
        }

        public User GetUser (string email, string password)
        {
            User user = GetUserByEmail(email);
            if (user != null)
            {
                byte[] hash;
                byte[] passw = Encoding.Default.GetBytes(password);
                using (var sha1 = new HMACSHA1(user.PasswordSalt))
                {
                    hash = sha1.ComputeHash(passw);
                }
                if (Convert.ToBase64String(user.PasswordHash) == Convert.ToBase64String(hash)) 
                    return user;
            }
            return null;
        }

        public User GetUserByEmail (string email)
        {
            return Users.Find(x => x.Email == email).FirstOrDefault();
        }

        public void CreateUser(string email, string password, string surname, string name, string patron, 
            DateTime birthdate, string categoryId, string institutionId, string educationId, string addInform,
            string contacts, string directionId, Stream photoStream, String photoType, string photoName)
        {
            byte[] salt = GenSalt(32), hash, passw = Encoding.Default.GetBytes(password);
            using (var sha1 = new HMACSHA1(salt))
            {
                hash = sha1.ComputeHash(passw);
            }
            Users.InsertOne( new User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
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
