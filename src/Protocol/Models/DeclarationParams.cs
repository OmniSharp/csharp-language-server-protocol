using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Declaration, Direction.ClientToServer)]
    public class DeclarationParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink>
    {
        /// <inheritdoc />
        public ProgressToken? PartialResultToken { get; set; }
    }
}
