using ClusterEmulator.Postman.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator
{
    public class ClusterFunctions
    {
        /// <summary>
        /// Мульти-функция
        /// </summary>
        /// <param name="agentWay"></param>
        /// <returns></returns>
        public double MultiFunction(WayForSend[] agentWay)
        {
            double x = Convert.ToDouble(agentWay[0].SendValue);
            double y = Convert.ToDouble(agentWay[1].SendValue);

            double result = x * Math.Sin(4 * Math.PI * x) + y * Math.Sin(4 * Math.PI * y); //Math.PI
            return result;
        }

        /// <summary>
        /// Функция Швефеля
        /// </summary>
        /// <param name="agentWay"></param>
        /// <returns></returns>
        public double SchwefelFunction(WayForSend[] agentWay)
        {
            double x = Convert.ToDouble(agentWay[0].SendValue);
            double y = Convert.ToDouble(agentWay[1].SendValue);

            //Console.WriteLine(x);
            //Console.WriteLine(y);

            double result = -Math.Pow(x, 2)-Math.Pow((x+y), 2);
            //Console.WriteLine(result);
            return result;

        }
        public ClusterFunctions() { }



    }
}
