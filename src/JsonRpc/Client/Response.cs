using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Response
    {
        public Response(object id) : this(id, null)
        {
        }

        public Response(object id, object result)
        {
            Id = id;
            Result = result;
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        public object Result { get; set; }
    }
}