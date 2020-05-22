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
        }

        public object Id { get; }
        public ErrorMessage Error { get; }
    }
}
