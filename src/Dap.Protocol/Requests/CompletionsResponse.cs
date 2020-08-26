using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class CompletionsResponse
    {
        /// <summary>
        /// The possible completions for .
        /// </summary>
        public Container<CompletionItem> Targets { get; set; } = null!;
    }
}
