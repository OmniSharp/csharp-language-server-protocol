using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    /// <summary>
    ///  The show message notification is sent from a server to a client to ask the client to display a particular message in the user interface.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ShowMessageParams
    {
        /// <summary>
        ///  The message type. See {@link MessageType}.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message.
        /// </summary>
        public string Message { get; set; }
    }
}