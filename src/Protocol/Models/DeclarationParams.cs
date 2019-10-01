using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DeclarationParams : TextDocumentPositionParams, IRequest<LocationOrLocationLinks>
    {

    }
}
