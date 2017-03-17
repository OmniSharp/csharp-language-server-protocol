using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonRpc.Server
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Response
    {
        internal Response(object id) : this(id, null)
        {
        }

        internal Response(object id, object result)
        {
            Id = id;
            Result = result;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        public object Result { get; set; }
    }
}