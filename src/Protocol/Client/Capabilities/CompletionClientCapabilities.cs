using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionClientCapabilities : DynamicCapability, ConnectedCapability<ICompletionHandler>
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [Optional]
        public CompletionItemClientCapabilities CompletionItem { get; set; }

        /// <summary>
        /// Specific capabilities for the `CompletionItemKind` in the `textDocument/completion` request.
        /// </summary>
        [Optional]
        public CompletionItemKindClientCapabilities CompletionItemKind { get; set; }

        /// <summary>
        ///  The client supports to send additional context information for a `textDocument/completion` request.
        /// </summary>
        [Optional]
        public bool ContextSupport { get; set; }
    }
}
