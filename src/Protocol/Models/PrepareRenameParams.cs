using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.PrepareRename)]
    public class PrepareRenameParams : TextDocumentPositionParams, IRequest<RangeOrPlaceholderRange>
    {
    }
}
