using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Hover, Direction.ClientToServer)]
    public class HoverParams : WorkDoneTextDocumentPositionParams, IRequest<Hover>, IWorkDoneProgressParams
    {
    }
}
