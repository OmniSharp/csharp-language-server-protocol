using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Notification : IMethodWithParams
    {
        internal Notification(
            string method,
            JToken @params,
            string protcolVersion)
        {
            ProtocolVersion = protcolVersion;
            Method = method;
            Params = @params;
        }

        internal Notification(string method, JToken @params) : this(method, @params, "2.0") { }

        [JsonProperty("jsonrpc")]
        public string ProtocolVersion { get; }

        public string Method { get; }

        public JToken Params { get; }
    }
}
