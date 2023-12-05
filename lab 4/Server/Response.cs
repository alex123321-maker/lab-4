using Newtonsoft.Json;
using System;

namespace Server
{

    [Serializable]
    public class Response
    {
        public string Message { get; set; }
        public ResponseType MessageType { get; set; }

        public Response(string message, ResponseType type)
        {
            Message = message;
            MessageType = type;
        }

        // Метод для десериализации из JSON строки
        public static Response Deserialize(string jsonData)
        {
            return JsonConvert.DeserializeObject<Response>(jsonData);
        }

        // Метод для сериализации в JSON строку
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public enum ResponseType
    {
        Success,
        Error
    }

}

