﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator
{
    internal class ClusterInfo
    {
        public int PORT { get; set; }
        public string IP { get; set; }

        public ClusterInfo()
        {
            PORT = 2000;
            IP = "192.168.1.73";
        }
    }
}
