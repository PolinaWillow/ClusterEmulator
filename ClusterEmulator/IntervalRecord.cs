using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator
{
    class IntervalRecord
    {
        public string interval { get; set; }
        public TimeSpan current_allWorkTime { get; set;}
        public TimeSpan current_usefulWorkTime { get; set; }

        public IntervalRecord(int interval)
        {
            this.interval = Convert.ToString(interval);
            this.current_allWorkTime = new TimeSpan();
            this.current_usefulWorkTime = new TimeSpan();
        }

        public void Add(string typeTime, TimeSpan curentTime)
        {
            switch (typeTime)
            {
                case "allWorkTime":
                    this.current_allWorkTime = curentTime;
                    break;
                case "usefulWorkTime":
                    this.current_usefulWorkTime = curentTime;
                    break;
            }
        }
    }
}
