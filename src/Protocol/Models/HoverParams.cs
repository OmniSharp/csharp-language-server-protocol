using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.Hover)]
    public class HoverParams : WorkDoneTextDocumentPositionParams, IRequest<Hover>, IWorkDoneProgressParams
    {
    }
}
