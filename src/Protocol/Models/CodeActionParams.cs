using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Params for the CodeActionRequest
    /// </summary>
    [Method(TextDocumentNames.CodeAction, Direction.ClientToServer)]
    public class CodeActionParams : ITextDocumentIdentifierParams, IPartialItemsRequest<CommandOrCodeActionContainer, CommandOrCodeAction>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The document in which the command was invoked.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; } = null!;

        /// <summary>
        /// The range for which the command was invoked.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// Context carrying additional information.
        /// </summary>
        public CodeActionContext Context { get; set; } = null!;

        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
