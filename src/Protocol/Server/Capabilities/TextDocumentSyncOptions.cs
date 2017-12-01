using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class TextDocumentSyncOptions
    {
        /// <summary>
        ///  Open and close notifications are sent to the server.
        /// </summary>
        [Optional]
        public bool OpenClose { get; set; }
        /// <summary>
        ///  Change notificatins are sent to the server. See TextDocumentSyncKind.None, TextDocumentSyncKind.Full
        ///  and TextDocumentSyncKindIncremental.
        /// </summary>
        [Optional]
        public TextDocumentSyncKind Change { get; set; }
        /// <summary>
        ///  Will save notifications are sent to the server.
        /// </summary>
        [Optional]
        public bool WillSave { get; set; }
        /// <summary>
        ///  Will save wait until requests are sent to the server.
        /// </summary>
        [Optional]
        public bool WillSaveWaitUntil { get; set; }
        /// <summary>
        ///  Save notifications are sent to the server.
        /// </summary>
        [Optional]
        public SaveOptions Save { get; set; }
    }
}
