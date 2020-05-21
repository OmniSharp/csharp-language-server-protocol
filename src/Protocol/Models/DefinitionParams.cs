using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Definition, Direction.ClientToServer)]
    public class DefinitionParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
