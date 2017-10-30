using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Request : IMethodWithParams
    {
        internal Request(object id, string method, JToken @params) : this(id, method, @params, "2.0") { }

        internal Request(
            object id,
            string method,
            JToken @params,
            string protocolVersion)
        {
            Id = id;
            ProtocolVersion = protocolVersion;
            Method = method;
            Params = @params;
        }

        public object Id { get; }

        public string ProtocolVersion { get; }

        public string Method { get; }

        public JToken Params { get; }
    }
}
