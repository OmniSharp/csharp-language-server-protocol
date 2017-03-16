using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonRPC.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Request
    {
        public object Id { get; set; }

        public string ProtocolVersion { get; set; } = "2.0";

        public string Method { get; set; }

        public object Params { get; set; }
    }
}