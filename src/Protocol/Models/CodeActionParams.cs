using System.Text.Json.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Params for the CodeActionRequest
    /// </summary>
    [Method(DocumentNames.CodeAction)]
    public class CodeActionParams : ITextDocumentIdentifierParams, IRequest<CommandOrCodeActionContainer>, IWorkDoneProgressParams, IPartialItems<CodeActionOrCommand>
    {
        /// <summary>
        /// The document in which the command was invoked.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The range for which the command was invoked.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Context carrying additional information.
        /// </summary>
        public CodeActionContext Context { get; set; }

        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
