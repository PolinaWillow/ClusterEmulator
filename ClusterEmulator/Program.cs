using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Text.Json;

namespace ClusterEmulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Statistic statistic = new Statistic();

            //Получение данных через сокет
            ClusterInfo clasterInfo = new ClusterInfo();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any/*clasterInfo.IP*/, clasterInfo.PORT);
            socket.Bind(endPoint);
            socket.Listen(100000000);

            int count = 0;

            while (true)
            {

                //Console.WriteLine("waiting for new connection...");
                Socket newSocket = socket.Accept();
                MemoryStream memoryStream = new MemoryStream();
                //Console.WriteLine("new connection...");

                statistic.start_useful = DateTime.Now;

                byte[] buffer = new byte[1024];
                int readBytes = newSocket.Receive(buffer);
                while (readBytes > 0)
                {
                    memoryStream.Write(buffer, 0, readBytes);
                    if (socket.Available > 0)
                    {
                        readBytes = newSocket.Receive(buffer);
                    }
                    else
                    {
                        break;
                    }
                }
                //Console.WriteLine("data received...");
                byte[] totalBytes = memoryStream.ToArray();
                memoryStream.Close();
                string readData = Encoding.Default.GetString(totalBytes);

                //Преобразование входной строки json в массив данных
                //Console.WriteLine(readData);

                try
                {
                    InputData inputData = JsonSerializer.Deserialize<InputData>(readData);
                    //for (int i = 0; i < inputData.Way_For_Send.Length; i++) {

                    //    Console.Write(inputData.Way_For_Send[i].SendValue+"-"+ inputData.Way_For_Send[i].ValueType+"; ");
                    //}
                    //Console.WriteLine();

                    //Подсчет значения целевой функции
                    double resultFunction = 0;
                    if (inputData.TypeSendData == "findValue")
                    {
                        resultFunction = FindValueOfFunction(inputData.Way_For_Send);
                    }


                    string dataToSend = JsonSerializer.Serialize(resultFunction);
                    byte[] dataToSendBytes = Encoding.Default.GetBytes(dataToSend);
                    newSocket.Send(dataToSendBytes);
                    newSocket.Close();
                    //Console.WriteLine("data sent...");
                }
                catch (Exception ex)
                {
                    string comand = JsonSerializer.Deserialize<string>(readData);
                    if(comand == "start")
                    {
                        statistic.start_all = DateTime.Now;
                        Console.WriteLine("start_all: " + statistic.start_all);
                    }
                    if(comand == "end")
                    {
                        Console.WriteLine(count);
                        statistic.end_all = DateTime.Now;
                        statistic.AllWorkTime();
                        Console.WriteLine("UsefulWorkTime: " + statistic.usefulWorkTime);
                        Console.WriteLine("AllWorkTime: " + statistic.allWorkTime);
                    }

                    string dataToSend = JsonSerializer.Serialize(true);
                    byte[] dataToSendBytes = Encoding.Default.GetBytes(dataToSend);
                    newSocket.Send(dataToSendBytes);
                    newSocket.Close();
                }

                statistic.end_useful = DateTime.Now;
                statistic.UsefulWorkTime();


                count++;

            }
        }

        public static double FindValueOfFunction(WayForSend[] agentWay)
        {
            double Value = Convert.ToDouble(agentWay[0].SendValue) - Convert.ToDouble(agentWay[1].SendValue) + 2 * Convert.ToDouble(agentWay[2].SendValue) + Convert.ToDouble(agentWay[3].SendValue) + 2 * Convert.ToDouble(agentWay[4].SendValue) + 0.5 * Convert.ToDouble(agentWay[5].SendValue) - 0.12 * Convert.ToDouble(agentWay[6].SendValue) - Convert.ToDouble(agentWay[7].SendValue) + 80 * Convert.ToDouble(agentWay[8].SendValue) + 0.00001 * Convert.ToDouble(agentWay[9].SendValue);

            if (string.Compare(Convert.ToString(agentWay[10].SendValue), "Сильное") == 0)
            {
                Value += 20;
            }
            return Value;
        }
    }
}