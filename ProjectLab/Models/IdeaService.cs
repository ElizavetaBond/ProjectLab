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
        public IdeaService(): base() { }

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
                Resolutions = new List<Resolution>(),
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

        public Expert GetExpert (string userId)
        {
            return Experts.Find(x => x.UserId == userId).FirstOrDefault();
        }

        public void DeleteIdea(string IdeaId)
        {
            var idea = GetIdea(IdeaId);
            if (idea.IdeaStatus.Name != IdeaStatusesNames.Approved && idea.Image != null)
                DeleteFile(idea.Image.Id);
            Ideas.DeleteOne(x => x.Id == idea.Id);
        }

        public void SendIdeaToReview(string IdeaId)
        {
            var idea = GetIdea(IdeaId);
            if (idea.IdeaType == IdeaTypesNames.Private) ; // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
            else
            {
                var experts = Experts.Find(x => x.Direction.Id == idea.Direction.Id) // выбрали экспертов по направленности
                                      .ToList()
                                      .OrderBy(x => x.ReviewIdeas.Count)
                                      .ToList(); // отсортировали по количеству работы
                if (experts.Count < 3) ;  // ОТПРАВЛЯЕМ АДМИНУ НА ПРОВЕРКУ!!!!
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var upd = new UpdateDefinitionBuilder<Expert>().Push(exp => exp.ReviewIdeas, idea);
                        Experts.FindOneAndUpdate(exp => exp.Id == experts[i].Id, upd);
                    }
                }
            }

            var update = new UpdateDefinitionBuilder<Idea>().Set(i => i.IdeaStatus, 
                        IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.OnReview).FirstOrDefault());
            Ideas.FindOneAndUpdate(i => i.Id == IdeaId, update);
        }

        public void RegistExpertResolution(string userId, string ideaId, int decision, string comment, int valueDegree)
        {
            var ExpertId = GetExpert(userId).Id;
            var resol = new Resolution // резолюция эксперта
            {
                ExpertId = ExpertId,
                Decision = decision,
                ValueDegree = decision > 0 ? valueDegree : 0,
                Comment = comment
            };

            var updateExp = new UpdateDefinitionBuilder<Expert>().PullFilter(exp => exp.ReviewIdeas, x => x.Id == ideaId); // удаляем идею у эксперта
            Experts.FindOneAndUpdate(x => x.Id == ExpertId, updateExp);

            var updateIdea = new UpdateDefinitionBuilder<Idea>().Push(idea => idea.Resolutions, resol); // добавляем резолюцию в список
            Ideas.FindOneAndUpdate(idea => idea.Id == ideaId, updateIdea);

            var resolutions = Ideas.Find(x => x.Id == ideaId).FirstOrDefault().Resolutions;
            if (resolutions.Count == 3) // если все эксперты оценили, то меняем статус
            {
                int res = 0, degree = 0;
                foreach (var x in resolutions)
                {
                    degree += x.ValueDegree;
                    res += x.Decision;
                }
                var status = new IdeaStatus();
                if (res > 1) status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Approved).FirstOrDefault();
                else if (res < -1) status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Rejected).FirstOrDefault();
                else status = IdeaStatuses.Find(x => x.Name == IdeaStatusesNames.Draft).FirstOrDefault();
                var update = new UpdateDefinitionBuilder<Idea>().Set(idea => idea.ValueDegree, (int)(degree / 3))
                                                                .Set(idea => idea.IdeaStatus, status)
                                                                .Set(idea => idea.Date, DateTime.Now);
                Ideas.FindOneAndUpdate(idea => idea.Id == ideaId, update);
            }
        }
        
    }
}
