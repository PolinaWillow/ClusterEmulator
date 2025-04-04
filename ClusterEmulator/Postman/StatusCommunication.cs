﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator.Postman
{
    internal class StatusCommunication
    {
        /// <summary>
        /// Ствтус соединения с кластером
        /// </summary>
        public string Status { get; set; }
        public int timeDelayStatus { get; set; }

        public int threadAgentCount { get; set; }

        public int PakegeSize { get; set; }

        /// <summary>
        /// Изменение статуса
        /// </summary>
        /// <param name="newStatus"></param>
        public void Set(string newStatus)
        {
            this.Status = newStatus;
        }

        /// <summary>
        /// Получение типа данных кдасса
        /// </summary>
        /// <returns></returns>
        public string TypeOf()
        {
            return "StatusCommunication";
        }
    }
}
