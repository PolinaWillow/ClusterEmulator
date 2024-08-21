using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator.Postman
{
    internal class Sender
    {
        /// <summary>
        /// Заголовок запроса
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Тело запроса
        /// </summary>
        public string Body { get; set; }


        public Sender()
        {
            Header = ""; //typeof(Sender);
            Body = "";
        }

        /// <summary>
        /// Добавление параметров для запроса на кластер
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void AddData(string header, string body)
        {
            Header = header;
            Body = body;
        }

        /// <summary>
        /// Функция возвращающая тип класса в виде строки
        /// </summary>
        /// <returns></returns>
        public string TypeOf()
        {
            return "Sender";
        }

        public void Print()
        {
            Console.WriteLine("{\n\tHeder: " + this.Header + "\n\tBody: " + this.Body + "\n}");
        }
    }
}
