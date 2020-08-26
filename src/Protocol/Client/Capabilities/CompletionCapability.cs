using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Completion))]
    public class CompletionCapability : DynamicCapability, ConnectedCapability<ICompletionHandler>
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [Optional]
        public CompletionItemCapabilityOptions? CompletionItem { get; set; }

        /// <summary>
        /// Specific capabilities for the `CompletionItemKind` in the `textDocument/completion` request.
        /// </summary>
        [Optional]
        public CompletionItemKindCapabilityOptions? CompletionItemKind { get; set; }

        /// <summary>
        /// The client supports to send additional context information for a `textDocument/completion` request.
        /// </summary>
        [Optional]
        public bool ContextSupport { get; set; }
    }
}
