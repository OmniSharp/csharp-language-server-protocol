using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// Document links have a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }
        /// <summary>
        /// Document link options
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// Document links have a resolve provider as well.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }
        }

        class DocumentLinkRegistrationOptionsConverter : RegistrationOptionsConverterBase<DocumentLinkRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public DocumentLinkRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.DocumentLinkProvider))
            {
                _handlersManager = handlersManager;
            }

            public override StaticOptions Convert(DocumentLinkRegistrationOptions source) => new StaticOptions {
                ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IDocumentLinkResolveHandler)),
                WorkDoneProgress = source.WorkDoneProgress,
            };
        }
    }
}
