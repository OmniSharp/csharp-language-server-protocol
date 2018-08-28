using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ImplementationParams : TextDocumentPositionParams, IRequest<LocationOrLocations>
    {

    }
}