using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Text.Json;
using ClusterEmulator.Postman;
using System.Windows.Input;
using ClusterEmulator.Postman.Calculation;
using System.Net.WebSockets;

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
                Socket newSocket = socket.Accept(); //Новое подключение к сокету
                MemoryStream memoryStream = new MemoryStream();


                statistic.start_useful = DateTime.Now; //Сбор статистики по полезной нагрузке

                byte[] buffer = new byte[1024]; //Чтение и конвертация данных из сокета
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
                
                byte[] totalBytes = memoryStream.ToArray();
                memoryStream.Close();
                string readData = Encoding.Default.GetString(totalBytes);

                //Преобразование данных в Sender
                Sender data = JsonSerializer.Deserialize<Sender>(readData);  
                
                switch (data.Header)
                {
                    case "StatusCommunication":
                        Console.WriteLine("Поступил запрос от StatusCommunication");
                        Console.WriteLine(data.Body);
                        if (data.Body == "start")
                        {
                            Console.WriteLine("Поступил запрос на начало сессии");
                            statistic.start_all = DateTime.Now;
                            Console.WriteLine("start_all: " + statistic.start_all);
                        }
                        if (data.Body == "end")
                        {
                            Console.WriteLine("Поступил запрос на конец сессии");
                            Console.WriteLine(count);
                            statistic.end_all = DateTime.Now;
                            statistic.AllWorkTime();
                            Console.WriteLine("UsefulWorkTime: " + statistic.usefulWorkTime);
                            Console.WriteLine("AllWorkTime: " + statistic.allWorkTime);
                        }

                        Sender res = new Sender();
                        res.AddData("bool", "true");
                        SendResponse(newSocket, res);

                        break;

                    case "Calculation":
                        //Console.WriteLine("Поступил запрос на расчет данных");
                        Calculation inputData = JsonSerializer.Deserialize<Calculation>(data.Body);       

                        //Подсчет значения целевой функции
                        double resultFunction = FindValueOfFunction(inputData.Way_For_Send);

                        Sender resultCalculat = new Sender();
                        resultCalculat.AddData("double", resultFunction.ToString());

                        SendResponse(newSocket, resultCalculat);

                        break;
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

        public static void SendResponse(Socket socket, Sender res)
        {
            string dataToSend = JsonSerializer.Serialize(res);
            byte[] dataToSendBytes = Encoding.Default.GetBytes(dataToSend);
            socket.Send(dataToSendBytes);

            socket.Close();
        }
    }
}