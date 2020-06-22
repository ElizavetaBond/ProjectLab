using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLab.StaticNames
{
    public static class ComponentsNames
    {
        public static readonly string Text;

        public static readonly string Message;

        public static readonly string Date;

        public static readonly string Time;

        public static readonly string Number;

        public static readonly string Flag;

        public static readonly string File;

        public static readonly string Photo;

        public static readonly string Location;

        public static readonly string Choice;

        public static readonly string MultipleChoice;

        public static readonly string Hyperlink;

        static ComponentsNames()
        {
            Text = "Текст";
            Message = "Сообщение";
            Date = "Дата";
            Time = "Время";
            Number = "Число";
            Flag = "Флаг";
            File = "Файл";
            Photo = "Фото";
            Location = "Место";
            Choice = "Выбор";
            MultipleChoice = "Множественный выбор";
            Hyperlink = "Гиперссылка";
        }

        public static List<string> Get()
        {
            return new List<string>()
            {
                Text, Message, Date, Time, Number, Flag, File, Photo, Location, Choice, MultipleChoice, Hyperlink
            };
        }

    }
}
