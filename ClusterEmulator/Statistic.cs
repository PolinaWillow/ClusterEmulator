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
        }
    }
}
