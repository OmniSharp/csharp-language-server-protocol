using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Server.Messages
{
    public interface IErrorMessage
    {
        int Code { get; }

        string Message { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        object Data { get; }
    }
}