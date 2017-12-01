namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
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
