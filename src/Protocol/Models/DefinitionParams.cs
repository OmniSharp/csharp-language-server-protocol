using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DefinitionParams : WorkDoneTextDocumentPositionParams, IRequest<LocationOrLocationLinks>, IPartialItems<LocationOrLocationLink>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
