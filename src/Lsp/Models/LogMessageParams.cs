using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class LogMessageParams
    {
        /// <summary>
        ///  The message type. See {@link MessageType}
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message
        /// </summary>
        public string Message { get; set; }
    }
}