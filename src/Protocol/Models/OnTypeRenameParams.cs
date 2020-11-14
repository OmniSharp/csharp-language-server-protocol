using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Parallel]
    [Method(TextDocumentNames.OnTypeRename, Direction.ClientToServer)]
    public class OnTypeRenameParams : WorkDoneTextDocumentPositionParams, IRequest<OnTypeRenameRanges>
    {
    }
}
