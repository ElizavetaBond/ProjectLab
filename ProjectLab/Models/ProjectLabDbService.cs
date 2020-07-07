using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectLab.Models.References;
using System.Collections.Generic;
using System.IO;

namespace ProjectLab.Models
{
    public class ProjectLabDbService
    {
        private IGridFSBucket gridFS;   // файловое хранилище
        
        // справочники
        protected IMongoCollection<Subject> Subjects { get; set; }
        protected IMongoCollection<Direction> Directions { get; set; }
        protected IMongoCollection<Education> Educations { get; set; }
        protected IMongoCollection<EducationalInstitution> EducationalInstitutions { get; set; }
        protected IMongoCollection<IdeaStatus> IdeaStatuses { get; set; }
        protected IMongoCollection<ProjectStatus> ProjectStatuses { get; set; }
        protected IMongoCollection<ProjectType> ProjectTypes { get; set; }
        protected IMongoCollection<Municipality> Municipalities { get; set; }
        protected IMongoCollection<UserCategory> UserCategories { get; set; }
        protected IMongoCollection<UserStatus> UserStatuses { get; set; }

        // коллекции
        protected IMongoCollection<Idea> Ideas { get; set; } // идеи
        protected IMongoCollection<Project> Projects { get; set; }
        protected IMongoCollection<User> Users { get; set; }
        protected IMongoCollection<Expert> Experts { get; set; }
        protected IMongoCollection<Review> Reviews { get; set; }

        public ProjectLabDbService()
        {
            string connectionString = "mongodb://localhost:27017";
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("projectlab");
            
            gridFS = new GridFSBucket(database);

            // справочники
            Subjects = database.GetCollection<Subject>("Subjects");
            Directions = database.GetCollection<Direction>("Directions");
            Educations = database.GetCollection<Education>("Educations");
            EducationalInstitutions = database.GetCollection<EducationalInstitution>("EducationalInstitutions");
            IdeaStatuses = database.GetCollection<IdeaStatus>("IdeaStatuses");
            ProjectStatuses = database.GetCollection<ProjectStatus>("ProjectStatuses");
            ProjectTypes = database.GetCollection<ProjectType>("ProjectTypes");
            Municipalities = database.GetCollection<Municipality>("Municipalities");
            UserCategories = database.GetCollection<UserCategory>("UserCategories");
            UserStatuses = database.GetCollection<UserStatus>("UserStatuses");

            // коллекции
            Ideas = database.GetCollection<Idea>("Ideas");
            Projects = database.GetCollection<Project>("Projects");
            Users = database.GetCollection<User>("Users");
            Experts = database.GetCollection<Expert>("Experts");
            Reviews = database.GetCollection<Review>("Reviews");
        }
        
        public byte[] GetFile(string id) // получение изображения
        {
            try
            {
                return gridFS.DownloadAsBytes(new ObjectId(id));
            }
            catch 
            {
                return null;
            }
        }
        
        public string SaveFile(Stream fileStream, string fileName) // сохранение изображения
        {
            return gridFS.UploadFromStream(fileName, fileStream).ToString();
        }

        public void DeleteFile(string id) // удаляет изображение из хранилища
        {
            gridFS.Delete(new ObjectId(id));
        }

        public List<Direction> GetDirections()
        {
            return Directions.Find(new BsonDocument()).ToList();
        }

        public List<EducationalInstitution> GetEducationalInstitutions()
        {
            return EducationalInstitutions.Find(new BsonDocument()).ToList();
        }

        public List<UserCategory> GetUserCategories()
        {
            return UserCategories.Find(new BsonDocument()).ToList();
        }

        public List<Education> GetEducations()
        {
            return Educations.Find(new BsonDocument()).ToList();
        }

        public List<User> GetUsers()
        {
            return Users.Find(new BsonDocument()).ToList();
        }
        public List<Idea> GetIdeas()
        {
            return Ideas.Find(new BsonDocument()).ToList();
        }

        public List<Project> GetProjects()
        {
            return Projects.Find(new BsonDocument()).ToList();
        }

        public Idea GetIdea (string IdeaId)
        {
            return Ideas.Find(x => x.Id == IdeaId).FirstOrDefault();
        }

        public User GetUser (string UserId)
        {
            return Users.Find(x => x.Id == UserId).FirstOrDefault();
        }

        public Project GetProject (string ProjectId)
        {
            return Projects.Find(x => x.Id == ProjectId).FirstOrDefault();
        }
    }
}
