using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Request
    {
        public object Id { get; set; }

        public string ProtocolVersion { get; } = "2.0";

        public string Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Params { get; set; }
    }
}
