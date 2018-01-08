using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ReferenceParams : TextDocumentPositionParams
    {
        public ReferenceContext Context { get; set; }
    }
}
