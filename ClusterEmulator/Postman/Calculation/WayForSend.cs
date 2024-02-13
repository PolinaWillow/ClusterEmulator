using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator.Postman.Calculation
{
    public class WayForSend
    {
        public string SendValue { get; set; }

        public string ValueType { get; set; }

        public WayForSend()
        {
            SendValue = "";
            ValueType = "";
        }
    }
}
