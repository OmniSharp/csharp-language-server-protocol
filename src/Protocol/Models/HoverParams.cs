using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class HoverParams : WorkDoneTextDocumentPositionParams, IRequest<Hover>, IWorkDoneProgressParams
    {
    }
}
