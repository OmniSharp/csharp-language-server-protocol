using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ReferenceParams : TextDocumentPositionParams, IRequest<LocationContainer>
    {
        public ReferenceContext Context { get; set; }
    }
}
