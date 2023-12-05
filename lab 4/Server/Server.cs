using Client;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            Server server = new Server(8001);
            Console.WriteLine("Сервер запущен. Для выхода нажмите Enter.");

            Console.ReadLine(); // Ждем, пока пользователь нажмет Enter, прежде чем завершить программу

        }
    }
    class Server
    {
        private YachtClubBDController yachtClubController;
        private int port;
        private UdpClient listener;
        private readonly Logger Logger;

        public Server(int _port)
        {
            Logger = LogManager.GetCurrentClassLogger();
            port = _port;
            listener = new UdpClient(_port);
            yachtClubController = new YachtClubBDController();
            Logger.Info("Сервер запущен.");

            Task.Run(() => StartListenAsync());
        }

        private async Task StartListenAsync()
        {

            while (true)
            {
                try
                {
                    UdpReceiveResult result = await listener.ReceiveAsync();

                    if (result.Buffer != null && result.Buffer.Length > 0)
                    {
                        ProcessRequest(result.Buffer, result.RemoteEndPoint);
                    }
                }

                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                    await Console.Out.WriteLineAsync("Error");
                    await Console.Out.WriteLineAsync(ex.ToString());
                }
            }
        }


        private void ProcessRequest(byte[] requestData, IPEndPoint clientEndPoint)
        {
            StringBuilder message = new StringBuilder("Запрос получен"); ;
            ResponseType responseType = ResponseType.Success;
            // Десериализация запроса
            string jsonRequest = Encoding.UTF8.GetString(requestData);
            Request request = Request.Deserialize(jsonRequest);

            // Обработка запроса (ваша логика здесь)
            // ...
            Console.Out.WriteLineAsync($"{request.MessageType}:{request.Message}");
            switch (request.MessageType)
            {
                case RequestType.Delete:
                    try
                    {
                        if (request.Parametrs.TryGetValue("Index", out string indexString) && int.TryParse(indexString, out int index))
                        {
                            Logger.Info("Запись удалена.");

                            var mes = yachtClubController.RemoveRecord(index);
                            message = new StringBuilder(mes);
                            responseType = ResponseType.Success;
                        }
                        else
                        {
                            Logger.Error($"Ошибка: Некорректный ID. Индекс = {indexString}");

                            message = new StringBuilder("Ошибка: Некорректный ID.");
                            responseType = ResponseType.Error;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex}");
                        responseType = ResponseType.Error;
                    }
                    break;
                case RequestType.GetAll:
                    try
                    {
                        message = new StringBuilder();
                        var list = yachtClubController.GetYachtClubs();
                        message = new StringBuilder(list);
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex}");
                        responseType = ResponseType.Error;
                    }

                    break;
                case RequestType.GetOne:
                    try
                    {
                        if (request.Parametrs.TryGetValue("Index", out string indexString) && int.TryParse(indexString, out int index))
                        {
                            var el = yachtClubController.GetYachtClub(index);
                            message = new StringBuilder(el);
                            responseType = ResponseType.Success;
                        }
                        else
                        {
                            Logger.Error($"Ошибка: Некорректный ID. Индекс = {indexString}");

                            message = new StringBuilder("Ошибка: Некорректный ID.");
                            responseType = ResponseType.Error;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex}");
                        responseType = ResponseType.Error;
                    }
                    break;
                case RequestType.Post:
                    try
                    {
                        string name, address;
                        int numberOfYachts, numberOfPlaces;
                        bool hasPool;

                        if (request.Parametrs.TryGetValue("Name", out name) &&
                            request.Parametrs.TryGetValue("Address", out address) &&
                            request.Parametrs.TryGetValue("NumberOfYachts", out string numberOfYachtsStr) &&
                            int.TryParse(numberOfYachtsStr, out numberOfYachts) &&
                            request.Parametrs.TryGetValue("NumberOfPlaces", out string numberOfPlacesStr) &&
                            int.TryParse(numberOfPlacesStr, out numberOfPlaces) &&
                            request.Parametrs.TryGetValue("HasPool", out string hasPoolStr) &&
                            bool.TryParse(hasPoolStr, out hasPool))
                        {
                            YachtClub newYachtClub = new YachtClub
                            {
                                Name = name,
                                Address = address,
                                NumberOfYachts = numberOfYachts,
                                NumberOfPlaces = numberOfPlaces,
                                HasPool = hasPool
                            };
                            var mes = yachtClubController.AddRecord(newYachtClub);
                            Logger.Info("Запись добавлена.");

                            message = new StringBuilder(mes);
                            responseType = ResponseType.Success;
                            
                        }
                        else
                        {
                            Logger.Error($"Ошибка: Некорректные параметры запроса. {request.Parametrs.ToString()}");
                            message = new StringBuilder("Ошибка: Некорректные параметры запроса.");
                            responseType = ResponseType.Error;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex}");
                        responseType = ResponseType.Error;
                    }
                    break;
                case RequestType.Update:
                    try
                    {
                        if (request.Parametrs.TryGetValue("Id", out string idString) && int.TryParse(idString, out int id) &&
                            request.Parametrs.TryGetValue("Name", out string name) &&
                            request.Parametrs.TryGetValue("Address", out string address) &&
                            request.Parametrs.TryGetValue("NumberOfYachts", out string numberOfYachtsStr) &&
                            int.TryParse(numberOfYachtsStr, out int numberOfYachts) &&
                            request.Parametrs.TryGetValue("NumberOfPlaces", out string numberOfPlacesStr) &&
                            int.TryParse(numberOfPlacesStr, out int numberOfPlaces) &&
                            request.Parametrs.TryGetValue("HasPool", out string hasPoolStr) &&
                            bool.TryParse(hasPoolStr, out bool hasPool))
                        {
                            YachtClub updatedYachtClub = new YachtClub
                            {
                                Name = name,
                                Address = address,
                                NumberOfYachts = numberOfYachts,
                                NumberOfPlaces = numberOfPlaces,
                                HasPool = hasPool
                            };
                            yachtClubController.UpdateRecord(id, updatedYachtClub);
                            Logger.Info("Запись обновлена.");

                            message = new StringBuilder("Запись успешно обновлена.");
                            responseType = ResponseType.Success;
                        }
                        else
                        {
                            Logger.Error("Ошибка: Некорректные параметры запроса.");
                            message = new StringBuilder("Ошибка: Некорректные параметры запроса.");
                            responseType = ResponseType.Error;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex.Message}");
                        responseType = ResponseType.Error;
                    }
                    break;
                case RequestType.UpdateTable:
                    try
                    {
                        var list = new List<YachtClub>();
                        list = JsonConvert.DeserializeObject<List<YachtClub>>(request.Message);
                        var response = yachtClubController.DeleteAndLoadData(list);
                        message = new StringBuilder(response);
                        responseType = ResponseType.Success;    
                    }
                    catch (Exception ex){
                        Logger.Fatal(ex);
                        message = new StringBuilder($"Ошибка: {ex}");
                        responseType = ResponseType.Error;
                    }
                    break;
                case RequestType.Menu:
                    message = new StringBuilder("Список команд:\n- delete\n- getAll\n- getOne\n- post\n- exit");
                    break;
                case RequestType.Uncorrect:
                    responseType = ResponseType.Error;
                    message = new StringBuilder("неверная команда");
                    break;
            }
            ServerResponse(message,responseType,clientEndPoint);
        }
        private void ServerResponse(StringBuilder message,ResponseType respType,IPEndPoint clientEndPoint)
        {
            // Подготовка ответа
            Response response = new Response(message.ToString(), respType);
            string jsonResponse = response.Serialize();
            byte[] responseData = Encoding.UTF8.GetBytes(jsonResponse);

            // Отправка ответа асинхронно
            listener.SendAsync(responseData, responseData.Length, clientEndPoint);
        }
    }
}
