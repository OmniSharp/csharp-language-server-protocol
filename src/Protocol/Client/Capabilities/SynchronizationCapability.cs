using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SynchronizationCapability : DynamicCapability, ConnectedCapability<ITextDocumentSyncHandler>
    {
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        public bool WillSave { get; set; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        public bool WillSaveWaitUntil { get; set; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        public bool DidSave { get; set; }
    }
}
