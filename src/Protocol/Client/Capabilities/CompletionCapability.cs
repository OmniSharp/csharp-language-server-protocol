using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionCapability : DynamicCapability, ConnectedCapability<ICompletionHandler>
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [Optional]
        public CompletionItemCapability CompletionItem { get; set; }

        /// <summary>
        /// Specific capabilities for the `CompletionItemKind` in the `textDocument/completion` request.
        /// </summary>
        [Optional]
        public CompletionItemKindCapability SymbolKind { get; set; }

        /// <summary>
        ///  The client supports to send additional context information for a `textDocument/completion` request.
        /// </summary>
        [Optional]
        public bool ContextSupport { get; set; }
    }


    public class CompletionItemKindCapability
    {
        /// <summary>
        /// The completion item kind values the client supports. When this
        /// property exists the client also guarantees that it will
        /// handle values outside its set gracefully and falls back
        /// to a default value when unknown.
        ///
        /// If this property is not present the client only supports
        /// the completion items kinds from `Text` to `Reference` as defined in
        /// the initial version of the protocol.
        /// </summary>
        [Optional]
        public Container<CompletionItemKind> ValueSet { get; set; }
    }
}
