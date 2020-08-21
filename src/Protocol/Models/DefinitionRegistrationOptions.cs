using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DefinitionRegistrationOptions : WorkDoneTextDocumentRegistrationOptions
    {
        public class StaticOptions : StaticWorkDoneTextDocumentRegistrationOptions { }

        class DefinitionRegistrationOptionsConverter : RegistrationOptionsConverterBase<DefinitionRegistrationOptions, StaticOptions>
        {
            public DefinitionRegistrationOptionsConverter() : base(nameof(ServerCapabilities.DefinitionProvider))
            {
            }
            public override StaticOptions Convert(DefinitionRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
