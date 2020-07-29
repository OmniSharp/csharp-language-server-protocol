using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RpcError
    {
        [JsonConstructor]
        public RpcError(object id, ErrorMessage message)
        {
            Id = id;
            Error = message;
            Method = string.Empty;
        }

        public RpcError(object id, string method, ErrorMessage message)
        {
            Id = id;
            Error = message;
            Method = method ?? string.Empty;
        }

        public object Id { get; }
        public ErrorMessage Error { get; }

        [JsonIgnore]
        public string Method { get; }
    }
}
