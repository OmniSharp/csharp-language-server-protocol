using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public RpcError(object id, string command, ErrorMessage message)
        {
            Id = id;
            Command = command;
            Error = message;
        }

        public object Id { get; }
        public ErrorMessage Error { get; }
        public string Command { get; }
    }
}
