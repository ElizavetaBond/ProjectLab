using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectLab.Models.References;
using System.IO;

namespace ProjectLab.Models
{
    public class ProjectLabDbService
    {
        private IGridFSBucket gridFS;   // файловое хранилище
        
        // справочники
        public IMongoCollection<Area> Areas { get; set; }
        public IMongoCollection<City> Cities { get; set; }
        public IMongoCollection<Direction> Directions { get; set; }
        public IMongoCollection<Education> Educations { get; set; }
        public IMongoCollection<EducationalInstitution> EducationalInstitutions { get; set; }
        public IMongoCollection<IdeaStatus> IdeaStatuses { get; set; }
        public IMongoCollection<ProjectStatus> ProjectsStatuses { get; set; }
        public IMongoCollection<Region> Regions { get; set; }
        public IMongoCollection<Reward> RewardTypes { get; set; }
        public IMongoCollection<UserCategory> UserCategories { get; set; }
        public IMongoCollection<UserStatus> UserStatuses { get; set; }

        // коллекции
        public IMongoCollection<Idea> Ideas { get; set; } // идеи
        public IMongoCollection<Project> Projects { get; set; }
        public IMongoCollection<User> Users { get; set; } 
        public IMongoCollection<Expert> Experts { get; set; }

        public ProjectLabDbService()
        {
            string connectionString = "mongodb://localhost:27017";
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("projectlab");
            
            gridFS = new GridFSBucket(database);

            // справочники
            Areas = database.GetCollection<Area>("Areas");
            Cities = database.GetCollection<City>("Cities");
            Directions = database.GetCollection<Direction>("Directions");
            Educations = database.GetCollection<Education>("Educations");
            EducationalInstitutions = database.GetCollection<EducationalInstitution>("EducationalInstitutions");
            IdeaStatuses = database.GetCollection<IdeaStatus>("IdeaStatuses");
            ProjectsStatuses = database.GetCollection<ProjectStatus>("ProjectStatuses");
            Regions = database.GetCollection<Region>("Regions");
            RewardTypes = database.GetCollection<Reward>("RewardTypes");
            UserCategories = database.GetCollection<UserCategory>("UserCategories");
            UserStatuses = database.GetCollection<UserStatus>("UserStatuses");

            // коллекции
            Ideas = database.GetCollection<Idea>("Ideas");
            Projects = database.GetCollection<Project>("Projects");
            Users = database.GetCollection<User>("Users");
            Experts = database.GetCollection<Expert>("Experts");
        }
        
        public byte[] GetImage(string id) // получение изображения
        {
            return gridFS.DownloadAsBytes(new ObjectId(id));
        }
        
        public string LoadImage(Stream imageStream, string imageName) // сохранение изображения
        {
            return gridFS.UploadFromStream(imageName, imageStream).ToString();
        }

        public void DeleteImage(string id) // удаляет изображение из хранилища
        {
            gridFS.Delete(new ObjectId(id));
        }
    }
}
