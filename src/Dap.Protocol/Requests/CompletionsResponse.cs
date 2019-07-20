namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class CompletionsResponse
    {
        /// <summary>
        /// The possible completions for .
        /// </summary>
        public Container<CompletionItem> targets { get; set; }
    }

}
