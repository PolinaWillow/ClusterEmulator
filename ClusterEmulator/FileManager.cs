using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterEmulator
{
    internal class FileManager
    {
        public string CreateFileName(string nameFile, string typeName = null)
        {
            if (typeName == "Data")
            {
                //Получение текущей даты
                DateTime today = DateTime.Now;

                //Форматирование даты
                string createData = Convert.ToString(today);
                createData = createData.Replace(" ", "_");
                createData = createData.Replace(":", "-");
                createData = createData.Replace(".", "-");
                //Формирование имени
                string fileName = /*"../../../../OutputResultFiles/"+*/  nameFile + "_" + createData + ".txt";
                return fileName;
            }
            else
            {
                //Формирование имени
                string fileName = /*"../../../../OutputResultFiles/"+*/  nameFile + ".txt";
                return fileName;
            }


        }


        public string CreateStatisticFile(string name, string comment = null)
        {
            string outputDataFile = CreateFileName(name);

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);
            using (StreamWriter sw = File.CreateText(outputDataFile)/*fileInf.CreateText()*/)
            {
                sw.WriteLine("Результаты сбора статистики по времени работы основных модулей:");
                if (comment != null)
                {
                    sw.WriteLine("Комментарии:");
                    sw.WriteLine(comment);
                    sw.WriteLine("-------------------------------------------------------------\n");
                }
                sw.WriteLine("UsefulWorkTime \t AllWorkTime");
                sw.Close();
            }
            return outputDataFile;
        }

        /// <summary>
        /// Запись строки в файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="writeString">Записываемая строка</param>
        public void Write(string fileName, string writeString)
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                //Запись результата              
                sw.WriteLine(writeString);
                sw.Close();
            }
        }
    }
}
