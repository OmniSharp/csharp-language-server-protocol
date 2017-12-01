using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Document link options
    /// </summary>
    public class DocumentLinkOptions : IDocumentLinkOptions
    {
        /// <summary>
        ///  Document links have a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        public static DocumentLinkOptions Of(IDocumentLinkOptions options)
        {
            return new DocumentLinkOptions() { ResolveProvider = options.ResolveProvider };
        }
    }
}
