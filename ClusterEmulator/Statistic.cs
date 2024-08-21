using System;
using System.Collections.Generic;
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


        public Statistic()
        {
            start_all = new DateTime();
            end_all = new DateTime();
            allWorkTime = new TimeSpan();
            usefulWorkTime = new TimeSpan();
            start_useful = new DateTime();
            end_useful = new DateTime();
        }

        public void AllWorkTime()
        {
            allWorkTime = this.end_all - this.start_all;
        }

        public void UsefulWorkTime()
        {
            usefulWorkTime +=this.end_useful - this.start_useful;
            //Console.WriteLine(usefulWorkTime);
        }

        public void Clear()
        {
            start_all = new DateTime();
            end_all = new DateTime();
            allWorkTime = new TimeSpan();
            usefulWorkTime = new TimeSpan();
            start_useful = new DateTime();
            end_useful = new DateTime();
        }
    }
}
