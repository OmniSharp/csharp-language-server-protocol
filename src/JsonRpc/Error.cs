using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Error<T>
    {
        public Error(object id, ErrorMessage<T> message) : this(id, message, "2.0")
        {
        }

        [JsonConstructor]
        public Error(object id, ErrorMessage<T> message, string protocolVersion)
        {
            Id = id;
            Message = message;
            ProtocolVersion = protocolVersion;
        }

        public string ProtocolVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Id { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ErrorMessage<T> Message { get; }
    }

    public class Error : Error<object>
    {
        public Error(object id, ErrorMessage<object> message) : this(id, message, "2.0")
        {
        }

        [JsonConstructor]
        public Error(object id, ErrorMessage<object> message, string protocolVersion) : base(id, message, protocolVersion)
        {
        }
    }
}