using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class CompletionCapability : DynamicCapability, ConnectedCapability<ICompletionHandler>
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        public CompletionItemCapability CompletionItem { get; set; }
    }
}
