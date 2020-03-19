using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ImplementationParams : WorkDoneTextDocumentPositionParams, IRequest<LocationOrLocationLinks>, IPartialItems<LocationOrLocationLink>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
