using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class HoverParams : TextDocumentPositionParams, IRequest<Hover>
    {

    }
}