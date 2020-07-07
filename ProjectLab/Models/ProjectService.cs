using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectLab.Models.References;
using ProjectLab.StaticNames;
using ProjectLab.ViewModels.Project;

namespace ProjectLab.Models
{
    public class ProjectService: ProjectLabDbService
    {
        public ProjectService(): base() { }
        
        public List<Project> GetWorkAvailableProjects()
        {
            return Projects.Find(x => x.ProjectStatus.Name == ProjectStatusesNames.Working
                                    && x.ProjectType.Name != ProjectTypesNames.Private).ToList();
        }

        public List<Project> GetCompetedkAvailableProjects()
        {
            return Projects.Find(x => x.ProjectStatus.Name == ProjectStatusesNames.Completed
                                    && x.ProjectType.Name != ProjectTypesNames.Private).ToList();
        }

        public void CreateProject(string IdeaId, string managerId, string typeId, DateTime finish)
        {
            var idea = GetIdea(IdeaId);
            Projects.InsertOne(new Project
            {
                Idea = idea,
                ManagerId = managerId,
                ProjectType = ProjectTypes.Find(x => x.Id == typeId).FirstOrDefault(),
                ProjectStatus = ProjectStatuses.Find(x => x.Name == ProjectStatusesNames.Working).FirstOrDefault(),
                Start = DateTime.Now,
                Finish = finish,
                Comments = new List<Comment>(),
                ParticipantsId = new List<string> { managerId },
                Sections = idea.ProjectTemplate.Sections
            });
        }

        public void AddAnswear(string userId, List<ComponentBrowseProjectViewModel> components, string projectId, int sectionNum)
        {
            var answear = new Answear
            {
                AuthorId = userId,
                Date = DateTime.Now,
                Components = new List<Component>()
            };
            foreach (var x in components)
            {
                var value = "";
                File file = null;
                if (x.ComponentType == ComponentsNames.Flag)
                {
                    value = x.Flag.ToString();
                }
                else if (x.ComponentType == ComponentsNames.File || x.ComponentType == ComponentsNames.Photo)
                {
                    file = x.File == null ? null :
                        new File
                        {
                            Id = SaveFile(x.File.OpenReadStream(), x.File.FileName),
                            Type = x.File.ContentType
                        };
                }
                else if (x.ComponentType == ComponentsNames.MultipleChoice)
                {
                    for (int i = 0; i < x.ListSelect.Count; i++)
                        if (x.ListRes[i])
                            value += x.ListSelect[i] + "; ";
                }
                else
                {
                    value = x.Value;
                }
                answear.Components.Add(new Component
                {
                    Name = x.Name,
                    ComponentType = x.ComponentType,
                    Description = x.Description,
                    IsNecessary = x.IsNecessary,
                    ListSelect = x.ListSelect,
                    Value = value,
                    File = file
                });
            }
            var update = new UpdateDefinitionBuilder<Project>().Push(x => x.Sections[sectionNum].Answears, answear);
            Projects.FindOneAndUpdate(x => x.Id == projectId, update);
        }

        public void CompleteProject(string projectId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Set(x => x.ProjectStatus, 
                            ProjectStatuses.Find(s => s.Name == ProjectStatusesNames.Completed).FirstOrDefault());
            Projects.FindOneAndUpdate(x => x.Id == projectId, update);
        }

        public void CancelProject(string projectId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Set(x => x.ProjectStatus,
                            ProjectStatuses.Find(s => s.Name == ProjectStatusesNames.Canceled).FirstOrDefault());
            Projects.FindOneAndUpdate(x => x.Id == projectId, update);
        }

        public void LeaveProject(string projectId, string participantId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Pull(x => x.ParticipantsId, participantId);
            Projects.FindOneAndUpdate(x => x.Id == projectId, update);
        }

        public List<Project> GetManagmentProjects(string userId)
        {
            return Projects.Find(x => x.ManagerId == userId).ToList();
        }

        public List<Project> GetParticipantProjects(string userId)
        {
            var projects = Projects.Find(new BsonDocument()).ToList();
            var res = new List<Project>();
            foreach (var p in projects)
            {
                if (p.ParticipantsId.FindIndex(x => x == userId) != -1)
                    res.Add(p);
            }
            return res;
        }

        public List<ProjectType> GetProjectTypes()
        {
            return ProjectTypes.Find(new BsonDocument()).ToList();
        }

        public void AddParticipant(string ProjectId, string UserId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Push(x => x.ParticipantsId, UserId);
            Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
        }

        public void ChangeProjectType (string ProjectId, string ProjectTypeId)
        {
            var update = new UpdateDefinitionBuilder<Project>().Set(x => x.ProjectType,
                    ProjectTypes.Find(p => p.Id == ProjectTypeId).FirstOrDefault());
            Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
        }

        public void ChangeDateFinish(string ProjectId, DateTime finish)
        {
            var update = new UpdateDefinitionBuilder<Project>().Set(x => x.Finish, finish);
            Projects.FindOneAndUpdate(x => x.Id == ProjectId, update);
        }
    }
}
