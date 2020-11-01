using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public class MonikerRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// Code Lens options.
        /// </summary>
        public class StaticOptions : WorkDoneProgressOptions
        {
        }

        class MonikerRegistrationOptionsOptionsConverter : RegistrationOptionsConverterBase<MonikerRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public MonikerRegistrationOptionsOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.MonikerProvider))
            {
                _handlersManager = handlersManager;
            }
            public override StaticOptions Convert(MonikerRegistrationOptions source)
            {
                return new StaticOptions {
                    WorkDoneProgress = source.WorkDoneProgress,
                };
            }
        }
    }
}
