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
using System.Threading.Tasks;

namespace ClusterEmulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Statistic statistic = new Statistic();
            FileManager fileManager = new FileManager();
            string fileName = fileManager.CreateStatisticFile("log");

            int timeDelay = 0; //Задержка отправления ответа

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

               //Console.WriteLine(readData);

                //Преобразование данных в Sender
                Sender data = JsonSerializer.Deserialize<Sender>(readData);
                data.Print();


                switch (data.Header)
                {
                    case "StatusCommunication":
                        //Console.WriteLine("Поступил запрос от StatusCommunication");
                        //Console.WriteLine(data.Body);
                        StatusCommunication body = JsonSerializer.Deserialize<StatusCommunication>(data.Body);
                        //Console.WriteLine(status);
                        if (body.Status == "start")
                        {
                            Console.WriteLine("Поступил запрос на начало сессии");
                            Console.WriteLine(data.Header);
                            statistic.start_all = DateTime.Now;
                            Console.WriteLine("start_all: " + statistic.start_all);
                            timeDelay = body.timeDelayStatus;
                            string writeStr = "\nЗапуск от "+ DateTime.Now +"\nКоличество потоков агентов "+body.threadAgentCount + "\nЗадержка: " + timeDelay;
                            fileManager.Write(fileName, writeStr);

                        }
                        if (body.Status == "end")
                        {
                            Console.WriteLine("Поступил запрос на конец сессии");
                            Console.WriteLine(count);
                            statistic.end_all = DateTime.Now;
                            statistic.AllWorkTime();
                            Console.WriteLine("UsefulWorkTime: " + statistic.usefulWorkTime);
                            Console.WriteLine("AllWorkTime: " + statistic.allWorkTime);

                            //Запись данных в файл
                            string writeStr = fileManager.FormatTime(statistic.usefulWorkTime) + "\t" + fileManager.FormatTime(statistic.allWorkTime);
                            fileManager.Write(fileName, writeStr);

                            //Очистка статистики
                            statistic.Clear();

                        }

                        Sender res = new Sender();
                        res.AddData("bool", "true");
                        SendResponse(newSocket, res);

                        break;

                    case "Calculation":
                        statistic.start_useful = DateTime.Now; //Сбор статистики по полезной нагрузке

                        //Задержка в мс от 0,1с до 1с
                        Task.Delay(timeDelay);

                        //Console.WriteLine("Поступил запрос на расчет данных");
                        Calculation inputData = JsonSerializer.Deserialize<Calculation>(data.Body);

                        //Подсчет значения целевой функции
                        ClusterFunctions clusterFunctions = new ClusterFunctions();
                        double resultFunction = clusterFunctions.MultiFunction(inputData.Way_For_Send);
                        //double resultFunction = clusterFunctions.SchwefelFunction(inputData.Way_For_Send);
                        //Console.WriteLine(resultFunction);

                        Sender resultCalculat = new Sender();
                        resultCalculat.AddData("double", resultFunction.ToString());
                        //resultCalculat.Print();

                        SendResponse(newSocket, resultCalculat);

                        statistic.end_useful = DateTime.Now;
                        statistic.UsefulWorkTime();
                        break;
                }
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