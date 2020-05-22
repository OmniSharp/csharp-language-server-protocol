using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkRegistrationOptions : WorkDoneTextDocumentRegistrationOptions, IDocumentLinkOptions
    {
        /// <summary>
        /// Document links have a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }
    }
}
