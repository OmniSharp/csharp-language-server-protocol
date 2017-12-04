using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CompletionCapability : DynamicCapability, ConnectedCapability<ICompletionHandler>
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        public CompletionItemCapability CompletionItem { get; set; }

        /// <summary>
        ///  The client supports to send additional context information for a `textDocument/completion` request.
        /// </summary>
        [Optional]
        public bool ContextSupport { get; set; }
    }
}
