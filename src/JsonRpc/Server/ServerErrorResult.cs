using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class ServerErrorResult
    {
        [JsonConstructor]
        public ServerErrorResult(int code, string? message, JsonElement? data)
        {
            Code = code;
            Message = message ?? string.Empty;
            Data = data;
        }

        public ServerErrorResult(int code, string? message)
        {
            Code = code;
            Message = message ?? string.Empty;
            Data = JsonSerializer.SerializeToElement(new { });
        }

        public int Code { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonElement? Data { get; set; }
    }
}
