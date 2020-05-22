namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ISynchronizationOptions
    {
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        bool WillSave { get; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        bool WillSaveWaitUntil { get; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        bool DidSave { get; }
    }
}
