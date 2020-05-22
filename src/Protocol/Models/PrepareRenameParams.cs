using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.PrepareRename, Direction.ClientToServer)]
    public class PrepareRenameParams : TextDocumentPositionParams, IRequest<RangeOrPlaceholderRange>
    {
    }
}
