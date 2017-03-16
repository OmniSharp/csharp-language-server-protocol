using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ReferenceParams : TextDocumentPositionParams
    {
        public ReferenceContext Context { get; set; }
    }
}