using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.Completion)]
    public class CompletionParams : WorkDoneTextDocumentPositionParams, IRequest<CompletionList>, IPartialItems<CompletionItem>
    {
        /// <summary>
        /// The completion context. This is only available it the client specifies to send
        /// this using `Capability.textDocument.completion.contextSupport === true`
        /// </summary>
        [Optional]
        public CompletionContext Context { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
