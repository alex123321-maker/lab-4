using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{


    [Serializable]
    public class Request
    {
        public string Message { get; set; }
        public RequestType MessageType { get; set; }
        public Dictionary<string,string> Parametrs { get; set; }

        public Request(string message, RequestType type, Dictionary<string, string> parametrs)
        {
            Parametrs = parametrs;
            Message = message;
            MessageType = type;
        }

        // Метод для десериализации из JSON строки
        public static Request Deserialize(string jsonData)
        {
            return JsonConvert.DeserializeObject<Request>(jsonData);
        }

        // Метод для сериализации в JSON строку
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    public enum RequestType
    {
        Uncorrect,
        Menu,
        Delete,
        GetAll,
        GetOne,
        Update,
        UpdateTable,
        Post
    }
}

