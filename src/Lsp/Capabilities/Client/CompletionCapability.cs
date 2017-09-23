namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    public class CompletionCapability : DynamicCapability
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        public CompletionItemCapability CompletionItem { get; set; }
    }
}
