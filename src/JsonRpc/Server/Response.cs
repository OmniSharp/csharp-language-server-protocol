using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonRPC.Server
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Response<T>
    {
        internal Response()
        {
        }

        public string ProtocolVersion { get; set; } = "2.0";

        public object Id { get; set; }

        public T Result { get; set; }
    }
}