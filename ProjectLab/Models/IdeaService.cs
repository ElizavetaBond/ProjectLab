using System;
using System.Collections.Generic;
using System.Linq;
using ProjectLab.StaticNames;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProjectLab.Models.References;
using System.IO;
using MongoDB.Bson;

namespace ProjectLab.Models
{
    public class IdeaService : ProjectLabDbService
    {
        public IdeaService(): base() {}

        public void CreateIdea(string name, string type, string target, string purpose, string description,
                               string equipment, string safety, string authorId, string directionId, string video,
                               Stream fileStream, string fileName, string fileType, List<Section> sections)
        {
            var idea = new Idea
            {
                Name = name,
                IdeaType = type,
                Target = target,
                Purpose = purpose,
                Description = description,
                Equipment = equipment,
                Safety = safety,
                AuthorId = authorId,
                IdeaStatus = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Draft).FirstOrDefault(), // черновик
                Direction = Directions.Find(x => x.Id == directionId).FirstOrDefault(),
                Video = video,
                Image = fileStream == null ? null :
                            new File
                            {
                                Id = SaveFile(fileStream, fileName),
                                Type = fileType
                            },
                Comments = new List<Comment>(),
                ProjectTemplate = new ProjectTemplate { Sections = sections }
            };
            
            Ideas.InsertOne(idea);
        }
    
        public List<Idea> GetOpenApprovedIdeas()
        {            
            return Ideas.Find(x => x.IdeaType == IdeaTypesNames.Open && x.IdeaStatus.Name == IdeaStatusesNames.Approved).ToList();
        }

        public List<Idea> GetIdeasByAuthor(string authorId)
        {
            return Ideas.Find(x => x.AuthorId == authorId).ToList();
        }

        public void DeleteIdea(string IdeaId)
        {
            Ideas.DeleteOne(x => x.Id == IdeaId);
        }

        public void SendIdeaToAdminOnReview(string IdeaId)
        {
            var idea = GetIdea(IdeaId);
            var review = new Review
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Idea = idea,
                DateSending = DateTime.Now,
                Resolutions = new List<Resolution>(),
                ExpertsId = new List<string>()
            };
            Reviews.InsertOne(review);
            var updateIdea = new UpdateDefinitionBuilder<Idea>().Set(i => i.IdeaStatus,
                IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.OnReview).FirstOrDefault())
                .Set(i => i.ReviewId, review.Id);
            Ideas.FindOneAndUpdate(i => i.Id == IdeaId, updateIdea);
        }

        public void SendIdeaToReview(string IdeaId)
        {
            var idea = GetIdea(IdeaId);
            var author = GetUser(idea.AuthorId);
            if (idea.IdeaType == IdeaTypesNames.Private) SendIdeaToAdminOnReview(IdeaId); // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
            else
            {
                var experts = Experts.Find(x => x.Direction.Id == idea.Direction.Id
                                              && x.EducationalInstitution.Id != author.EducationalInstitution.Id).ToList()
                                              .OrderBy(x => x.ReviewIdeas.Count).ToList();
                if (experts.Count < 3) SendIdeaToAdminOnReview(IdeaId);  // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
                else
                {
                    var review = new Review
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Idea = idea,
                        DateSending = DateTime.Now,
                        Resolutions = new List<Resolution>(),
                        ExpertsId = new List<string>()
                    };
                    for (int i = 0; i < 3; i++)
                    {
                        var updateExpert = new UpdateDefinitionBuilder<Expert>().Push(x => x.ReviewIdeas, idea);
                        Experts.FindOneAndUpdate(x => x.Id == experts[i].Id, updateExpert);
                        review.ExpertsId.Add(experts[i].Id);
                    }
                    Reviews.InsertOne(review);
                    var updateIdea = new UpdateDefinitionBuilder<Idea>().Set(i => i.IdeaStatus,
                        IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.OnReview).FirstOrDefault())
                        .Set(i => i.ReviewId, review.Id);
                    Ideas.FindOneAndUpdate(i => i.Id == IdeaId, updateIdea);
                }
            }

            
        }

        public void RegistExpertResolution(string expertId, string ideaId, int decision, string comment, int valueDegree)
        {
            var idea = GetIdea(ideaId);
            var resol = new Resolution // резолюция эксперта
            {
                ExpertId = expertId,
                Decision = decision,
                ValueDegree = decision > 0 ? valueDegree : 0,
                Comment = comment,
                DateReview = DateTime.Now
            };

            // сохраняем резолюцию в коллекции Reviews
            var updateReview = new UpdateDefinitionBuilder<Review>().Push(x => x.Resolutions, resol); 
            Reviews.FindOneAndUpdate(x => x.Id == idea.ReviewId, updateReview);

            // удаляем идею у Эксперта
            var updateExp = new UpdateDefinitionBuilder<Expert>().PullFilter(exp => exp.ReviewIdeas, x => x.Id == ideaId); 
            Experts.FindOneAndUpdate(x => x.Id == expertId, updateExp);

            // получаем резолюции
            var resolutions = Reviews.Find(x => x.Id == idea.ReviewId).FirstOrDefault().Resolutions;
            // если все эксперты оценили, то меняем статус
            if (resolutions.Count == 3) 
            {
                int res = 0, degree = 0;
                foreach (var x in resolutions)
                {
                    degree += x.ValueDegree;
                    res += x.Decision;
                }
                var status = new IdeaStatus();
                if (res > 1)  // утвердить
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Approved).FirstOrDefault();
                else if (res < -1) // отклонить
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Rejected).FirstOrDefault();
                else // отправить на доработку
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Draft).FirstOrDefault();
                // изменить статус идеи, установить степень ценности
                var updateIdea = new UpdateDefinitionBuilder<Idea>().Set(idea => idea.ValueDegree, (int)(degree / 3))
                                                                .Set(idea => idea.IdeaStatus, status)
                                                                .Set(idea => idea.Date, DateTime.Now);
                Ideas.FindOneAndUpdate(idea => idea.Id == ideaId, updateIdea);
            }
        }

        public void RegistAdminResolution(string userId, string ideaId, int decision, string comment, int valueDegree)
        {
            var idea = GetIdea(ideaId);
            var resol = new Resolution // резолюция эксперта
            {
                ExpertId = userId,
                Decision = decision,
                ValueDegree = decision > 0 ? valueDegree : 0,
                Comment = comment,
                DateReview = DateTime.Now
            };

            // сохраняем резолюцию в коллекции Reviews
            var updateReview = new UpdateDefinitionBuilder<Review>().Push(x => x.Resolutions, resol);
            Reviews.FindOneAndUpdate(x => x.Id == idea.ReviewId, updateReview);

                var status = new IdeaStatus();
                if (decision == 1)  // утвердить
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Approved).FirstOrDefault();
                else if (decision == -1) // отклонить
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Rejected).FirstOrDefault();
                else // отправить на доработку
                    status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Draft).FirstOrDefault();
                // изменить статус идеи, установить степень ценности
                var updateIdea = new UpdateDefinitionBuilder<Idea>().Set(idea => idea.ValueDegree, valueDegree)
                                                                .Set(idea => idea.IdeaStatus, status)
                                                                .Set(idea => idea.Date, DateTime.Now);
                Ideas.FindOneAndUpdate(idea => idea.Id == ideaId, updateIdea);
        }

        public Expert GetExpert (string ExpertId)
        {
            return Experts.Find(x => x.Id == ExpertId).FirstOrDefault();
        }

        public List<Resolution> GetResolutions(string IdeaId)
        {
            var idea = GetIdea(IdeaId);
            var review = Reviews.Find(x => x.Id == idea.ReviewId).FirstOrDefault();
            return review == null ? new List<Resolution>() : review.Resolutions;
        }
        
        public List<Idea> GetPrivateIdeasOnReview()
        {
            return Ideas.Find(x => x.IdeaType == IdeaTypesNames.Private && x.IdeaStatus.Name == IdeaStatusesNames.OnReview).ToList();
        }
    }
}
