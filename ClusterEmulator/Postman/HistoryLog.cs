using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator.Postman
{
    public class HistoryLog
    {
        public int timeDelay { get; set; }
        public int threadAgentCount { get; set; }

        public HistoryLog() { 
            this.timeDelay = 0;
            this.threadAgentCount = 0;
        }
    }
}
