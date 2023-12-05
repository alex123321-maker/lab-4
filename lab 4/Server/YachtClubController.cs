using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Server
{
    internal class YachtClubController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string filePath; // Путь к файлу CSV
        private List<YachtClub> yachtClubs;

        public YachtClubController(string filePath = "D:/учёба 5 сем/архетектуры ИС/lab_2/yachtclubs.csv")
        {
            Logger.Info(filePath);
            this.filePath = filePath;
        }

        // Метод для чтения всех записей из файла и возврата списка объектов YachtClub
        public List<YachtClub> ReadAllRecords()
        {
            yachtClubs = new List<YachtClub>();

            try
            {
                if (!File.Exists(filePath))
                {
                    WriteRecords();
                    Console.WriteLine("No");
                }
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');

                        // Проверяем, что данные в строке корректны, иначе пропускаем строку
                        if (data.Length >= 5)
                        {
                            YachtClub club = new YachtClub
                            {
                                Name = data[0],
                                Address = data[1],
                                NumberOfYachts = int.Parse(data[2]),
                                NumberOfPlaces = int.Parse(data[3]),
                                HasPool = bool.Parse(data[4])
                            };

                            yachtClubs.Add(club);
                        }
                    }
                }
                Logger.Info("Файл прочитан", yachtClubs);
            }
            catch (IOException e)
            {
                Logger.Error(e.ToString());
            }
            return yachtClubs;
        }

        // Метод для записи списка объектов YachtClub в файл CSV
        public void WriteRecords()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (YachtClub club in yachtClubs)
                    {
                        string line = $"{club.Name},{club.Address},{club.NumberOfYachts},{club.NumberOfPlaces},{club.HasPool}";
                        writer.WriteLine(line);
                    }
                }
                Logger.Info("Запись в файл завершена");
            }
            catch (IOException e)
            {
                Logger.Error($"Ошибка записи в файл: {e.Message}");
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            
        }

        // Метод для добавления новой записи в список и сохранения в файле
        public void AddRecord(YachtClub club)
        {
            
            yachtClubs.Add(club);
        }

        // Метод для удаления записи по индексу
        public void RemoveRecord(int index)
        {
            
            if (index >= 0 && index < yachtClubs.Count)
            {
                yachtClubs.RemoveAt(index);
            }
            else
            {
                Logger.Error("Неверный индекс для удаления записи.");
                throw new IndexOutOfRangeException();
            }
        }
        public List<YachtClub> GetYachtClubs()
        {
            return yachtClubs;
        }
        public YachtClub GetYachtClub(int index)
        {
            return yachtClubs.ElementAt(index);
        }
        
    }
}
