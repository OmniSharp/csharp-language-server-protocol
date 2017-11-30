using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentSyncOptions
    {
        /// <summary>
        ///  Open and close notifications are sent to the server.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool OpenClose { get; set; }
        /// <summary>
        ///  Change notificatins are sent to the server. See TextDocumentSyncKind.None, TextDocumentSyncKind.Full
        ///  and TextDocumentSyncKindIncremental.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentSyncKind Change { get; set; }
        /// <summary>
        ///  Will save notifications are sent to the server.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool WillSave { get; set; }
        /// <summary>
        ///  Will save wait until requests are sent to the server.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool WillSaveWaitUntil { get; set; }
        /// <summary>
        ///  Save notifications are sent to the server.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SaveOptions Save { get; set; }
    }
}