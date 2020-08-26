using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CodeLensRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// Code lens has a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        /// <summary>
        /// Code Lens options.
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// Code lens has a resolve provider as well.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }
        }

        class CodeLensRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeLensRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public CodeLensRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CodeLensProvider))
            {
                _handlersManager = handlersManager;
            }
            public override StaticOptions Convert(CodeLensRegistrationOptions source)
            {
                return new StaticOptions {
                    ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICodeLensResolveHandler)),
                    WorkDoneProgress = source.WorkDoneProgress
                };
            }
        }
    }
}
