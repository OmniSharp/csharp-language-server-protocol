using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class HoverRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class HoverRegistrationOptionsConverter : RegistrationOptionsConverterBase<HoverRegistrationOptions, StaticOptions>
        {
            public HoverRegistrationOptionsConverter() : base(nameof(ServerCapabilities.HoverProvider))
            {
            }
            public override StaticOptions Convert(HoverRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
