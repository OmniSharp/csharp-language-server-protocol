using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentRegistrationOptions
    {
        /// <summary>
        ///  A document selector to identify the scope of the registration. If set to null
        ///  the document selector provided on the client side will be used.
        /// </summary>
        public DocumentSelector DocumentSelector { get; set; }
    }
}