using ProjectLab.Models.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using ProjectLab.StaticNames;

namespace ProjectLab.Models.Statistics
{
    public abstract class Statistics
    {
        public string Title { get; set; }               // названиие диаграммы
        public string ComparedCategory { get; set; }    // сравниваемая категория
        public string MeasuredQuantity { get; set; }    // измеряемая величина
        public List<KeyValue> KeyValues { get; set; }   // список значений


        protected ProjectLabDbService db;


        // фильтры
        protected DateTime Start { get; set; }
        protected DateTime Finish { get; set; }
        protected List<Direction> Directions { get; set; }
        protected List<EducationalInstitution> EducationalInstitutions { get; set; }
        protected List<UserCategory> UserCategories { get; set; }


        public Statistics(ProjectLabDbService serv, StatisticsSettings settings)
        {
            db = serv;
            Title = settings.ComparedCategory + " / " + settings.MeasuredQuantity;
            ComparedCategory = settings.ComparedCategory;
            MeasuredQuantity = settings.MeasuredQuantity;
            KeyValues = new List<KeyValue>();
            Start = settings.Start;
            Finish = settings.Finish;
            Directions = settings.Directions;
            EducationalInstitutions = settings.EducationalInstitutions;
            UserCategories = settings.UserCategories;
        }

        public void Generate() // вызывает метод для формирования статистики в зависимости от измеряемой величины
        {
            if (MeasuredQuantity == MeasuredQuantitiesNames.ApprovedIdeas)
                CountApprovedIdeas();
            else if (MeasuredQuantity == MeasuredQuantitiesNames.CreatedProjects)
                CountCreatedProjects();
            else if (MeasuredQuantity == MeasuredQuantitiesNames.ParticipantsInProjects)
                CountParticipantsInProjects();
            else if (MeasuredQuantity == MeasuredQuantitiesNames.ArchieveProjects)
                CountArchieveProjects();
            else if (MeasuredQuantity == MeasuredQuantitiesNames.RegisteredUsers)
                CountRegisteredUsers();
        }

        protected abstract void CountApprovedIdeas();               // подсчет количества утвержденных идей
        protected abstract void CountCreatedProjects();             // подсчет количества созданных проектов
        protected abstract void CountParticipantsInProjects();    // подсчет количества проектов, в котором участвуют..
        protected abstract void CountArchieveProjects();            // подсчет количества завершенных проектов
        protected abstract void CountRegisteredUsers();             // подсчет количества зарегистрированных пользователей
    }
}
