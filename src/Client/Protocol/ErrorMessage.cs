using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client.Protocol
{
    /// <summary>
    ///     A JSON-RPC error message.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ErrorMessage
    {
        /// <summary>
        ///     The error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///     The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Optional data associated with the message.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JToken Data { get; set; }
    }
}
