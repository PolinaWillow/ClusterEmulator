using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator
{
    internal class Statistic
    {
        public DateTime start_all { get; set; }
        public DateTime end_all { get; set; }
        public TimeSpan allWorkTime { get; set; }

        public DateTime start_useful { get; set; }
        public DateTime end_useful { get; set; }
        public TimeSpan usefulWorkTime { get; set; }

        public DateTime start_read { get; set; }
        public DateTime end_read { get; set; }
        public TimeSpan readWorkTime { get; set; }

        public DateTime currentUseFul_start { get; set; }
        public DateTime currentUseFul_end { get; set; }
        public TimeSpan CurrentUseFull { get; set; }

        public IntervalRecords_Dictionary iterationRecords { get; set; }


        public Statistic()
        {
            start_all = new DateTime();
            end_all = new DateTime();
            allWorkTime = new TimeSpan();
            usefulWorkTime = new TimeSpan();
            start_useful = new DateTime();
            end_useful = new DateTime();
            readWorkTime = new TimeSpan();
            start_read = new DateTime();
            end_read = new DateTime();

            currentUseFul_start = new DateTime();
            currentUseFul_end = new DateTime();

            CurrentUseFull = new TimeSpan();

            this.iterationRecords = new IntervalRecords_Dictionary();
        }

        public void AllWorkTime()
        {
            allWorkTime = this.end_all - this.start_all;
        }

        public void UsefulWorkTime()
        {
            usefulWorkTime += this.end_useful - this.start_useful;
            //Console.WriteLine(usefulWorkTime);
        }

        public void currentTime(int iteration)
        {
            TimeSpan CurrentAllTime = DateTime.Now - this.start_all;
            iterationRecords.Add(iteration, "allWorkTime", CurrentAllTime); //Записываем текущее общее время работы
            iterationRecords.Add(iteration, "usefulWorkTime", usefulWorkTime); //Записываем текущее общее время работы

        }

        public void ReadTime(){
            readWorkTime = this.end_read - this.start_read;
        }

        public void AddReadTime(string type)
        {
            switch (type)
            {
                case "all":
                    allWorkTime +=  2* readWorkTime;
                    break;
                case "useful":
                    usefulWorkTime += readWorkTime;
                    break;
                default:
                    break;
            }
        }

        public void Clear()
        {
            start_all = new DateTime();
            end_all = new DateTime();
            allWorkTime = new TimeSpan();
            usefulWorkTime = new TimeSpan();
            start_useful = new DateTime();
            end_useful = new DateTime();
            readWorkTime = new TimeSpan();
            start_read = new DateTime();
            end_read = new DateTime();

            currentUseFul_start = new DateTime();
            currentUseFul_end = new DateTime();

            CurrentUseFull = new TimeSpan();

            this.iterationRecords = new IntervalRecords_Dictionary();
        }
    }
}
