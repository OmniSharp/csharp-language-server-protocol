using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Completion, Direction.ClientToServer)]
    public class CompletionParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<CompletionList, CompletionItem>
    {
        /// <summary>
        /// The completion context. This is only available it the client specifies to send
        /// this using `Capability.textDocument.completion.contextSupport === true`
        /// </summary>
        [Optional]
        public CompletionContext? Context { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }
    }
}
