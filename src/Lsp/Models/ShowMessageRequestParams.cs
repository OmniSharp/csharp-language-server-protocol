using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    ///  The show message request is sent from a server to a client to ask the client to display a particular message in the user interface. In addition to the show message notification the request allows to pass actions and to wait for an answer from the client.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ShowMessageRequestParams
    {
        /// <summary>
        ///  The message type. See {@link MessageType}
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        ///  The actual message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  The message action items to present.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<MessageActionItem> Actions { get; set; }
    }
}