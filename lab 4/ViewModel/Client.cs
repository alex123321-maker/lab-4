using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfApp1.commands;
using WpfApp1.ViewModels;

namespace Client
{
    class Client : BaseViewModel
    {
        private IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
        private UdpClient client;
        private ObservableCollection<YachtClub> yachtClubs;
        private string _serverResponse;

        private BaseCommand getAllCommand;
        private BaseCommand saveCommand;


        public BaseCommand GetAllCommand
        {
            get
            {
                return getAllCommand ?? (getAllCommand = new BaseCommand(obj =>
                {
                    SendRequestAsync(new Request(" ", RequestType.GetAll, new Dictionary<string, string>()));
                }));
            }
        }

        public BaseCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new BaseCommand(obj =>
                {
                    try
                    {
                        var jsonString = JsonConvert.SerializeObject(YachtClubs);
                        SendRequestAsync(new Request(jsonString, RequestType.UpdateTable, new Dictionary<string, string>()));

                    }
                    catch (Exception ex)
                    {
                        _serverResponse = $"Ошибка при обновлении записи: {ex.Message}";
                    }
                }));
            }
        }

        public ObservableCollection<YachtClub> YachtClubs
        {
            get { return yachtClubs; }
            set
            {
                Set(ref yachtClubs, value);
            }
        }
        public string ServerResponse
        {
            get => _serverResponse;
            set
            {
                Set(ref _serverResponse, value, nameof(_serverResponse));
            }
        }

        public Client()
        {
            yachtClubs = new ObservableCollection<YachtClub>();



            client = new UdpClient();

            Thread.Sleep(1000);
            client.Connect(serverEP);
        }




        private async Task SendRequestAsync(Request request)
        {
            string jsonRequest = request.Serialize();
            byte[] requestData = Encoding.UTF8.GetBytes(jsonRequest);
            await client.SendAsync(requestData, requestData.Length);
            await ReceiveResponseAsync();
        }

        private async Task ReceiveResponseAsync()
        {
            UdpReceiveResult result = await client.ReceiveAsync();
            string jsonResponse = Encoding.UTF8.GetString(result.Buffer);
            Response response = Response.Deserialize(jsonResponse);
            try
            {
                JToken jsonToken = JToken.Parse(response.Message);
                if (jsonToken is JArray)
                {
                    YachtClubs = new ObservableCollection<YachtClub>(jsonToken.ToObject<List<YachtClub>>());
                    // Обработка списка yachtClubs
                }
                else if (jsonToken is JObject)
                {
                    YachtClubs.Add(jsonToken.ToObject<YachtClub>());
                    // Обработка отдельного yachtClub
                }
            }
            catch (JsonReaderException ex)
            {
                ServerResponse = response.Message;
            }


        }
    }
};