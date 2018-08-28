using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Notification
    {
        [JsonProperty("jsonrpc")]
        public string ProtocolVersion { get; } = "2.0";

        public string Method { get; set; }

        public object Params { get; set; }
    }
}
