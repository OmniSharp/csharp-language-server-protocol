using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class TextDocumentSyncCapability : DynamicCapability, ConnectedCapability<IDidChangeTextDocumentHandler>, ConnectedCapability<IDidCloseTextDocumentHandler>, ConnectedCapability<IDidOpenTextDocumentHandler>, ConnectedCapability<IDidSaveTextDocumentHandler>, ConnectedCapability<IWillSaveTextDocumentHandler>, ConnectedCapability<IWillSaveWaitUntilTextDocumentHandler>
    {
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        [Optional]
        public bool WillSave { get; set; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        [Optional]
        public bool WillSaveWaitUntil { get; set; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        [Optional]
        public bool DidSave { get; set; }
    }
}
