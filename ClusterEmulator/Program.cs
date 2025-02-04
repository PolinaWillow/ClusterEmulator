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
using System.Threading;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using System.Collections.Generic;

namespace ClusterEmulator
{
    internal class Program
    {
        static ParamsForTesting paramsForTesting = new ParamsForTesting();

        static void Main(string[] args)
        {
            Statistic statistic = new Statistic();
            HistoryLog historyLog = new HistoryLog();
            FileManager fileManager = new FileManager();
            string fileName = fileManager.CreateStatisticFile("log");

            

            //Получение данных через сокет
            ClusterInfo clasterInfo = new ClusterInfo();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any/*clasterInfo.IP*/, clasterInfo.PORT);
            socket.Bind(endPoint);
            socket.Listen(100000000);

            

            while (true)
            {
                Socket newSocket = socket.Accept(); //Новое подключение к сокету
                Thread clientThread = new Thread(() => Handler(newSocket, statistic, historyLog, fileManager, fileName));
                clientThread.Start();
               
            }
        }

        public static void Handler (Socket clientSocket, Statistic statistic, HistoryLog historyLog, FileManager fileManager, string fileName)
        {

            int timeDelay = 0; //Задержка отправления ответа
            int count = 0;

            while (true)
            {
                //Определяем сколько времени читаются данные
                statistic.start_read = DateTime.Now;

                MemoryStream memoryStream = new MemoryStream();

                byte[] buffer = new byte[1024 * 4]; //Чтение и конвертация данных из сокета
                int readBytes = clientSocket.Receive(buffer);
                while (readBytes > 0)
                {
                    memoryStream.Write(buffer, 0, readBytes);
                    if (clientSocket.Available > 0)
                    {
                        readBytes = clientSocket.Receive(buffer);
                    }
                    else
                    {
                        break;
                    }
                }

                byte[] totalBytes = memoryStream.ToArray();
                memoryStream.Close();
                string readData = Encoding.Default.GetString(totalBytes);

                statistic.end_read = DateTime.Now;
                statistic.ReadTime();

                //Console.WriteLine(readData);

                if (totalBytes.Length == 0)
                {
                    return;
                }
                
                //Преобразование данных в Sender
                Sender data = JsonSerializer.Deserialize<Sender>(readData);
                data.Print();


                ClusterFunctions clusterFunctions = new ClusterFunctions();

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

                            //Проверка, если задержка и количество потоков совпадает с предыдущим стартом, то запись не повторяем
                            if (!(historyLog.timeDelay == timeDelay) || !(historyLog.threadAgentCount == body.threadAgentCount))
                            {
                                string writeStr = "\nЗапуск от " + DateTime.Now + "\nКоличество потоков агентов " + body.threadAgentCount + "\nЗадержка: " + timeDelay;
                                fileManager.Write(fileName, writeStr);
                                historyLog.timeDelay = timeDelay;
                                historyLog.threadAgentCount = body.threadAgentCount;
                            }


                            Sender res = new Sender();
                            res.AddData("bool", "true");
                            SendResponse(clientSocket, res);
                        }
                        if (body.Status == "end")
                        {
                            Console.WriteLine("Поступил запрос на конец сессии");
                            Console.WriteLine(count);
                            statistic.end_all = DateTime.Now;
                            statistic.AllWorkTime();
                            statistic.AddReadTime("all");
                            Console.WriteLine("UsefulWorkTime: " + statistic.usefulWorkTime);
                            Console.WriteLine("AllWorkTime: " + statistic.allWorkTime);

                            //Запись данных в файл
                            string writeStr = fileManager.FormatTime(statistic.usefulWorkTime) + "\t" + fileManager.FormatTime(statistic.allWorkTime);
                            fileManager.Write(fileName, writeStr);

                            //Очистка статистики
                            statistic.Clear();

                            Sender res = new Sender();
                            res.AddData("bool", "true");
                            SendResponse(clientSocket, res);

                            //clientSocket.Close(); ;
                        }
                        break;

                    case "Calculation":
                        statistic.start_useful = DateTime.Now; //Сбор статистики по полезной нагрузке

                        //Задержка в мс от 0,1с до 1с
                        Task.Delay(timeDelay);

                        //Console.WriteLine("Поступил запрос на расчет данных");
                        Calculation inputData = JsonSerializer.Deserialize<Calculation>(data.Body);

                        //Подсчет значения целевой функции
                        double resultFunction;
                        switch (paramsForTesting.TargetFunction)
                        {
                            case "MultiFunction":
                                resultFunction = clusterFunctions.MultiFunction(inputData.Way_For_Send);
                                break;
                            case "SchwefelFunction":
                                resultFunction = clusterFunctions.SchwefelFunction(inputData.Way_For_Send);
                                break;
                            case "TwoExtremeFunction":
                                resultFunction = clusterFunctions.TwoExtremeFunction(inputData.Way_For_Send);
                                break;
                            default: resultFunction = 0.0; break;
                        }

                        //Console.WriteLine(resultFunction);

                        Sender resultCalculat = new Sender();
                        resultCalculat.AddData("double", resultFunction.ToString());
                        //resultCalculat.Print();

                        SendResponse(clientSocket, resultCalculat);

                        statistic.end_useful = DateTime.Now;
                        statistic.UsefulWorkTime();
                        statistic.AddReadTime("useful");
                        break;

                    case "MultyCalculation":
                        statistic.start_useful = DateTime.Now; //Сбор статистики по полезной нагрузке

                        //Задержка в мс от 0,1с до 1с
                        Task.Delay(timeDelay);

                        //Console.WriteLine("Поступил запрос на расчет данных");
                        MultyCalculation MultyCalculation_req = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                        //Подсчет значения целевой функции
                        
                        foreach (Calculation_v2 item in MultyCalculation_req.calculationList)
                        {
                            switch (paramsForTesting.TargetFunction)
                            {
                                case "MultiFunction":
                                    item.result = clusterFunctions.MultiFunction(item.Way_For_Send);
                                    break;
                                case "SchwefelFunction":
                                    item.result = clusterFunctions.SchwefelFunction(item.Way_For_Send);
                                    break;
                                case "TwoExtremeFunction":
                                    item.result = clusterFunctions.TwoExtremeFunction(item.Way_For_Send);
                                    break;
                                default: resultFunction = 0.0; break;
                            }
                        }
                        

                        Sender Calculat_res = new Sender();
                        Calculat_res.AddData(MultyCalculation_req.TypeOf(), MultyCalculation_req.GetJSON());
                        //resultCalculat.Print();

                        SendResponse(clientSocket, Calculat_res);

                        statistic.end_useful = DateTime.Now;
                        statistic.UsefulWorkTime();
                        statistic.AddReadTime("useful");
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

            //socket.Close();
        }
    }
}