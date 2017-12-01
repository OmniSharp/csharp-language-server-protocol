using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkRegistrationOptions : TextDocumentRegistrationOptions, IDocumentLinkOptions
    {
        /// <summary>
        /// Document links have a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }
    }
}
