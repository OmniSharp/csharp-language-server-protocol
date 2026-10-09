using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
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

        [JsonPropertyName("code")]
        public int Code { get; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        object? IErrorMessage.Data => Data;
    }
}
