using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public class ErrorMessage: ErrorMessage<object>
    {
        public ErrorMessage(int code, string message) : base(code, message, null)
        {
        }

        [JsonConstructor]
        public ErrorMessage(int code, string message, object data) : base(code, message, data)
        {
        }
    }

    public class ErrorMessage<T> : IErrorMessage
    {
        [JsonConstructor]
        public ErrorMessage(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public int Code { get; }

        public string Message { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; }

        object IErrorMessage.Data => Data;
    }
}
