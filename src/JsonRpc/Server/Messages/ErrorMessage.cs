using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ErrorMessage : IErrorMessage
    {
        public ErrorMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }

        [JsonConstructor]
        public ErrorMessage(int code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public int Code { get; }

        public string Message { get; }

        public object Data { get; }

        object IErrorMessage.Data => Data;
    }
}
