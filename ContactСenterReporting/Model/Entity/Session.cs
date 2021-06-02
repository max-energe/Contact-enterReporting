using System;
using System.ComponentModel.DataAnnotations;

namespace ContactСenterReporting.Model
{
    class Session
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; } 
        public State State { get; private set; }
        public string Operator { get; private set; }

       /// <summary>
       /// Инициализация сессии
       /// </summary>
       /// <param name="start"></param>
       /// <param name="end"></param>
       /// <param name="_operator"></param>
       /// <param name="state"></param>
       /// <returns></returns>
        public static Session Create(string start, string end, string _operator, string state)
        {
            if (!DateTime.TryParse(start, out DateTime _startDate))
                throw new Exception(string.Format("{0} невозможно преобразовать в {1}", start, typeof(DateTime)));
            if (!DateTime.TryParse(end, out DateTime _endDate))
                throw new Exception(string.Format("{0} невозможно преобразовать в {1}", end, typeof(DateTime)));
            if (string.IsNullOrWhiteSpace(_operator))
                throw new Exception(string.Format("{0} не может быть пустым, null или состоять из пробелов", nameof(_operator)));
             
            return new Session()
            {
                Start = _startDate,
                End = _endDate,
                Operator = _operator,
                State = StateConvert(state)
            };
        }

        public static Session Create(DateTime start, DateTime end)
            => new Session()
            {
                Start = start,
                End = end
            };

        private static State StateConvert(string state)
        {
            switch (state)
            {
                case "Готов":
                    return State.Ready;
                case "Обработка":
                    return State.Treatment;
                case "Пауза":
                    return State.Pause;
                case "Разговор":
                    return State.Conversation;
                case "Перезвон":
                    return State.Pause;
                default:
                    throw new ArgumentException("Некорректное значение для получения состояния");
            }
        }
    }

    internal enum State
    {
        Ready,
        Conversation,
        Pause,
        Treatment,
        Chime
    }
}
