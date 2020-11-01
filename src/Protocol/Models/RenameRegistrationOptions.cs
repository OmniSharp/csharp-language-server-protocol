using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RenameRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [Optional]
        public bool PrepareProvider { get; set; }

        public class StaticOptions : WorkDoneProgressOptions
        {
            /// <summary>
            /// Renames should be checked and tested before being executed.
            /// </summary>
            [Optional]
            public bool PrepareProvider { get; set; }
        }

        class RenameRegistrationOptionsConverter : RegistrationOptionsConverterBase<RenameRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public RenameRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.RenameProvider))
            {
                _handlersManager = handlersManager;
            }
            public override StaticOptions Convert(RenameRegistrationOptions source) => new StaticOptions {
                PrepareProvider = source.PrepareProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IPrepareRenameHandler)),
                WorkDoneProgress = source.WorkDoneProgress
            };
        }
    }
}
