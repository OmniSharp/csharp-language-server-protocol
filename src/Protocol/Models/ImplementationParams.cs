using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Implementation, Direction.ClientToServer)]
    public class ImplementationParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
