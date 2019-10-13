using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Params for the CodeActionRequest
    /// </summary>
    public class CodeActionParams : ITextDocumentIdentifierParams, IRequest<Container<CommandOrCodeAction>>, IWorkDoneProgressParams, IPartialItems<CodeActionOrCommand>
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
        [Optional]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
