using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ReferenceRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class ReferenceRegistrationOptionsConverter : RegistrationOptionsConverterBase<ReferenceRegistrationOptions, StaticOptions>
        {
            public ReferenceRegistrationOptionsConverter() : base(nameof(ServerCapabilities.ReferencesProvider))
            {
            }
            public override StaticOptions Convert(ReferenceRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
