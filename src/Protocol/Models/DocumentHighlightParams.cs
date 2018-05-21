using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentHighlightParams : TextDocumentPositionParams, IRequest<DocumentHighlightContainer>
    {

    }
}