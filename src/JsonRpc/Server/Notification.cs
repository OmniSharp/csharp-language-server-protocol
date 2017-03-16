using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonRPC.Server
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Notification
    {
        internal Notification(
            string method,
            JContainer @params,
            string protcolVersion)
        {
            ProtocolVersion = protcolVersion;
            Method = method;
            Params = @params;
        }

        internal Notification(string method, JContainer @params) : this(method, @params, "2.0") { }

        public string ProtocolVersion { get; }

        public string Method { get; }

        public JContainer Params { get; }
    }
}