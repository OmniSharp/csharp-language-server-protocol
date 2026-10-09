using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public interface IErrorMessage
    {
        int Code { get; }

        string Message { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        object? Data { get; }
    }
}
