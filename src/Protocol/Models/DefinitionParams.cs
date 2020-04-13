using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.Definition)]
    public class DefinitionParams : WorkDoneTextDocumentPositionParams, IRequest<LocationOrLocationLinks>, IPartialItems<LocationOrLocationLink>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
