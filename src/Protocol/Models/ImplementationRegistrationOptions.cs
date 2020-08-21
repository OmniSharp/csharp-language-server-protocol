using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ImplementationRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions { }

        class ImplementationRegistrationOptionsConverter : RegistrationOptionsConverterBase<ImplementationRegistrationOptions, StaticOptions>
        {
            public ImplementationRegistrationOptionsConverter() : base(nameof(ServerCapabilities.ImplementationProvider))
            {
            }
            public override StaticOptions Convert(ImplementationRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
