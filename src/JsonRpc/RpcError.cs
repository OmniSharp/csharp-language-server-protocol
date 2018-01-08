using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{

    [JsonConverter(typeof(RpcErrorConverter))]
    public class RpcError<T>
    {
        public RpcError(object id, ErrorMessage<T> message) : this(id, message, "2.0")
        {
        }

        [JsonConstructor]
        public RpcError(object id, ErrorMessage<T> message, string protocolVersion)
        {
            Id = id;
            Error = message;
            ProtocolVersion = protocolVersion;
        }

        public string ProtocolVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Id { get; }

        public ErrorMessage<T> Error { get; }
    }

    public class RpcError : RpcError<object>
    {
        public RpcError(object id, ErrorMessage<object> message) : this(id, message, "2.0")
        {
        }

        [JsonConstructor]
        public RpcError(object id, ErrorMessage<object> message, string protocolVersion) : base(id, message, protocolVersion)
        {
        }
    }
}
