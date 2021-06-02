using ContactСenterReporting.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ContactСenterReporting
{

    class Program
    {
        private static Stopwatch stopWatch;

        static void Main(string[] args)
        {
            // Получаем набор элементов из исходного файла
            var sessions = File.ReadLines(GetCsvPath())
                                .Select(a => a.Split(';'))
                                .Skip(1) 
                                .Select(columns => Session.Create(
                                    columns[0],
                                    columns[1],
                                    columns[3],
                                    columns[4]));


            DisplayMaxSessionDay(sessions);
            DisplayDurationStatistics(sessions);

            Console.ReadLine();
        }

        /// <summary>
        /// Вывод на <see cref="Console"/>  отчета о максимальном числе активных сессмй за каждый день 
        /// </summary>
        /// <param name="sessions"></param>
        private static void DisplayMaxSessionDay(IEnumerable<Session> sessions)
        {
            RunStopWatcher();

            // Получаем распределение сессий по дням
            var groups = sessions.GroupBy(x => new { x.Start.Year, x.Start.Month, x.Start.Day });

            var loadStatistics = new Dictionary<DateTime, int>();

            foreach (var group in groups)
            {
                // Создаем дерево диапазонов
                var tree = new RangeTree<DateTime>();
                // Создаем коллекцию временных точек
                HashSet<DateTime> dates = new HashSet<DateTime>();

                foreach (var session in group)
                {
                    dates.Add(session.Start);
                    dates.Add(session.End);
                }
                var sordedRangeDates = dates.OrderBy(x => x.Hour)
                                            .ThenBy(m => m.Minute)
                                            .ThenBy(s => s.Second)
                                            .ThenBy(ms => ms.Millisecond)
                                            .ToArray();

                // Заполняем дерево диапазонов
                foreach (var session in group)
                    tree.Add(session.Start, session.End); 

                loadStatistics.Add(new DateTime(group.Key.Year, group.Key.Month, group.Key.Day), sordedRangeDates.Select(x => tree[x].Count()).Max());
            }

            Console.WriteLine("##### ЗАДАНИЕ 1 #####");

            foreach (var day in loadStatistics)
            {
                Console.WriteLine($"{day.Key.ToString("d")}  {day.Value}");
            }

            Console.WriteLine("Время выполнения: " + GetTimeOperation());
        }

        /// <summary>
        /// Вывод на <see cref="Console"/>  отчета о продолжительности нахождения оператора в различных <see cref="State"/> 
        /// </summary>
        /// <param name="sessions"></param>
        private static void DisplayDurationStatistics(IEnumerable<Session> sessions)
        {
            var operatores = sessions
                            .GroupBy(x => new { x.Operator, x.State })
                            .Select(z => new { z.Key.Operator, z.Key.State, TimeSpan = z.Sum(y => (y.End - y.Start).TotalMinutes) })
                            .GroupBy(n => n.Operator)
                            .Select(r => new
                            {
                                Operator = r.Key.ToString(),
                                SetState = new StateDictionary<State, double>(r.Select(h => new KeyValuePair<State, double>(h.State, h.TimeSpan)))
                            });

            Console.WriteLine("##### ЗАДАНИЕ 2 #####");
            foreach (var _operator in operatores)
            {
                Console.WriteLine("{0}  {1}", _operator.Operator, _operator.SetState.ToString());
            }
        }

        private static string GetTimeOperation() 
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               ts.Hours, ts.Minutes, ts.Seconds,
               ts.Milliseconds);
            return elapsedTime;
        }

        private static void RunStopWatcher()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        /// <summary>
        /// Получает путь к файлу формата .csv
        /// </summary>
        /// <returns></returns>
        private static string GetCsvPath()
        {
            string сsvPath = String.Empty;

            bool result = false;

            while (!result)
            {
                
                Console.WriteLine("Введите имя файла для обработки с расширением .csv");

                сsvPath = Console.ReadLine();

                if (!File.Exists(сsvPath))
                {
                    Console.WriteLine("Указанный файл не существует");
                    
                    continue;
                }
                if (!Path.GetExtension(сsvPath).Equals(".csv"))
                {
                    Console.WriteLine("Указанно неверное расширения файла");
                    
                    continue;
                }

                break;
            }

            return сsvPath;
        }
    }
}
