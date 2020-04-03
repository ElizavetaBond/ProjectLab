using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectLab.Models.References;

namespace ProjectLab.Models
{
    public class ProjectLabDbService
    {
        public IGridFSBucket gridFS;   // файловое хранилище
        
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
        public IMongoCollection<Idea> Ideas { get; set; }
        public IMongoCollection<Project> Projects { get; set; }
        public IMongoCollection<User> Users { get; set; } 

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
        }

        /*public void Init()
        {
            var t = new IdeaStatus[] {  new IdeaStatus { Name = "Черновик" },
                                        new IdeaStatus { Name = "На модерации" },
                                        new IdeaStatus { Name = "Утверждена" },
                                        new IdeaStatus { Name = "Отклонена" } };
            IdeaStatuses.InsertMany(t);

            var t2 = new IdeaType[] {   new IdeaType { Name = "Открытая"},
                                        new IdeaType { Name = "Приватная" } };
            IdeaTypes.InsertMany(t2);

            var t3 = new Direction[] { new Direction { Name ="Медиа" },
                                       new Direction { Name ="Астрономия" },
                                       new Direction { Name ="Обществознание" },
                                       new Direction { Name ="Биология" },
                                       new Direction { Name ="Основы безопасности жизнедеятелности" },
                                       new Direction { Name ="Политология" },
                                       new Direction { Name ="География" },
                                       new Direction { Name ="Правоведение" },
                                       new Direction { Name ="Дефектология, логопедия" },
                                       new Direction { Name ="Психология" },
                                       new Direction { Name ="Русский язык и литература" },
                                       new Direction { Name ="Английских" },
                                       new Direction { Name ="Казахский" },
                                       new Direction { Name ="Украинский" },
                                       new Direction { Name ="Белорусский" },
                                       new Direction { Name ="Армянский" },
                                       new Direction { Name ="Грузинский" },
                                       new Direction { Name ="Социология" },
                                       new Direction { Name ="Информатика" },
                                       new Direction { Name ="Социальная работа'" },
                                       new Direction { Name ="Искусствоведение, изобразительное искусство" },
                                       new Direction { Name ="История и мировая история" },
                                       new Direction { Name ="Технология" },
                                       new Direction { Name ="Управление" },
                                       new Direction { Name ="Культурология" },
                                       new Direction { Name ="Физическая культура и здоровье" },
                                       new Direction { Name ="Математика" },
                                       new Direction { Name ="Физика" },
                                       new Direction { Name ="Геометрия" },
                                       new Direction { Name ="Медицинская деятельность" },
                                       new Direction { Name ="Химия" },
                                       new Direction { Name ="Международные отношения" },
                                       new Direction { Name ="Черчение" },
                                       new Direction { Name ="Музыка" },
                                       new Direction { Name ="Другое" }};
            Directions.InsertMany(t3);

            var t4 = new UserStatus[] { new UserStatus { Name = "Участник сообщества"},
                                        new UserStatus { Name = "Эксперт"},
                                        new UserStatus { Name = "Администратор" } };
            UserStatuses.InsertMany(t4);

            var i1 = new Idea {
                Name = "Участник сообщества",
                Description = "Пример",
                Target = "Пример",
                Purpose = "Пример",
                Equipment = "Пример",
                Safety = "Пример" };
            i1.Description = "Медиа2";

            var i2 = new Idea
            {
                Name = "Участник сообщества",
                Description = "Пример",
                Target = "Пример",
                Purpose = "Пример",
                Equipment = "Пример",
                Safety = "Пример"
            };
            Ideas.InsertMany(new Idea[] { i1, i2 });
        }*/
    }
}
