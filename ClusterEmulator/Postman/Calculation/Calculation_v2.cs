using ClusterEmulator.Postman.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class Calculation_v2
    {
        /// <summary>
        /// Путь агента для отправления
        /// </summary>
        public WayForSend[] Way_For_Send { get; set; }

        /// <summary>
        /// Id агента
        /// </summary>
        public string idAgent { get; set; } //Id агента

        /// <summary>
        /// результат расчета
        /// </summary>
        public double result { get; set; } //результат расчета

        public Calculation_v2()
        {
            
        }

        public string TypeOf()
        {
            return "Calculation_v2";
        }

    }
}
