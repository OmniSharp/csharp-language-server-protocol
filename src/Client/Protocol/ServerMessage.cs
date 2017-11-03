using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Protocol
{
    /// <summary>
    ///     The server-side representation of an LSP message.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ServerMessage
    {
        /// <summary>
        ///     The JSON-RPC protocol version.
        /// </summary>
        public string ProtocolVersion { get; set; } = "2.0";

        /// <summary>
        ///     The request / response Id, if the message represents a request or a response.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Id { get; set; }

        /// <summary>
        ///     The JSON-RPC method name.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        ///     The request / notification message, if the message represents a request or a notification.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JObject Params { get; set; }

        /// <summary>
        ///     The response message, if the message represents a response.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JObject Result { get; set; }

        /// <summary>
        ///     The response error (if any).
        /// </summary>
        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ErrorMessage Error { get; set; }
    }
}
