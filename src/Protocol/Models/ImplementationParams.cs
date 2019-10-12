using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ImplementationParams : TextDocumentPositionParams, IRequest<LocationOrLocationLinks>
    {

    }
}
