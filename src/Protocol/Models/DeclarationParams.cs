using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DeclarationParams : WorkDoneTextDocumentPositionParams, IRequest<LocationOrLocationLinks>, IPartialItems<LocationLink>
    {
        /// <inheritdoc />
        public ProgressToken PartialResultToken { get; set; }
    }
}
