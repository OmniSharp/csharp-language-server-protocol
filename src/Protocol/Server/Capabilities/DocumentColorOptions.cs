using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class DocumentColorOptions : StaticWorkDoneTextDocumentRegistrationOptions, IDocumentColorOptions
    {
        /// <summary>
        ///  Code lens has a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        public static DocumentColorOptions Of(IDocumentColorOptions options)
        {
            return new DocumentColorOptions() {
                ResolveProvider = options.ResolveProvider,
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
