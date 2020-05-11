using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client.Protocol
{
    /// <summary>
    ///     The server-side representation of an LSP message.
    /// </summary>
    public class ServerMessage
    {
        /// <summary>
        ///     The JSON-RPC protocol version.
        /// </summary>
        [JsonProperty("jsonrpc")]
        public string ProtocolVersion { get; set; } = "2.0";

        /// <summary>
        ///     The request / response Id, if the message represents a request or a response.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public object Id { get; set; }

        /// <summary>
        ///     The JSON-RPC method name.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        ///     The request / notification message, if the message represents a request or a notification.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public JToken Params { get; set; }

        /// <summary>
        ///     The response message, if the message represents a response.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public JToken Result { get; set; }

        /// <summary>
        ///     The response error (if any).
        /// </summary>
        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ErrorMessage Error { get; set; }
    }
}
